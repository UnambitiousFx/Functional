using System.Diagnostics;
using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent assertion for NotFoundError failures, enabling chaining of custom assertions
///     on not-found-specific properties.
/// </summary>
[DebuggerStepThrough]
public readonly struct NotFoundErrorAssertion
{
    /// <summary>
    ///     Represents a fluent assertion mechanism for handling NotFoundError cases in test results.
    /// </summary>
    internal NotFoundErrorAssertion(NotFoundFailure failure)
    {
        Failure = failure;
    }

    /// <summary>
    ///     Gets the NotFoundError associated with this assertion.
    /// </summary>
    public NotFoundFailure Failure { get; }

    /// <summary>
    ///     Applies the specified assertion action to the NotFoundError and returns the current assertion
    ///     instance to allow method chaining.
    /// </summary>
    /// <param name="assert">The action to be applied to the NotFoundError.</param>
    /// <returns>The current <see cref="NotFoundErrorAssertion" /> instance to allow method chaining.</returns>
    public NotFoundErrorAssertion And(Action<NotFoundFailure> assert)
    {
        assert(Failure);
        return this;
    }

    /// <summary>
    ///     Asserts that the NotFoundError has the expected resource name.
    /// </summary>
    /// <param name="expectedResource">The expected resource name.</param>
    /// <returns>The current instance of <see cref="NotFoundErrorAssertion" /> for further chaining.</returns>
    public NotFoundErrorAssertion WithResource(string expectedResource)
    {
        Assert.Equal(expectedResource, Failure.Resource);
        return this;
    }

    /// <summary>
    ///     Asserts that the NotFoundError has the expected identifier.
    /// </summary>
    /// <param name="expectedIdentifier">The expected identifier.</param>
    /// <returns>The current instance of <see cref="NotFoundErrorAssertion" /> for further chaining.</returns>
    public NotFoundErrorAssertion WithIdentifier(string expectedIdentifier)
    {
        Assert.Equal(expectedIdentifier, Failure.Identifier);
        return this;
    }

    /// <summary>
    ///     Asserts that the NotFoundError message matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error message to assert against.</param>
    /// <returns>The current instance of <see cref="NotFoundErrorAssertion" /> for further chaining.</returns>
    public NotFoundErrorAssertion AndMessage(string expected)
    {
        Assert.Equal(expected, Failure.Message);
        return this;
    }

    /// <summary>
    ///     Asserts that the code of the NotFoundError matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error code to assert against.</param>
    /// <returns>The current instance of <see cref="NotFoundErrorAssertion" /> for further chaining.</returns>
    public NotFoundErrorAssertion AndCode(string expected)
    {
        Assert.Equal(expected, Failure.Code);
        return this;
    }
}
