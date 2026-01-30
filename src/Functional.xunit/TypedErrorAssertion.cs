using System.Diagnostics;
using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a generic fluent assertion for strongly-typed error failures,
///     enabling chaining of custom assertions on any error type.
/// </summary>
/// <typeparam name="TError">The specific error type that implements IError.</typeparam>
[DebuggerStepThrough]
public readonly struct TypedErrorAssertion<TError>
    where TError : IFailure
{
    /// <summary>
    ///     Represents a fluent assertion mechanism for handling strongly-typed error cases in test results.
    /// </summary>
    internal TypedErrorAssertion(TError error)
    {
        Error = error;
    }

    /// <summary>
    ///     Gets the strongly-typed error associated with this assertion.
    /// </summary>
    public TError Error { get; }

    /// <summary>
    ///     Applies the specified assertion action to the strongly-typed error and returns the current assertion
    ///     instance to allow method chaining.
    /// </summary>
    /// <param name="assert">The action to be applied to the strongly-typed error.</param>
    /// <returns>The current <see cref="TypedErrorAssertion{TError}" /> instance to allow method chaining.</returns>
    public TypedErrorAssertion<TError> And(Action<TError> assert)
    {
        assert(Error);
        return this;
    }

    /// <summary>
    ///     Applies a predicate to the strongly-typed error, failing the assertion if the predicate is not satisfied.
    /// </summary>
    /// <param name="predicate">The predicate to test the error against.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The current <see cref="TypedErrorAssertion{TError}" /> instance to allow method chaining.</returns>
    public TypedErrorAssertion<TError> Where(Func<TError, bool> predicate, string? because = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(Error))
        {
            var errorMessage = Error.Message;
            Assert.Fail(because ?? $"Error does not satisfy predicate. Error: '{errorMessage}'");
        }

        return this;
    }

    /// <summary>
    ///     Asserts that the error message matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error message to assert against.</param>
    /// <returns>The current instance of <see cref="TypedErrorAssertion{TError}" /> for further chaining.</returns>
    public TypedErrorAssertion<TError> AndMessage(string expected)
    {
        Assert.Equal(expected, Error.Message);
        return this;
    }

    /// <summary>
    ///     Asserts that the code of the error matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error code to assert against.</param>
    /// <returns>The current instance of <see cref="TypedErrorAssertion{TError}" /> for further chaining.</returns>
    public TypedErrorAssertion<TError> AndCode(string expected)
    {
        Assert.Equal(expected, Error.Code);
        return this;
    }
}
