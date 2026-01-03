using System.Diagnostics;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a utility for asserting successful states in unit tests, without encapsulating
///     a specific value.
/// </summary>
[DebuggerStepThrough]
public readonly struct SuccessAssertion
{
}

/// <summary>
///     Represents a fluent assertion mechanism for successfully concluded operations, encapsulating a non-nullable
///     value.
/// </summary>
[DebuggerStepThrough]
public readonly struct SuccessAssertion<TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Holds the success result value for the current assertion context.
    /// </summary>
    /// <remarks>
    ///     This variable is initialized via the constructor of the <see cref="SuccessAssertion{TValue}" /> struct
    ///     and represents the underlying success value being validated or transformed through fluent chaining.
    /// </remarks>
    private readonly TValue _value;

    /// <summary>
    ///     Provides a fluent API for asserting success values in a result type.
    /// </summary>
    internal SuccessAssertion(TValue value)
    {
        _value = value;
    }

    /// <summary>
    ///     Gets the underlying value of the success assertion.
    /// </summary>
    /// <remarks>
    ///     This property exposes the wrapped value within a successful result, allowing for further operations or assertions.
    /// </remarks>
    public TValue Value => _value;

    /// <summary>
    ///     Executes additional assertions on the success value while maintaining a fluent context.
    /// </summary>
    /// <param name="assert">An action to perform custom assertions on the success value.</param>
    /// <returns>The current <see cref="SuccessAssertion{TValue}" /> instance, enabling further chaining.</returns>
    public SuccessAssertion<TValue> And(Action<TValue> assert)
    {
        assert(_value);
        return this;
    }

    /// <summary>
    ///     Projects the value to another type while maintaining the fluent context.
    /// </summary>
    /// <param name="map">A function that transforms the current value into a new value of the specified type.</param>
    /// <typeparam name="TOut">The type of the new value returned by the projection.</typeparam>
    /// <returns>A new success assertion containing the projected value.</returns>
    public SuccessAssertion<TOut> Map<TOut>(Func<TValue, TOut> map)
        where TOut : notnull
    {
        return new SuccessAssertion<TOut>(map(_value));
    }

    /// <summary>Converts the current value into a new Result using the specified projector function.</summary>
    /// <param name="projector">The function used to transform the current value into a new Result.</param>
    /// <typeparam name="TOut">The type of the value encapsulated by the new Result.</typeparam>
    /// <returns>A Result containing the projected value.</returns>
    public Result<TOut> ToResult<TOut>(Func<TValue, TOut> projector)
        where TOut : notnull
    {
        return Result.Success(projector(_value));
    }

    /// <summary>
    ///     Deconstructs the success assertion to extract its value.
    /// </summary>
    /// <param name="value">The value contained in the success assertion.</param>
    public void Deconstruct(out TValue value)
    {
        value = _value;
    }
}
