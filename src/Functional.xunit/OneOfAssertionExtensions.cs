using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Provides extension methods for fluent assertion of <see cref="OneOf{TFirst,TSecond}" /> instances.
/// </summary>
public static class OneOfAssertionExtensions
{
    /// <summary>
    ///     Asserts the specified <see cref="OneOf{TFirst, TSecond}" /> and returns a
    ///     <see cref="OneOfAssertion{TFirst, TSecond}" /> instance for further validation.
    /// </summary>
    /// <param name="oneOf">The instance of <see cref="OneOf{TFirst, TSecond}" /> to be asserted.</param>
    /// <returns>A <see cref="OneOfAssertion{TFirst, TSecond}" /> instance representing the assertion of the provided value.</returns>
    /// <typeparam name="TFirst">The type of the first value the <see cref="OneOf{TFirst, TSecond}" /> can represent.</typeparam>
    /// <typeparam name="TSecond">The type of the second value the <see cref="OneOf{TFirst, TSecond}" /> can represent.</typeparam>
    public static OneOfAssertion<TFirst, TSecond> ShouldBe<TFirst, TSecond>(this OneOf<TFirst, TSecond> oneOf)
        where TFirst : notnull
        where TSecond : notnull =>
        new(oneOf);

    /// <summary>
    ///     Filters the value of the given <see cref="OneOfAssertion{TValue}" /> based on a predicate.
    /// </summary>
    /// <param name="assertion">The <see cref="OneOfAssertion{TValue}" /> to apply the filter to.</param>
    /// <param name="predicate">The predicate to evaluate the value against.</param>
    /// <param name="because">An optional reason why the assertion is performed.</param>
    /// <returns>The same instance of <see cref="OneOfAssertion{TValue}" /> if the predicate passes.</returns>
    /// <typeparam name="TValue">The type of the value. Must be non-nullable.</typeparam>
    public static OneOfAssertion<TValue> Where<TValue>(
        this OneOfAssertion<TValue> assertion,
        Func<TValue, bool> predicate,
        string? because = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(assertion.Value))
        {
            Assert.Fail(because ?? $"Value '{assertion.Value}' does not satisfy predicate.");
        }

        return assertion;
    }
}
