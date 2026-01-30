using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace UnambitiousFx.Functional.Generators;

/// <summary>
///     Source generator that creates OneOf types for 2 to 10 generic type parameters.
/// </summary>
[Generator]
public class OneOfGenerator : IIncrementalGenerator
{
    private const int MinTypeCount = 2;
    private const int MaxTypeCount = 10;

    private static readonly string[] Ordinals =
    [
        "First",
        "Second",
        "Third",
        "Fourth",
        "Fifth",
        "Sixth",
        "Seventh",
        "Eighth",
        "Ninth",
        "Tenth"
    ];

    private static readonly string[] OrdinalsLower =
    [
        "first",
        "second",
        "third",
        "fourth",
        "fifth",
        "sixth",
        "seventh",
        "eighth",
        "ninth",
        "tenth"
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the generation step
        context.RegisterPostInitializationOutput(ctx =>
        {
            // Generate OneOf for 2 to 10 type parameters
            for (var typeCount = MinTypeCount; typeCount <= MaxTypeCount; typeCount++)
            {
                var source = GenerateOneOf(typeCount);
                ctx.AddSource($"OneOf{typeCount}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        });
    }

    private static string GenerateOneOf(int typeCount)
    {
        var sb = new TemplateBuilder();

        WriteFileHeader(sb);
        var typeParams = BuildTypeParams(typeCount);
        var whereConstraints = BuildWhereConstraints(typeCount);
        WriteOneOfInterface(sb, typeCount, typeParams, whereConstraints);
        WriteOneOfType(sb, typeCount, typeParams, whereConstraints);

        // Generate debug view proxy class
        GenerateDebugViewProxy(sb, typeCount, typeParams, whereConstraints);

        return sb.ToString();
    }

    private static void WriteFileHeader(TemplateBuilder sb)
    {
        sb.Line("#nullable enable");
        sb.Line("using System.Diagnostics;");
        sb.Line("using System.Diagnostics.CodeAnalysis;");
        sb.Line();
        sb.Line("namespace UnambitiousFx.Functional;");
        sb.Line();
    }

    private static void WriteOneOfInterface(TemplateBuilder sb, int typeCount, string typeParams,
        IEnumerable<string> whereConstraints)
    {
        WriteOneOfInterfaceDocs(sb, typeCount);
        sb.Line($"public interface IOneOf<{typeParams}>");
        using (sb.Indent())
        {
            foreach (var constraint in whereConstraints)
            {
                sb.Line(constraint);
            }
        }

        sb.Line("{");
        using (sb.Indent())
        {
            WriteInterfaceIsProperties(sb, typeCount);
            WriteInterfaceMatchValue(sb, typeCount);
            WriteInterfaceMatchActions(sb, typeCount);
            WriteInterfaceBind(sb, typeCount);
            WriteInterfaceTryGetMethods(sb, typeCount);
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteOneOfInterfaceDocs(TemplateBuilder sb, int typeCount)
    {
        sb.Line("/// <summary>");
        sb.Line($"///     Minimal discriminated union interface for values that can be one of {typeCount} types.");
        sb.Line("/// </summary>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line($"/// <typeparam name=\"T{i}\">{GetOrdinal(i)} possible contained type.</typeparam>");
        }
    }

    private static void WriteOneOfType(TemplateBuilder sb, int typeCount, string typeParams,
        IEnumerable<string> whereConstraints)
    {
        WriteOneOfDocs(sb, typeCount);
        sb.Line("[DebuggerDisplay(\"{DebuggerDisplay,nq}\")]");
        sb.Line($"[DebuggerTypeProxy(typeof(OneOfDebugView{BuildGenericTypeArity(typeCount)}))]");
        sb.Line($"public readonly record struct OneOf<{typeParams}> : IOneOf<{typeParams}>");
        using (sb.Indent())
        {
            foreach (var constraint in whereConstraints)
            {
                sb.Line(constraint);
            }
        }

        sb.Line("{");
        using (sb.Indent())
        {
            WriteOneOfFields(sb);
            WriteOneOfDebuggerDisplay(sb, typeCount);
            WriteOneOfConstructor(sb);
            WriteIsProperties(sb, typeCount);
            WriteMatchValue(sb, typeCount);
            WriteMatchActions(sb, typeCount);
            WriteBind(sb, typeCount);
            WriteTryGetMethods(sb, typeCount);
            WriteToString(sb);
            WriteFactoryMethods(sb, typeCount, typeParams);
            WriteImplicitOperators(sb, typeCount, typeParams);
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteOneOfDocs(TemplateBuilder sb, int typeCount)
    {
        sb.Line("/// <summary>");
        sb.Line($"///     Minimal discriminated union representing a value that can be one of {typeCount} types.");
        sb.Line("/// </summary>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line($"/// <typeparam name=\"T{i}\">{GetOrdinal(i)} possible contained type.</typeparam>");
        }
    }

    private static void WriteOneOfFields(TemplateBuilder sb)
    {
        // Private fields
        sb.Line("private readonly byte _index;");
        sb.Line("private readonly object? _value;");
        sb.Line();
    }

    private static void WriteOneOfDebuggerDisplay(TemplateBuilder sb, int typeCount)
    {
        // DebuggerDisplay
        sb.AppendIndented("private string DebuggerDisplay => _index switch { ");
        for (var i = 0; i < typeCount; i++)
        {
            sb.AppendRaw($"{i} => $\"{GetOrdinal(i + 1)}({{_value}})\", ");
        }

        sb.LineRaw("_ => \"Invalid\" };");
        sb.Line();
    }

    private static void WriteOneOfConstructor(TemplateBuilder sb)
    {
        // Private constructor
        sb.Line("private OneOf(byte index, object? value)");
        sb.Line("{");
        using (sb.Indent())
        {
            sb.Line("_index = index;");
            sb.Line("_value = value;");
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteIsProperties(TemplateBuilder sb, int typeCount)
    {
        // Boolean properties (IsFirst, IsSecond, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line("/// <inheritdoc/>");
            sb.Line($"public bool Is{GetOrdinal(i)} => _index == {i - 1};");
            sb.Line();
        }
    }

    private static void WriteInterfaceIsProperties(TemplateBuilder sb, int typeCount)
    {
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line("/// <summary>");
            sb.Line($"///     True when the instance currently holds a value of the {GetOrdinalLower(i)} type.");
            sb.Line("/// </summary>");
            sb.Line($"public bool Is{GetOrdinal(i)} {{ get; }}");
            sb.Line();
        }
    }

    private static void WriteMatchValue(TemplateBuilder sb, int typeCount)
    {
        // Match method returning value
        sb.Line("/// <inheritdoc/>");
        var matchParams = string.Join(", ",
            Enumerable.Range(1, typeCount).Select(i => $"Func<T{i}, TOut> {GetOrdinalLower(i)}Func"));
        sb.Line($"public TOut Match<TOut>({matchParams})");
        sb.Line("{");
        using (sb.Indent())
        {
            sb.Line("return _index switch");
            sb.Line("{");
            using (sb.Indent())
            {
                for (var i = 0; i < typeCount; i++)
                {
                    sb.Line($"{i} => {GetOrdinalLower(i + 1)}Func((T{i + 1})_value!),");
                }

                sb.Line("_ => throw new InvalidOperationException(\"Invalid state\")");
            }

            sb.Line("};");
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteInterfaceMatchValue(TemplateBuilder sb, int typeCount)
    {
        sb.Line("/// <summary>");
        sb.Line("///     Pattern match returning a value.");
        sb.Line("/// </summary>");
        sb.Line("/// <typeparam name=\"TOut\">The type of value to return</typeparam>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line(
                $"/// <param name=\"{GetOrdinalLower(i)}Func\">Function to invoke when value is of type T{i}</param>");
        }

        sb.Line("/// <returns>The result of invoking the appropriate function</returns>");
        var matchParams = string.Join(", ",
            Enumerable.Range(1, typeCount).Select(i => $"Func<T{i}, TOut> {GetOrdinalLower(i)}Func"));
        sb.Line($"public TOut Match<TOut>({matchParams});");
        sb.Line();
    }

    private static void WriteMatchActions(TemplateBuilder sb, int typeCount)
    {
        // Match method executing actions
        sb.Line("/// <inheritdoc/>");
        var matchActionParams = string.Join(", ",
            Enumerable.Range(1, typeCount).Select(i => $"Action<T{i}> {GetOrdinalLower(i)}Action"));
        sb.Line($"public void Match({matchActionParams})");
        sb.Line("{");
        using (sb.Indent())
        {
            sb.Line("switch (_index)");
            sb.Line("{");
            using (sb.Indent())
            {
                for (var i = 0; i < typeCount; i++)
                {
                    sb.Line($"case {i}: {GetOrdinalLower(i + 1)}Action((T{i + 1})_value!); break;");
                }

                sb.Line("default: throw new InvalidOperationException(\"Invalid state\");");
            }

            sb.Line("}");
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteBind(TemplateBuilder sb, int typeCount)
    {
        sb.Line("/// <inheritdoc/>");
        var outTypeParams = BuildTypeParams(typeCount, "TOut");
        var outWhereConstraints = BuildWhereConstraints(typeCount, "TOut");
        var bindParams = string.Join(", ",
            Enumerable.Range(1, typeCount)
                .Select(i => $"Func<T{i}, OneOf<{outTypeParams}>> {GetOrdinalLower(i)}Func"));

        sb.Line($"public OneOf<{outTypeParams}> Bind<{outTypeParams}>({bindParams})");
        foreach (var constraint in outWhereConstraints)
        {
            sb.Line(constraint);
        }

        sb.Line("{");
        using (sb.Indent())
        {
            sb.Line($"return Match({BuildOrdinalParameterList(typeCount, "Func")});");
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteInterfaceMatchActions(TemplateBuilder sb, int typeCount)
    {
        sb.Line("/// <summary>");
        sb.Line("///     Pattern match executing side-effect actions.");
        sb.Line("/// </summary>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line(
                $"/// <param name=\"{GetOrdinalLower(i)}Action\">Action to invoke when value is of type T{i}</param>");
        }

        var matchActionParams = string.Join(", ",
            Enumerable.Range(1, typeCount).Select(i => $"Action<T{i}> {GetOrdinalLower(i)}Action"));
        sb.Line($"public void Match({matchActionParams});");
        sb.Line();
    }

    private static void WriteInterfaceBind(TemplateBuilder sb, int typeCount)
    {
        sb.Line("/// <summary>");
        sb.Line("///     Binds to a new OneOf by applying the matching function.");
        sb.Line("/// </summary>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line($"/// <typeparam name=\"TOut{i}\">{GetOrdinal(i)} output type.</typeparam>");
        }

        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line(
                $"/// <param name=\"{GetOrdinalLower(i)}Func\">Function to invoke when value is of type T{i}</param>");
        }

        sb.Line("/// <returns>The OneOf produced by the matching function</returns>");
        var outTypeParams = BuildTypeParams(typeCount, "TOut");
        var outWhereConstraints = BuildWhereConstraints(typeCount, "TOut");
        var bindParams = string.Join(", ",
            Enumerable.Range(1, typeCount)
                .Select(i => $"Func<T{i}, OneOf<{outTypeParams}>> {GetOrdinalLower(i)}Func"));
        sb.Line($"public OneOf<{outTypeParams}> Bind<{outTypeParams}>({bindParams})");
        foreach (var constraint in outWhereConstraints)
        {
            sb.Line(constraint);
        }

        sb.Line(";");
        sb.Line();
    }

    private static void WriteTryGetMethods(TemplateBuilder sb, int typeCount)
    {
        // TryGet methods (First, Second, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line("/// <inheritdoc/>");
            sb.Line($"public bool TryGet{GetOrdinal(i)}([NotNullWhen(true)] out T{i}? {GetOrdinalLower(i)})");
            sb.Line("{");
            using (sb.Indent())
            {
                sb.Line($"if (_index == {i - 1})");
                sb.Line("{");
                using (sb.Indent())
                {
                    sb.Line($"{GetOrdinalLower(i)} = (T{i})_value!;");
                    sb.Line("return true;");
                }

                sb.Line("}");
                sb.Line($"{GetOrdinalLower(i)} = default;");
                sb.Line("return false;");
            }

            sb.Line("}");
            sb.Line();
        }
    }

    private static void WriteInterfaceTryGetMethods(TemplateBuilder sb, int typeCount)
    {
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line("/// <summary>");
            sb.Line($"///     Attempts to extract the {GetOrdinalLower(i)} value.");
            sb.Line("/// </summary>");
            sb.Line($"/// <param name=\"{GetOrdinalLower(i)}\">The {GetOrdinalLower(i)} value if present</param>");
            sb.Line($"/// <returns>True if the value is of type T{i}, false otherwise</returns>");
            sb.Line($"public bool TryGet{GetOrdinal(i)}([NotNullWhen(true)] out T{i}? {GetOrdinalLower(i)});");
            sb.Line();
        }
    }

    private static void WriteToString(TemplateBuilder sb)
    {
        sb.Line("/// <summary>");
        sb.Line("///     Returns a string representation of the current value.");
        sb.Line("/// </summary>");
        sb.Line("public override string? ToString()");
        sb.Line("{");
        using (sb.Indent())
        {
            sb.Line("return _value!.ToString();");
        }

        sb.Line("}");
        sb.Line();
    }

    private static void WriteFactoryMethods(TemplateBuilder sb, int typeCount, string typeParams)
    {
        // Static factory methods (FromFirst, FromSecond, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line("/// <summary>");
            sb.Line($"///     Creates a OneOf instance containing a {GetOrdinalLower(i)} value.");
            sb.Line("/// </summary>");
            sb.Line($"/// <param name=\"value\">The {GetOrdinalLower(i)} value</param>");
            sb.Line($"/// <returns>A OneOf instance containing the {GetOrdinalLower(i)} value</returns>");
            sb.Line($"public static OneOf<{typeParams}> From{GetOrdinal(i)}(T{i} value)");
            sb.Line("{");
            using (sb.Indent())
            {
                sb.Line($"return new OneOf<{typeParams}>({i - 1}, value);");
            }

            sb.Line("}");
            sb.Line();
        }
    }

    private static void WriteImplicitOperators(TemplateBuilder sb, int typeCount, string typeParams)
    {
        // Implicit conversion operators
        for (var i = 1; i <= typeCount; i++)
        {
            sb.Line("/// <summary>");
            sb.Line($"///     Implicit conversion from {GetOrdinalLower(i)} type to OneOf.");
            sb.Line("/// </summary>");
            sb.Line($"/// <param name=\"value\">The {GetOrdinalLower(i)} value</param>");
            sb.Line($"/// <returns>A OneOf instance containing the {GetOrdinalLower(i)} value</returns>");
            sb.Line($"public static implicit operator OneOf<{typeParams}>(T{i} value)");
            sb.Line("{");
            using (sb.Indent())
            {
                sb.Line($"return From{GetOrdinal(i)}(value);");
            }

            sb.Line("}");
            sb.Line();
        }
    }

    private static void GenerateDebugViewProxy(TemplateBuilder sb, int typeCount, string typeParams,
        IEnumerable<string> whereConstraints)
    {
        sb.Line($"internal sealed class OneOfDebugView<{typeParams}>");
        using (sb.Indent())
        {
            foreach (var constraint in whereConstraints)
            {
                sb.Line(constraint);
            }
        }

        sb.Line("{");
        using (sb.Indent())
        {
            sb.Line($"private readonly OneOf<{typeParams}> _oneOf;");
            sb.Line();
            sb.Line($"public OneOfDebugView(OneOf<{typeParams}> oneOf)");
            sb.Line("{");
            using (sb.Indent())
            {
                sb.Line("_oneOf = oneOf;");
            }

            sb.Line("}");
            sb.Line();

            // ActiveType property - use public IsFirst/IsSecond properties to determine active type
            sb.AppendIndented("public string ActiveType => ");
            for (var i = 1; i <= typeCount; i++)
            {
                if (i > 1)
                {
                    sb.AppendRaw(" : ");
                }

                sb.AppendRaw($"_oneOf.Is{GetOrdinal(i)} ? \"{GetOrdinal(i)}\"");
            }

            sb.LineRaw(" : \"Invalid\";");
            sb.Line();

            // IsFirst, IsSecond, etc. properties
            for (var i = 1; i <= typeCount; i++)
            {
                sb.Line($"public bool Is{GetOrdinal(i)} => _oneOf.Is{GetOrdinal(i)};");
            }

            sb.Line();

            // Typed value properties (FirstValue, SecondValue, etc.)
            for (var i = 1; i <= typeCount; i++)
            {
                sb.Line(
                    $"public T{i}? {GetOrdinal(i)}Value => _oneOf.TryGet{GetOrdinal(i)}(out var value) ? value : default;");
            }
        }

        sb.Line("}");
    }

    private static string GetOrdinal(int number)
    {
        return GetOrdinalFrom(Ordinals, number);
    }

    private static string GetOrdinalLower(int number)
    {
        return GetOrdinalFrom(OrdinalsLower, number);
    }

    private static string GetOrdinalFrom(IReadOnlyList<string> ordinals, int number)
    {
        if (number < 1 || number > ordinals.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(number), number,
                $"Only supports 1-{ordinals.Count}");
        }

        return ordinals[number - 1];
    }

    private static string BuildTypeParams(int typeCount)
    {
        return BuildTypeParams(typeCount, "T");
    }

    private static string BuildTypeParams(int typeCount, string prefix)
    {
        return string.Join(", ", Enumerable.Range(1, typeCount).Select(i => $"{prefix}{i}"));
    }

    private static ImmutableArray<string> BuildWhereConstraints(int typeCount)
    {
        return BuildWhereConstraints(typeCount, "T");
    }

    private static ImmutableArray<string> BuildWhereConstraints(int typeCount, string prefix)
    {
        return [..Enumerable.Range(1, typeCount).Select(i => $"where {prefix}{i} : notnull")];
    }

    private static string BuildGenericTypeArity(int typeCount)
    {
        return $"<{new string(',', typeCount - 1)}>";
    }

    private static string BuildOrdinalParameterList(int typeCount, string suffix)
    {
        return string.Join(", ", Enumerable.Range(1, typeCount).Select(i => $"{GetOrdinalLower(i)}{suffix}"));
    }

    private sealed class TemplateBuilder
    {
        private const int IndentSize = 4;
        private readonly StringBuilder _sb = new();
        private int _indentLevel;

        public IDisposable Indent()
        {
            _indentLevel++;
            return new IndentScope(this);
        }

        public void Line(string? text = null)
        {
            if (text is null)
            {
                _sb.AppendLine();
                return;
            }

            AppendIndent();
            _sb.AppendLine(text);
        }

        public void AppendIndented(string text)
        {
            AppendIndent();
            _sb.Append(text);
        }

        public void AppendRaw(string text)
        {
            _sb.Append(text);
        }

        public void LineRaw(string text)
        {
            _sb.AppendLine(text);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        private void AppendIndent()
        {
            if (_indentLevel > 0)
            {
                _sb.Append(' ', _indentLevel * IndentSize);
            }
        }

        private sealed class IndentScope : IDisposable
        {
            private readonly TemplateBuilder _builder;
            private bool _disposed;

            public IndentScope(TemplateBuilder builder)
            {
                _builder = builder;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _builder._indentLevel = Math.Max(0, _builder._indentLevel - 1);
            }
        }
    }
}
