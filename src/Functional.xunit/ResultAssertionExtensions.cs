using UnambitiousFx.Functional.Failures;
using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Provides extension methods for making assertions on <see cref="Result" /> and <see cref="Result{TValue}" />
///     instances
///     in the context of unit testing.
/// </summary>
public static class ResultAssertionExtensions
{
    /// <summary>
    ///     Asserts the specified <see cref="Result" /> and returns a <see cref="ResultAssertion" /> instance for further
    ///     validation.
    /// </summary>
    /// <param name="result">The result to be asserted.</param>
    /// <returns>A <see cref="ResultAssertion" /> instance representing the assertion of the provided result.</returns>
    public static ResultAssertion ShouldBe(this Result result)
    {
        return new ResultAssertion(result);
    }

    /// <summary>
    ///     Asserts the state of the specified <see cref="Result{TValue}" /> and returns a
    ///     <see cref="ResultAssertion{TValue}" /> for its value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to assert.</param>
    /// <returns>A <see cref="ResultAssertion{TValue}" /> for the contained value.</returns>
    public static ResultAssertion<TValue> ShouldBe<TValue>(this Result<TValue> result)
        where TValue : notnull
    {
        return new ResultAssertion<TValue>(result);
    }

    /// <summary>
    ///     Applies a predicate to the success value, failing the assertion if the predicate is not satisfied.
    /// </summary>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <param name="assertion">The success assertion to evaluate.</param>
    /// <param name="predicate">The predicate to test the value against.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SuccessAssertion instance.</returns>
    public static SuccessAssertion<T> Where<T>(this SuccessAssertion<T> assertion,
                                               Func<T, bool>            predicate,
                                               string?                  because = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(assertion.Value)) {
            Assert.Fail(because ?? $"Value '{assertion.Value}' does not satisfy predicate.");
        }

        return assertion;
    }

    /// <summary>
    ///     Applies a predicate to the error of a failure assertion, failing if the predicate is not satisfied.
    /// </summary>
    /// <param name="assertion">The failure assertion to evaluate.</param>
    /// <param name="predicate">The predicate to test the errors against.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same FailureAssertion instance.</returns>
    public static FailureAssertion Where(this FailureAssertion assertion,
                                         Func<IFailure, bool>  predicate,
                                         string?               because = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(assertion.Failure)) {
            var errorMessage = assertion.Failure.Message;
            Assert.Fail(because ?? $"Errors do not satisfy predicate. Error: '{errorMessage}'");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the success value is equal to the expected value.
    /// </summary>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <param name="assertion">The success assertion to evaluate.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SuccessAssertion instance.</returns>
    public static SuccessAssertion<T> BeEquivalentTo<T>(this SuccessAssertion<T> assertion,
                                                        T                         expected,
                                                        string?                   because = null)
        where T : notnull
    {
        if (!EqualityComparer<T>.Default.Equals(assertion.Value, expected)) {
            var message = because != null
                ? $"Expected value to be equivalent to '{expected}' because {because}, but found '{assertion.Value}'."
                : $"Expected value to be equivalent to '{expected}', but found '{assertion.Value}'.";
            Assert.Fail(message);
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the success value is not null.
    /// </summary>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <param name="assertion">The success assertion to evaluate.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SuccessAssertion instance.</returns>
    public static SuccessAssertion<T> NotBeNull<T>(this SuccessAssertion<T> assertion,
                                                   string?                   because = null)
        where T : notnull
    {
        if (assertion.Value is null) {
            Assert.Fail(because ?? "Expected value not to be null.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the success value is of the specified type.
    /// </summary>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <typeparam name="TExpected">The expected type.</typeparam>
    /// <param name="assertion">The success assertion to evaluate.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SuccessAssertion instance.</returns>
    public static SuccessAssertion<T> BeOfType<T, TExpected>(this SuccessAssertion<T> assertion,
                                                             string?                   because = null)
        where T : notnull
    {
        if (assertion.Value is not TExpected) {
            var message = because != null
                ? $"Expected value to be of type '{typeof(TExpected).Name}' because {because}, but was '{assertion.Value.GetType().Name}'."
                : $"Expected value to be of type '{typeof(TExpected).Name}', but was '{assertion.Value.GetType().Name}'.";
            Assert.Fail(message);
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the error message contains the specified substring.
    /// </summary>
    /// <param name="assertion">The failure assertion to evaluate.</param>
    /// <param name="substring">The substring that should be contained in the error message.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same FailureAssertion instance.</returns>
    public static FailureAssertion ContainMessage(this FailureAssertion assertion,
                                                  string                substring,
                                                  string?               because = null)
    {
        ArgumentNullException.ThrowIfNull(substring);
        if (!assertion.Failure.Message.Contains(substring, StringComparison.Ordinal)) {
            var message = because != null
                ? $"Expected error message to contain '{substring}' because {because}, but was '{assertion.Failure.Message}'."
                : $"Expected error message to contain '{substring}', but was '{assertion.Failure.Message}'.";
            Assert.Fail(message);
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the error message starts with the specified prefix.
    /// </summary>
    /// <param name="assertion">The failure assertion to evaluate.</param>
    /// <param name="prefix">The prefix that the error message should start with.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same FailureAssertion instance.</returns>
    public static FailureAssertion StartWithMessage(this FailureAssertion assertion,
                                                    string                prefix,
                                                    string?               because = null)
    {
        ArgumentNullException.ThrowIfNull(prefix);
        if (!assertion.Failure.Message.StartsWith(prefix, StringComparison.Ordinal)) {
            var message = because != null
                ? $"Expected error message to start with '{prefix}' because {because}, but was '{assertion.Failure.Message}'."
                : $"Expected error message to start with '{prefix}', but was '{assertion.Failure.Message}'.";
            Assert.Fail(message);
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the error code contains the specified substring.
    /// </summary>
    /// <param name="assertion">The failure assertion to evaluate.</param>
    /// <param name="substring">The substring that should be contained in the error code.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same FailureAssertion instance.</returns>
    public static FailureAssertion ContainCode(this FailureAssertion assertion,
                                               string                substring,
                                               string?               because = null)
    {
        ArgumentNullException.ThrowIfNull(substring);
        if (!assertion.Failure.Code.Contains(substring, StringComparison.Ordinal)) {
            var message = because != null
                ? $"Expected error code to contain '{substring}' because {because}, but was '{assertion.Failure.Code}'."
                : $"Expected error code to contain '{substring}', but was '{assertion.Failure.Code}'.";
            Assert.Fail(message);
        }

        return assertion;
    }

    /// <summary>
    ///     Applies multiple assertions to the success value, ensuring all conditions are satisfied.
    /// </summary>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <param name="assertion">The success assertion to evaluate.</param>
    /// <param name="assertions">Multiple assertion actions to apply.</param>
    /// <returns>The same SuccessAssertion instance.</returns>
    public static SuccessAssertion<T> SatisfyAll<T>(this SuccessAssertion<T> assertion,
                                                    params Action<T>[]        assertions)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(assertions);
        foreach (var assert in assertions) {
            assert(assertion.Value);
        }

        return assertion;
    }
}
