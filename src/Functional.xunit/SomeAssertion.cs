using System.Diagnostics;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent wrapper for an Option.Some value, allowing for chained assertions
///     and transformations.
/// </summary>
/// <typeparam name="T">The type of the value contained in the Option.Some.</typeparam>
[DebuggerStepThrough]
public readonly struct SomeAssertion<T>
    where T : notnull
{
    /// <summary>
    ///     Stores the underlying value of the assertion, representing an existing valid value of type
    ///     <typeparamref name="T" />.
    /// </summary>
    /// <remarks>
    ///     The <c>_value</c> is used internally to hold the value being asserted upon within the fluent interface.
    ///     It is immutable and is utilized in operations like mapping or asserting further conditions.
    /// </remarks>
    private readonly T _value;

    /// <summary>
    ///     Represents a fluent assertion wrapper for an Option.Some value.
    ///     Provides methods for chaining additional assertions or transformations
    ///     on the encapsulated value.
    /// </summary>
    internal SomeAssertion(T value)
    {
        _value = value;
    }

    /// <summary>
    ///     Gets the value contained within the assertion.
    /// </summary>
    /// <remarks>
    ///     This property provides access to the underlying value of the assertion,
    ///     which represents a successful state for the assertion object.
    /// </remarks>
    public T Value => _value;

    /// <summary>
    ///     Chains an action to be executed on the encapsulated value of the current assertion.
    /// </summary>
    /// <param name="assert">The action to perform on the encapsulated value. Must not be null.</param>
    /// <returns>
    ///     The current <see cref="SomeAssertion{T}" /> instance, enabling further chained assertions or operations.
    /// </returns>
    public SomeAssertion<T> And(Action<T> assert)
    {
        assert(_value);
        return this;
    }

    /// <summary>
    ///     Applies a transformation function to the underlying value of the assertion
    ///     and returns a new <see cref="SomeAssertion{TOut}" /> instance with the transformed value.
    /// </summary>
    /// <typeparam name="TOut">The type of the transformed value.</typeparam>
    /// <param name="map">A function that defines how to transform the underlying value.</param>
    /// <returns>A new <see cref="SomeAssertion{TOut}" /> containing the transformed value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="map" /> function is null.</exception>
    public SomeAssertion<TOut> Map<TOut>(Func<T, TOut> map)
        where TOut : notnull
    {
        return new SomeAssertion<TOut>(map(_value));
    }

    /// <summary>
    ///     Deconstructs the current instance into its value component.
    /// </summary>
    /// <param name="value">The value contained within the current instance.</param>
    public void Deconstruct(out T value)
    {
        value = _value;
    }
}
