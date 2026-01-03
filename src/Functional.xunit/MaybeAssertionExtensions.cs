using Xunit;

namespace UnambitiousFx.Functional.xunit;

/// <summary>
///     Provides extension methods for performing assertions on instances of <see cref="Maybe{TValue}" />
///     and related types, simplifying the process of validating functional constructs.
/// </summary>
public static class MaybeAssertionExtensions
{
    /// <summary>
    ///     Performs an assertion on the specified <see cref="Maybe{TValue}" /> and returns a
    ///     <see cref="MaybeAssertion{TValue}" />
    ///     instance for further validation.
    /// </summary>
    /// <param name="maybe">The instance of <see cref="Maybe{TValue}" /> to be asserted.</param>
    /// <typeparam name="TValue">
    ///     The type of the value encapsulated by the <see cref="Maybe{TValue}" />, which must be
    ///     non-nullable.
    /// </typeparam>
    /// <returns>A <see cref="MaybeAssertion{TValue}" /> instance for the given <see cref="Maybe{TValue}" />.</returns>
    public static MaybeAssertion<TValue> ShouldBe<TValue>(this Maybe<TValue> maybe) where TValue : notnull =>
        new(maybe);

    /// <summary>
    ///     Applies a predicate to the value inside a SomeAssertion, failing if the predicate is not satisfied.
    /// </summary>
    /// <typeparam name="T">The option value type.</typeparam>
    /// <param name="assertion">The Some assertion to evaluate.</param>
    /// <param name="predicate">The predicate to test the value against.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SomeAssertion instance.</returns>
    public static SomeAssertion<T> Where<T>(this SomeAssertion<T> assertion,
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
}
