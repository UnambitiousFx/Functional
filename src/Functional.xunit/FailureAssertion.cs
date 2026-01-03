using System.Diagnostics;
using UnambitiousFx.Functional.Errors;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Represents a fluent assertion for failure cases, enabling chaining of custom assertions
///     or message validation on errors.
/// </summary>
[DebuggerStepThrough]
public readonly struct FailureAssertion
{
    /// <summary>
    ///     Represents a fluent assertion mechanism for handling failure cases in the context of test results.
    /// </summary>
    internal FailureAssertion(IError error)
    {
        Error = error;
    }

    /// <summary>
    ///     Gets the errors associated with the failure assertion.
    /// </summary>
    /// <remarks>
    ///     Represents the error information related to a failed operation,
    ///     allowing further assertions or evaluations on the errors.
    /// </remarks>
    public IError Error { get; }

    /// <summary>
    ///     Applies the specified assertion action to the encapsulated errors and returns the current failure assertion
    ///     instance to allow method chaining.
    /// </summary>
    /// <param name="assert">The action to be applied to the encapsulated errors.</param>
    /// <returns>The current <see cref="FailureAssertion" /> instance to allow method chaining.</returns>
    public FailureAssertion And(Action<IError> assert)
    {
        assert(Error);
        return this;
    }

    /// <summary>
    ///     Asserts that the message of the first error matches the expected value and continues the failure assertion chain.
    /// </summary>
    /// <param name="expected">The expected error message to assert against.</param>
    /// <returns>The current instance of <see cref="FailureAssertion" /> for further chaining.</returns>
    public FailureAssertion AndMessage(string expected)
    {
        Assert.Equal(expected, Error.Message);
        return this;
    }

    /// <summary>
    ///     Asserts that the code of the first error matches the expected value and continues the failure assertion chain.
    /// </summary>
    /// <param name="expected">The expected error code to assert against.</param>
    /// <returns>The current instance of <see cref="FailureAssertion" /> for further chaining.</returns>
    public FailureAssertion AndCode(string expected)
    {
        Assert.Equal(expected, Error.Code);
        return this;
    }
}
