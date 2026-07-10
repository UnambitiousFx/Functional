#if NET11_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UnambitiousFx.Functional;

/// <summary>
///     C# 15 union support for <see cref="Maybe{TValue}" />: a single-case union whose case type is
///     <typeparamref name="TValue" />; None is represented by a null <see cref="Value" />, enabling
///     exhaustive <c>switch</c> matching (<c>TValue</c> + <c>null</c> arms) on net11.0+.
/// </summary>
[Union]
public readonly partial record struct Maybe<TValue> : IUnion, Maybe<TValue>.IUnionMembers
{
    /// <summary>
    ///     Gets the union view of this maybe: the value when Some, <see langword="null" /> when None.
    /// </summary>
    public object? Value => IsSome
                                ? _value
                                : null;

    /// <summary>
    ///     Gets a value indicating whether the union holds a value (equivalent to <see cref="IsSome" />).
    /// </summary>
    public bool HasValue => IsSome;

    /// <summary>
    ///     Attempts to extract the value (non-boxing union access for the <typeparamref name="TValue" /> case).
    /// </summary>
    /// <param name="value">The value if Some, otherwise null.</param>
    /// <returns>True if Some, false otherwise.</returns>
    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        value = _value;
        return IsSome;
    }

    /// <summary>
    ///     C# 15 union construction surface. Constructors stay private; creation delegates
    ///     to the existing factory methods.
    /// </summary>
    public interface IUnionMembers
    {
        /// <summary>Creates a Some maybe (union creation member).</summary>
        /// <param name="value">The value.</param>
        /// <returns>A maybe containing the value.</returns>
        static Maybe<TValue> Create(TValue value)
        {
            return Some(value);
        }

        /// <summary>Gets the union view of the maybe.</summary>
        object? Value { get; }
    }
}
#endif
