using System.Diagnostics;
using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent assertion for ValidationError failures, enabling chaining of custom assertions
///     on validation-specific properties.
/// </summary>
[DebuggerStepThrough]
public readonly struct ValidationErrorAssertion
{
    /// <summary>
    ///     Represents a fluent assertion mechanism for handling ValidationError cases in test results.
    /// </summary>
    internal ValidationErrorAssertion(ValidationFailure failure)
    {
        Failure = failure;
    }

    /// <summary>
    ///     Gets the ValidationError associated with this assertion.
    /// </summary>
    public ValidationFailure Failure { get; }

    /// <summary>
    ///     Applies the specified assertion action to the ValidationError and returns the current assertion
    ///     instance to allow method chaining.
    /// </summary>
    /// <param name="assert">The action to be applied to the ValidationError.</param>
    /// <returns>The current <see cref="ValidationErrorAssertion" /> instance to allow method chaining.</returns>
    public ValidationErrorAssertion And(Action<ValidationFailure> assert)
    {
        assert(Failure);
        return this;
    }

    /// <summary>
    ///     Asserts that the ValidationError contains the exact failure message.
    /// </summary>
    /// <param name="expectedFailure">The expected failure message.</param>
    /// <returns>The current instance of <see cref="ValidationErrorAssertion" /> for further chaining.</returns>
    public ValidationErrorAssertion WithFailure(string expectedFailure)
    {
        Assert.Contains(expectedFailure, Failure.Failures);
        return this;
    }

    /// <summary>
    ///     Asserts that the ValidationError contains a failure message that contains the specified text.
    /// </summary>
    /// <param name="expectedText">The expected text within a failure message.</param>
    /// <returns>The current instance of <see cref="ValidationErrorAssertion" /> for further chaining.</returns>
    public ValidationErrorAssertion WithFailureContaining(string expectedText)
    {
        Assert.Contains(Failure.Failures, f => f.Contains(expectedText));
        return this;
    }

    /// <summary>
    ///     Asserts that the ValidationError has the expected number of failures.
    /// </summary>
    /// <param name="expectedCount">The expected count of failures.</param>
    /// <returns>The current instance of <see cref="ValidationErrorAssertion" /> for further chaining.</returns>
    public ValidationErrorAssertion WithFailureCount(int expectedCount)
    {
        Assert.Equal(expectedCount, Failure.Failures.Count);
        return this;
    }

    /// <summary>
    ///     Asserts that the ValidationError message matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error message to assert against.</param>
    /// <returns>The current instance of <see cref="ValidationErrorAssertion" /> for further chaining.</returns>
    public ValidationErrorAssertion AndMessage(string expected)
    {
        Assert.Equal(expected, Failure.Message);
        return this;
    }

    /// <summary>
    ///     Asserts that the code of the ValidationError matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error code to assert against.</param>
    /// <returns>The current instance of <see cref="ValidationErrorAssertion" /> for further chaining.</returns>
    public ValidationErrorAssertion AndCode(string expected)
    {
        Assert.Equal(expected, Failure.Code);
        return this;
    }
}
