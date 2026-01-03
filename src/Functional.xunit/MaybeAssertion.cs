using System.Diagnostics;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents an assertion utility for the <see cref="Maybe{TValue}" /> type,
///     allowing for fluent testing of whether a <see cref="Maybe{TValue}" /> instance
///     contains a value (Some) or is empty (None).
/// </summary>
/// <typeparam name="TValue">
///     The type of the value held by the <see cref="Maybe{TValue}" /> instance.
///     This type must be non-nullable.
/// </typeparam>
[DebuggerStepThrough]
public readonly struct MaybeAssertion<TValue> where TValue : notnull
{
    private readonly Maybe<TValue> _maybe;

    /// <summary>
    ///     Represents an assertion utility for the <see cref="Maybe{TValue}" /> type,
    ///     allowing fluent assertion of whether a <see cref="Maybe{TValue}" /> instance
    ///     contains a value or no value (Some or None).
    /// </summary>
    public MaybeAssertion(Maybe<TValue> maybe)
    {
        _maybe = maybe;
    }

    /// <summary>
    ///     Asserts that the current instance of <see cref="Maybe{TValue}" /> contains a value (Option.Some).
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="SomeAssertion{TValue}" /> containing the value if the assertion is successful.
    ///     Throws an assertion failure if the current instance does not represent an Option.Some.
    /// </returns>
    public SomeAssertion<TValue> Some()
    {
        if (!_maybe.Some(out var value))
        {
            Assert.Fail("Expected Option.Some but was None.");
        }

        return new SomeAssertion<TValue>(value);
    }

    /// <summary>
    ///     Ensures that the current instance of <see cref="Maybe{TValue}" /> is in a "none" state.
    /// </summary>
    /// <returns>An instance of <see cref="NoneAssertion" /> for chaining further none-specific assertions.</returns>
    public NoneAssertion None()
    {
        if (_maybe.IsSome)
        {
            Assert.Fail("Expected Option.None but was Some.");
        }

        return new NoneAssertion();
    }
}
