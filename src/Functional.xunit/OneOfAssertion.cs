using System.Diagnostics;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent assertion wrapper for the one of value of a OneOf type.
/// </summary>
/// <typeparam name="TValue">The type of the first value. Must be non-nullable.</typeparam>
[DebuggerStepThrough]
public readonly struct OneOfAssertion<TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Represents a fluent assertion wrapper for the one of value of a OneOf type.
    /// </summary>
    internal OneOfAssertion(TValue value)
    {
        Value = value;
    }

    /// <summary>
    ///     Gets the current value of this assertion.
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    ///     Performs an assertion on the value encapsulated within the current instance
    ///     and returns the instance to facilitate further assertions in a fluent manner.
    /// </summary>
    /// <param name="assert">
    ///     An action that defines the assertion logic to be executed against the value.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="OneOfAssertion{TValue}" /> to enable method chaining.
    /// </returns>
    public OneOfAssertion<TValue> And(Action<TValue> assert)
    {
        assert(Value);
        return this;
    }

    /// <summary>
    ///     Maps the current instance of <see cref="OneOfAssertion{TValue}" /> to a new
    ///     <see cref="OneOfAssertion{TOut}" /> with the value projected by the provided function.
    /// </summary>
    /// <typeparam name="TOut">The type of the newly projected value, which must be non-nullable.</typeparam>
    /// <param name="projector">A function that takes the current value and projects it to a new value of type TOut.</param>
    /// <returns>A new instance of <see cref="OneOfAssertion{TOut}" /> containing the projected value.</returns>
    public OneOfAssertion<TOut> Map<TOut>(Func<TValue, TOut> projector)
        where TOut : notnull =>
        new(projector(Value));
}

/// <summary>
///     Represents an assertion utility for working with a <see cref="OneOf{TFirst,TSecond}" /> type.
///     Provides fluent assertion methods to validate the contents of a OneOf instance.
/// </summary>
/// <typeparam name="TFirst">The type of the first possible value contained in the OneOf instance.</typeparam>
/// <typeparam name="TSecond">The type of the second possible value contained in the OneOf instance.</typeparam>
[DebuggerStepThrough]
public readonly struct OneOfAssertion<TFirst, TSecond>
    where TFirst : notnull
    where TSecond : notnull
{
    private readonly OneOf<TFirst, TSecond> _oneOf;

    /// <summary>
    ///     Provides a fluent assertion utility for working with instances of <see cref="OneOf{TFirst, TSecond}" />.
    ///     Enables validation of a OneOf type's contents through expressive and readable methods.
    /// </summary>
    public OneOfAssertion(OneOf<TFirst, TSecond> oneOf)
    {
        _oneOf = oneOf;
    }

    /// <summary>
    ///     Attempts to create a fluent assertion wrapper for the first value of a
    ///     OneOf type. Throws an assertion failure if the OneOf does not contain
    ///     a value of the first type.
    /// </summary>
    /// <returns>
    ///     A fluent assertion wrapper for the first value of the OneOf.
    /// </returns>
    public OneOfAssertion<TFirst> First()
    {
        if (!_oneOf.First(out var value))
        {
            Assert.Fail("Expected OneOf.First but was Second.");
        }

        return new OneOfAssertion<TFirst>(value);
    }

    /// <summary>
    ///     Asserts that the current instance of OneOf contains a value of type TSecond.
    ///     If the value is not of type TSecond, the method fails the assertion.
    /// </summary>
    /// <returns>
    ///     A fluent assertion wrapper of type <see cref="OneOfAssertion{TSecond}" /> for performing
    ///     additional checks or transformations on the second value.
    /// </returns>
    public OneOfAssertion<TSecond> Second()
    {
        if (!_oneOf.Second(out var value))
        {
            Assert.Fail("Expected OneOf.Second but was First.");
        }

        return new OneOfAssertion<TSecond>(value);
    }
}
