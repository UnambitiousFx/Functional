using System.Diagnostics;
using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent assertion for BadRequestFailure, enabling chaining of custom assertions
///     on bad-request-specific properties.
/// </summary>
[DebuggerStepThrough]
public readonly struct BadRequestFailureAssertion
{
    /// <summary>
    ///     Represents a fluent assertion mechanism for handling BadRequestFailure cases in test results.
    /// </summary>
    internal BadRequestFailureAssertion(BadRequestFailure failure)
    {
        Failure = failure;
    }

    /// <summary>
    ///     Gets the BadRequestFailure associated with this assertion.
    /// </summary>
    public BadRequestFailure Failure { get; }

    /// <summary>
    ///     Applies the specified assertion action to the BadRequestFailure and returns the current assertion
    ///     instance to allow method chaining.
    /// </summary>
    /// <param name="assert">The action to be applied to the BadRequestFailure.</param>
    /// <returns>The current <see cref="BadRequestFailureAssertion" /> instance to allow method chaining.</returns>
    public BadRequestFailureAssertion And(Action<BadRequestFailure> assert)
    {
        assert(Failure);
        return this;
    }

    /// <summary>
    ///     Asserts that the BadRequestFailure message matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error message to assert against.</param>
    /// <returns>The current instance of <see cref="BadRequestFailureAssertion" /> for further chaining.</returns>
    public BadRequestFailureAssertion AndMessage(string expected)
    {
        Assert.Equal(expected, Failure.Message);
        return this;
    }

    /// <summary>
    ///     Asserts that the code of the BadRequestFailure matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error code to assert against.</param>
    /// <returns>The current instance of <see cref="BadRequestFailureAssertion" /> for further chaining.</returns>
    public BadRequestFailureAssertion AndCode(string expected)
    {
        Assert.Equal(expected, Failure.Code);
        return this;
    }
}
