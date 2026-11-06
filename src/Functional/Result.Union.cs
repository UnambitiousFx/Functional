#if NET11_0_OR_GREATER
using System.Runtime.CompilerServices;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     C# 15 union support for <see cref="Result{TValue}" />: the union's case types are
///     <typeparamref name="TValue" /> (success) and <see cref="Failure" /> (failure), enabling
///     exhaustive <c>switch</c> matching and implicit unwrapping pattern matching on net11.0+.
/// </summary>
/// <remarks>
///     A result is <em>always</em> a success or a failure — never absent — so the union exposes a
///     <strong>non-null</strong> <see cref="Value" /> and its exhaustive <c>switch</c> needs only the
///     <c>TValue</c> and <see cref="Failure" /> arms (no <c>null</c> arm). A degenerate
///     <c>default(Result&lt;TValue&gt;)</c> is treated as a failure via <see cref="UninitializedFailure" />.
/// </remarks>
[Union]
public readonly partial record struct Result<TValue> : IUnion, Result<TValue>.IUnionMembers
{
    /// <summary>
    ///     The failure yielded for a degenerate <c>default(Result&lt;TValue&gt;)</c> instance, so the
    ///     union <see cref="Value" /> is never null and <c>default</c> lands on the failure arm rather
    ///     than throwing an unmatched-arm exception.
    /// </summary>
    private static readonly Failure UninitializedFailure =
        new("Result was used uninitialized (default value).");

    /// <summary>
    ///     Gets the union view of this result: the success value, or the failure. Never <see langword="null" />
    ///     — a <c>default(Result&lt;TValue&gt;)</c> yields <see cref="UninitializedFailure" />.
    /// </summary>
    /// <remarks>
    ///     Union matching yields the case value only — result metadata is not carried.
    ///     Use <see cref="Metadata" /> or <c>Match</c> when metadata matters. Reading this property
    ///     boxes a value-type success; prefer typed <c>switch</c>/<c>is</c> arms (routed through
    ///     the non-boxing <c>TryGetValue</c>).
    /// </remarks>
    public object Value => IsSuccess
                               ? _value!
                               : _error ?? UninitializedFailure;

    /// <summary>
    ///     Gets a value indicating whether the union holds a case value. Always <see langword="true" />:
    ///     the union view is never null (a default result is treated as a failure).
    /// </summary>
    public bool HasValue => true;

    // The TValue case of the non-boxing access pattern is served by the existing
    // TryGetValue(out TValue?) member. No TryGetValue(out Failure?) overload: it would make
    // every `TryGetValue(out var …)` call ambiguous, and Failure is a reference type, so
    // matching it through Value never boxes.

    /// <summary>
    ///     C# 15 union construction surface. Constructors stay internal; creation delegates
    ///     to the existing factory methods.
    /// </summary>
    public interface IUnionMembers
    {
        /// <summary>Creates a successful result (union creation member).</summary>
        /// <param name="value">The success value.</param>
        /// <returns>A successful result containing the value.</returns>
        static Result<TValue> Create(TValue value)
        {
            return Result.Success(value);
        }

        /// <summary>Creates a failed result (union creation member).</summary>
        /// <param name="error">The failure.</param>
        /// <returns>A failed result containing the failure.</returns>
        static Result<TValue> Create(Failure error)
        {
            return Result.Failure<TValue>(error);
        }

        /// <summary>Gets the non-null union view of the result.</summary>
        object Value { get; }
    }
}
#endif
