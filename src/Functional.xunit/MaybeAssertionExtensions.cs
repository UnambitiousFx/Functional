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
    public static MaybeAssertion<TValue> ShouldBe<TValue>(this Maybe<TValue> maybe)
        where TValue : notnull
    {
        return new MaybeAssertion<TValue>(maybe);
    }

    /// <summary>
    ///     Applies a predicate to the value inside a SomeAssertion, failing if the predicate is not satisfied.
    /// </summary>
    /// <typeparam name="T">The option value type.</typeparam>
    /// <param name="assertion">The Some assertion to evaluate.</param>
    /// <param name="predicate">The predicate to test the value against.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SomeAssertion instance.</returns>
    public static SomeAssertion<T> Where<T>(this SomeAssertion<T> assertion,
                                            Func<T, bool>         predicate,
                                            string?               because = null)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(predicate);
        if (!predicate(assertion.Value)) {
            Assert.Fail(because ?? $"Value '{assertion.Value}' does not satisfy predicate.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the Some value is equal to the expected value.
    /// </summary>
    /// <typeparam name="T">The option value type.</typeparam>
    /// <param name="assertion">The Some assertion to evaluate.</param>
    /// <param name="expected">The expected value.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SomeAssertion instance.</returns>
    public static SomeAssertion<T> BeEquivalentTo<T>(this SomeAssertion<T> assertion,
                                                     T                      expected,
                                                     string?                because = null)
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
    ///     Asserts that the Some value is not null.
    /// </summary>
    /// <typeparam name="T">The option value type.</typeparam>
    /// <param name="assertion">The Some assertion to evaluate.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SomeAssertion instance.</returns>
    public static SomeAssertion<T> NotBeNull<T>(this SomeAssertion<T> assertion,
                                                string?                because = null)
        where T : notnull
    {
        if (assertion.Value is null) {
            Assert.Fail(because ?? "Expected value not to be null.");
        }

        return assertion;
    }

    /// <summary>
    ///     Asserts that the Some value is of the specified type.
    /// </summary>
    /// <typeparam name="T">The option value type.</typeparam>
    /// <typeparam name="TExpected">The expected type.</typeparam>
    /// <param name="assertion">The Some assertion to evaluate.</param>
    /// <param name="because">An optional reason to include if the assertion fails.</param>
    /// <returns>The same SomeAssertion instance.</returns>
    public static SomeAssertion<T> BeOfType<T, TExpected>(this SomeAssertion<T> assertion,
                                                          string?                because = null)
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
    ///     Applies multiple assertions to the Some value, ensuring all conditions are satisfied.
    /// </summary>
    /// <typeparam name="T">The option value type.</typeparam>
    /// <param name="assertion">The Some assertion to evaluate.</param>
    /// <param name="assertions">Multiple assertion actions to apply.</param>
    /// <returns>The same SomeAssertion instance.</returns>
    public static SomeAssertion<T> SatisfyAll<T>(this SomeAssertion<T> assertion,
                                                 params Action<T>[]     assertions)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(assertions);
        foreach (var assert in assertions) {
            assert(assertion.Value);
        }

        return assertion;
    }
}
