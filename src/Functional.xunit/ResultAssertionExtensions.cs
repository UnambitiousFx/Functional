using UnambitiousFx.Functional.Errors;
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
    public static ResultAssertion ShouldBe(this Result result) => new(result);

    /// <summary>
    ///     Asserts the state of the specified <see cref="Result{TValue}" /> and returns a
    ///     <see cref="ResultAssertion{TValue}" /> for its value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to assert.</param>
    /// <returns>A <see cref="ResultAssertion{TValue}" /> for the contained value.</returns>
    public static ResultAssertion<TValue> ShouldBe<TValue>(this Result<TValue> result) where TValue : notnull =>
        new(result);

    /// <summary>
    ///     Applies a predicate to the success value, failing the assertion if the predicate is not satisfied.
    /// </summary>
    /// <typeparam name="T">The success value type.</typeparam>
    /// <param name="assertion">The success assertion to evaluate.</param>
    /// <param name="predicate">The predicate to test the value against.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SuccessAssertion instance.</returns>
    public static SuccessAssertion<T> Where<T>(this SuccessAssertion<T> assertion,
        Func<T, bool> predicate,
        string? because = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(assertion.Value))
        {
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
        Func<IError, bool> predicate,
        string? because = null)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(assertion.Error))
        {
            var errorMessage = assertion.Error.Message;
            Assert.Fail(because ?? $"Errors do not satisfy predicate. Error: '{errorMessage}'");
        }

        return assertion;
    }
}
