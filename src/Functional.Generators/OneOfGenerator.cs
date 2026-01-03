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
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the generation step
        context.RegisterPostInitializationOutput(ctx =>
        {
            // Generate OneOf for 2 to 10 type parameters
            for (var typeCount = 2; typeCount <= 10; typeCount++)
            {
                var source = GenerateOneOf(typeCount);
                ctx.AddSource($"OneOf{typeCount}.g.cs", SourceText.From(source, Encoding.UTF8));
            }
        });
    }

    private static string GenerateOneOf(int typeCount)
    {
        var sb = new StringBuilder();

        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System.Diagnostics;");
        sb.AppendLine("using System.Diagnostics.CodeAnalysis;");
        sb.AppendLine();
        sb.AppendLine("namespace UnambitiousFx.Functional;");
        sb.AppendLine();

        // Generate type parameters
        var typeParams = string.Join(", ", Enumerable.Range(1, typeCount).Select(i => $"T{i}"));
        var whereConstraints =
            string.Join("\n    ", Enumerable.Range(1, typeCount).Select(i => $"where T{i} : notnull"));

        sb.AppendLine("/// <summary>");
        sb.AppendLine(
            $"///     Minimal discriminated union representing a value that can be one of {typeCount} types.");
        sb.AppendLine("/// </summary>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine($"/// <typeparam name=\"T{i}\">{GetOrdinal(i)} possible contained type.</typeparam>");
        }

        sb.AppendLine("[DebuggerDisplay(\"{DebuggerDisplay,nq}\")]");
        sb.AppendLine(
            $"[DebuggerTypeProxy(typeof(OneOfDebugView{(typeCount == 2 ? "<,>" : $"<{new string(',', typeCount - 1)}>")}))]");
        sb.AppendLine($"public readonly record struct OneOf<{typeParams}>");
        sb.AppendLine($"    {whereConstraints}");
        sb.AppendLine("{");

        // Private fields
        sb.AppendLine("    private readonly byte _index;");
        sb.AppendLine("    private readonly object? _value;");
        sb.AppendLine();

        // DebuggerDisplay
        sb.Append("    private string DebuggerDisplay => _index switch { ");
        for (var i = 0; i < typeCount; i++)
        {
            sb.Append($"{i} => $\"{GetOrdinal(i + 1)}({{_value}})\", ");
        }

        sb.AppendLine("_ => \"Invalid\" };");
        sb.AppendLine();

        // Private constructor
        sb.AppendLine("    private OneOf(byte index, object? value)");
        sb.AppendLine("    {");
        sb.AppendLine("        _index = index;");
        sb.AppendLine("        _value = value;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Boolean properties (IsFirst, IsSecond, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine(
                $"    ///     True when the instance currently holds a value of the {GetOrdinalLower(i)} type.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    public bool Is{GetOrdinal(i)} => _index == {i - 1};");
            sb.AppendLine();
        }

        // Match method returning value
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    ///     Pattern match returning a value.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    /// <typeparam name=\"TOut\">The type of value to return</typeparam>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine(
                $"    /// <param name=\"{GetOrdinalLower(i)}Func\">Function to invoke when value is of type T{i}</param>");
        }

        sb.AppendLine("    /// <returns>The result of invoking the appropriate function</returns>");
        var matchParams = string.Join(", ",
            Enumerable.Range(1, typeCount).Select(i => $"Func<T{i}, TOut> {GetOrdinalLower(i)}Func"));
        sb.AppendLine($"    public TOut Match<TOut>({matchParams})");
        sb.AppendLine("    {");
        sb.AppendLine("        return _index switch");
        sb.AppendLine("        {");
        for (var i = 0; i < typeCount; i++)
        {
            sb.AppendLine($"            {i} => {GetOrdinalLower(i + 1)}Func((T{i + 1})_value!),");
        }

        sb.AppendLine("            _ => throw new InvalidOperationException(\"Invalid state\")");
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Match method executing actions
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    ///     Pattern match executing side-effect actions.");
        sb.AppendLine("    /// </summary>");
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine(
                $"    /// <param name=\"{GetOrdinalLower(i)}Action\">Action to invoke when value is of type T{i}</param>");
        }

        var matchActionParams = string.Join(", ",
            Enumerable.Range(1, typeCount).Select(i => $"Action<T{i}> {GetOrdinalLower(i)}Action"));
        sb.AppendLine($"    public void Match({matchActionParams})");
        sb.AppendLine("    {");
        sb.AppendLine("        switch (_index)");
        sb.AppendLine("        {");
        for (var i = 0; i < typeCount; i++)
        {
            sb.AppendLine($"            case {i}: {GetOrdinalLower(i + 1)}Action((T{i + 1})_value!); break;");
        }

        sb.AppendLine("            default: throw new InvalidOperationException(\"Invalid state\");");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();

        // TryGet methods (First, Second, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    ///     Attempts to extract the {GetOrdinalLower(i)} value.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine(
                $"    /// <param name=\"{GetOrdinalLower(i)}\">The {GetOrdinalLower(i)} value if present</param>");
            sb.AppendLine($"    /// <returns>True if the value is of type T{i}, false otherwise</returns>");
            sb.AppendLine($"    public bool {GetOrdinal(i)}([NotNullWhen(true)] out T{i}? {GetOrdinalLower(i)})");
            sb.AppendLine("    {");
            sb.AppendLine($"        if (_index == {i - 1})");
            sb.AppendLine("        {");
            sb.AppendLine($"            {GetOrdinalLower(i)} = (T{i})_value!;");
            sb.AppendLine("            return true;");
            sb.AppendLine("        }");
            sb.AppendLine($"        {GetOrdinalLower(i)} = default;");
            sb.AppendLine("        return false;");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    ///     Returns a string representation of the current value.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public override string? ToString()");
        sb.AppendLine("    {");
        sb.AppendLine("        return _value!.ToString();");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Static factory methods (FromFirst, FromSecond, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    ///     Creates a OneOf instance containing a {GetOrdinalLower(i)} value.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    /// <param name=\"value\">The {GetOrdinalLower(i)} value</param>");
            sb.AppendLine($"    /// <returns>A OneOf instance containing the {GetOrdinalLower(i)} value</returns>");
            sb.AppendLine($"    public static OneOf<{typeParams}> From{GetOrdinal(i)}(T{i} value)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return new OneOf<{typeParams}>({i - 1}, value);");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        // Implicit conversion operators
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    ///     Implicit conversion from {GetOrdinalLower(i)} type to OneOf.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    /// <param name=\"value\">The {GetOrdinalLower(i)} value</param>");
            sb.AppendLine($"    /// <returns>A OneOf instance containing the {GetOrdinalLower(i)} value</returns>");
            sb.AppendLine($"    public static implicit operator OneOf<{typeParams}>(T{i} value)");
            sb.AppendLine("    {");
            sb.AppendLine($"        return From{GetOrdinal(i)}(value);");
            sb.AppendLine("    }");
            sb.AppendLine();
        }

        sb.AppendLine("}");
        sb.AppendLine();

        // Generate debug view proxy class
        GenerateDebugViewProxy(sb, typeCount, typeParams, whereConstraints);

        return sb.ToString();
    }

    private static void GenerateDebugViewProxy(StringBuilder sb, int typeCount, string typeParams,
        string whereConstraints)
    {
        sb.AppendLine($"internal sealed class OneOfDebugView<{typeParams}>");
        sb.AppendLine($"    {whereConstraints}");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly OneOf<{typeParams}> _oneOf;");
        sb.AppendLine();
        sb.AppendLine($"    public OneOfDebugView(OneOf<{typeParams}> oneOf)");
        sb.AppendLine("    {");
        sb.AppendLine("        _oneOf = oneOf;");
        sb.AppendLine("    }");
        sb.AppendLine();

        // ActiveType property - use public IsFirst/IsSecond properties to determine active type
        sb.Append("    public string ActiveType => ");
        for (var i = 1; i <= typeCount; i++)
        {
            if (i > 1)
            {
                sb.Append(" : ");
            }

            sb.Append($"_oneOf.Is{GetOrdinal(i)} ? \"{GetOrdinal(i)}\"");
        }

        sb.AppendLine(" : \"Invalid\";");
        sb.AppendLine();

        // IsFirst, IsSecond, etc. properties
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine($"    public bool Is{GetOrdinal(i)} => _oneOf.Is{GetOrdinal(i)};");
        }

        sb.AppendLine();

        // Typed value properties (FirstValue, SecondValue, etc.)
        for (var i = 1; i <= typeCount; i++)
        {
            sb.AppendLine(
                $"    public T{i}? {GetOrdinal(i)}Value => _oneOf.{GetOrdinal(i)}(out var value) ? value : default;");
        }

        sb.AppendLine("}");
    }

    private static string GetOrdinal(int number)
    {
        return number switch
        {
            1 => "First",
            2 => "Second",
            3 => "Third",
            4 => "Fourth",
            5 => "Fifth",
            6 => "Sixth",
            7 => "Seventh",
            8 => "Eighth",
            9 => "Ninth",
            10 => "Tenth",
            _ => throw new ArgumentException("Only supports 1-10", nameof(number))
        };
    }

    private static string GetOrdinalLower(int number)
    {
        return number switch
        {
            1 => "first",
            2 => "second",
            3 => "third",
            4 => "fourth",
            5 => "fifth",
            6 => "sixth",
            7 => "seventh",
            8 => "eighth",
            9 => "ninth",
            10 => "tenth",
            _ => throw new ArgumentException("Only supports 1-10", nameof(number))
        };
    }
}
