using System.Diagnostics;
using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent assertion for ConflictError failures, enabling chaining of custom assertions
///     on conflict-specific properties.
/// </summary>
[DebuggerStepThrough]
public readonly struct ConflictErrorAssertion
{
    /// <summary>
    ///     Represents a fluent assertion mechanism for handling ConflictError cases in test results.
    /// </summary>
    internal ConflictErrorAssertion(ConflictFailure failure)
    {
        Failure = failure;
    }

    /// <summary>
    ///     Gets the ConflictError associated with this assertion.
    /// </summary>
    public ConflictFailure Failure { get; }

    /// <summary>
    ///     Applies the specified assertion action to the ConflictError and returns the current assertion
    ///     instance to allow method chaining.
    /// </summary>
    /// <param name="assert">The action to be applied to the ConflictError.</param>
    /// <returns>The current <see cref="ConflictErrorAssertion" /> instance to allow method chaining.</returns>
    public ConflictErrorAssertion And(Action<ConflictFailure> assert)
    {
        assert(Failure);
        return this;
    }

    /// <summary>
    ///     Asserts that the ConflictError message matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error message to assert against.</param>
    /// <returns>The current instance of <see cref="ConflictErrorAssertion" /> for further chaining.</returns>
    public ConflictErrorAssertion AndMessage(string expected)
    {
        Assert.Equal(expected, Failure.Message);
        return this;
    }

    /// <summary>
    ///     Asserts that the code of the ConflictError matches the expected value.
    /// </summary>
    /// <param name="expected">The expected error code to assert against.</param>
    /// <returns>The current instance of <see cref="ConflictErrorAssertion" /> for further chaining.</returns>
    public ConflictErrorAssertion AndCode(string expected)
    {
        Assert.Equal(expected, Failure.Code);
        return this;
    }
}
