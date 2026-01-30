using System.Diagnostics;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a utility for asserting the state of a <see cref="Result" /> instance in unit tests.
///     This struct provides methods for verifying whether a result indicates success or failure,
///     and facilitates further validation of associated data or errors.
/// </summary>
[DebuggerStepThrough]
public readonly struct ResultAssertion
{
    private readonly Result _result;

    /// <summary>
    ///     Represents an assertion helper for the <see cref="Result" /> type, providing methods to verify
    ///     whether the result instance represents a success or failure state.
    /// </summary>
    public ResultAssertion(Result result)
    {
        _result = result;
    }

    /// <summary>
    ///     Asserts that the result instance represents a successful outcome.
    ///     Throws an assertion failure if the result indicates a failure.
    /// </summary>
    /// <returns>
    ///     A <see cref="SuccessAssertion" /> that represents the success state of the result
    /// </returns>
    public SuccessAssertion Success()
    {
        if (!_result.IsSuccess)
        {
            Assert.Fail("Expected success result but was failure.");
        }

        return new SuccessAssertion();
    }

    /// <summary>
    ///     Provides assertion methods for verifying the failure state of a <see cref="Result" /> instance
    ///     in the context of unit tests. This enables detailed validation of errors associated
    ///     with a failed result.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="FailureAssertion" /> for chaining further assertions on the
    ///     error associated with the failure.
    /// </returns>
    public FailureAssertion Failure()
    {
        if (!_result.TryGetError(out var error))
        {
            Assert.Fail("Expected failure result but was success.");
        }

        return new FailureAssertion(error);
    }
}

/// <summary>
///     Represents assertions for a <see cref="Result{TValue}" /> instance in a unit test.
///     This struct allows convenient verification of the success or failure state
///     of the result and provides tools to assert the correctness of the associated value or error.
/// </summary>
/// <typeparam name="TValue">
///     The type of the value associated with a successful result.
///     Must be a non-nullable reference or value type.
/// </typeparam>
[DebuggerStepThrough]
public readonly struct ResultAssertion<TValue> where TValue : notnull
{
    private readonly Result<TValue> _result;

    /// <summary>
    ///     Provides assertion methods for a <see cref="Result{TValue}" /> instance, enabling
    ///     verification of successful or failed outcomes in unit tests.
    /// </summary>
    public ResultAssertion(Result<TValue> result)
    {
        _result = result;
    }

    /// <summary>
    ///     Asserts that the result is successful and provides a fluent assertion object
    ///     for further verification of the encapsulated value.
    /// </summary>
    /// <returns>
    ///     A <see cref="SuccessAssertion{TValue}" /> instance that allows verification of
    ///     the non-nullable value associated with a successful result.
    /// </returns>
    /// <exception cref="Xunit.Sdk.XunitException">
    ///     Thrown if the result is not successful.
    /// </exception>
    public SuccessAssertion<TValue> Success()
    {
        if (!_result.TryGetValue(out var value))
        {
            Assert.Fail("Expected success result but was failure.");
        }

        return new SuccessAssertion<TValue>(value);
    }

    /// <summary>
    ///     Asserts that the current <see cref="Result{TValue}" /> instance represents a failure state.
    ///     If the result is not in a failure state, the method will fail the test.
    /// </summary>
    /// <returns>A <see cref="FailureAssertion" /> instance allowing further inspection or validation of the associated error.</returns>
    public FailureAssertion Failure()
    {
        if (_result.TryGet(out _, out var error))
        {
            Assert.Fail("Expected failure result but was success.");
        }

        return new FailureAssertion(error);
    }
}