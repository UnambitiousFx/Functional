namespace UnambitiousFx.Functional.xunit.Tasks;

/// <summary>
///     Provides extension methods for performing fluent assertions on <see cref="Result" /> and <see cref="Task{T}" />
///     instances.
///     This class is part of the <c>UnambitiousFx.Functional.XUnit.Tasks</c> namespace and facilitates
///     seamless testing of asynchronous and synchronous results within unit tests.
/// </summary>
public static class OneOfAssertionExtensions
{
    /// <summary>
    ///     Asserts the specified <see cref="OneOf{TFirst, TSecond}" /> and returns a
    ///     <see cref="OneOfAssertion{TFirst, TSecond}" />
    ///     instance for further validation.
    /// </summary>
    /// <param name="awaitableOneOf">
    ///     The awaited <see cref="Task{T}" /> containing a <see cref="OneOf{TFirst, TSecond}" />
    ///     instance to be asserted.
    /// </param>
    /// <typeparam name="TFirst">
    ///     The type of the first possible value contained in the <see cref="OneOf{TFirst, TSecond}" />
    ///     instance.
    /// </typeparam>
    /// <typeparam name="TSecond">
    ///     The type of the second possible value contained in the <see cref="OneOf{TFirst, TSecond}" />
    ///     instance.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="OneOfAssertion{TFirst, TSecond}" /> instance representing the assertion of the provided
    ///     <see cref="OneOf{TFirst, TSecond}" />.
    /// </returns>
    public static async Task<OneOfAssertion<TFirst, TSecond>> ShouldBe<TFirst, TSecond>(
        this Task<OneOf<TFirst, TSecond>> awaitableOneOf)
        where TFirst : notnull
        where TSecond : notnull
    {
        var oneOf = await awaitableOneOf;
        return new OneOfAssertion<TFirst, TSecond>(oneOf);
    }

    /// <summary>
    ///     Extracts the first value from the provided <see cref="OneOfAssertion{TFirst, TSecond}" />
    ///     instance and returns a <see cref="OneOfAssertion{TFirst}" /> for further fluent assertions.
    /// </summary>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{T}" /> containing a <see cref="OneOfAssertion{TFirst, TSecond}" />
    ///     instance from which the first value will be extracted.
    /// </param>
    /// <typeparam name="TFirst">
    ///     The type of the first possible value contained in the <see cref="OneOf{TFirst, TSecond}" />
    ///     instance.
    /// </typeparam>
    /// <typeparam name="TSecond">
    ///     The type of the second possible value contained in the <see cref="OneOf{TFirst, TSecond}" />
    ///     instance.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="OneOfAssertion{TFirst}" /> instance representing the fluent assertion
    ///     of the first value.
    /// </returns>
    public static async Task<OneOfAssertion<TFirst>> First<TFirst, TSecond>(
        this Task<OneOfAssertion<TFirst, TSecond>> awaitableAssertion)
        where TFirst : notnull
        where TSecond : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.First();
    }

    /// <summary>
    ///     Extracts and asserts the second value from the specified <see cref="OneOfAssertion{TFirst, TSecond}" />
    ///     instance and returns a <see cref="OneOfAssertion{TSecond}" /> for further validation.
    /// </summary>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{T}" /> containing a <see cref="OneOfAssertion{TFirst, TSecond}" />
    ///     instance whose second value is to be asserted.
    /// </param>
    /// <typeparam name="TFirst">
    ///     The type of the first possible value of the <see cref="OneOfAssertion{TFirst, TSecond}" />.
    /// </typeparam>
    /// <typeparam name="TSecond">
    ///     The type of the second possible value of the <see cref="OneOfAssertion{TFirst, TSecond}" />.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="OneOfAssertion{TSecond}" /> instance that allows further validation of the second value
    ///     within the asserted <see cref="OneOfAssertion{TFirst, TSecond}" />.
    /// </returns>
    public static async Task<OneOfAssertion<TSecond>> Second<TFirst, TSecond>(
        this Task<OneOfAssertion<TFirst, TSecond>> awaitableAssertion)
        where TFirst : notnull
        where TSecond : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Second();
    }

    /// <summary>
    ///     Asserts the specified <see cref="OneOfAssertion{TValue}" /> and applies an additional
    ///     assertion action on its underlying value.
    /// </summary>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{T}" /> containing a <see cref="OneOfAssertion{TValue}" />
    ///     instance to be further validated.
    /// </param>
    /// <param name="assert">
    ///     The assertion action to be applied to the value contained within the
    ///     <see cref="OneOfAssertion{TValue}" />.
    /// </param>
    /// <typeparam name="TValue">
    ///     The type of the value contained in the <see cref="OneOfAssertion{TValue}" />. Must be non-nullable.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="OneOfAssertion{TValue}" /> instance representing the result of the provided
    ///     assertion action for further validation.
    /// </returns>
    public static async Task<OneOfAssertion<TValue>> And<TValue>(this Task<OneOfAssertion<TValue>> awaitableAssertion,
        Action<TValue> assert) where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.And(assert);
    }

    /// <summary>
    ///     Transforms the value contained within the awaited <see cref="OneOfAssertion{TValue}" />
    ///     using the specified projection function and returns a new <see cref="OneOfAssertion{TOut}" />.
    /// </summary>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{T}" /> containing a <see cref="OneOfAssertion{TValue}" />
    ///     instance to be transformed.
    /// </param>
    /// <param name="projector">
    ///     A function that transforms the value contained within the <see cref="OneOfAssertion{TValue}" />
    ///     into a new value of type <typeparamref name="TOut" />.
    /// </param>
    /// <typeparam name="TValue">
    ///     The type of the value contained in the <see cref="OneOfAssertion{TValue}" /> instance.
    ///     Must be non-nullable.
    /// </typeparam>
    /// <typeparam name="TOut">
    ///     The type of the value to be returned by the projection function.
    ///     Must be non-nullable.
    /// </typeparam>
    /// <returns>
    ///     A new <see cref="OneOfAssertion{TOut}" /> instance containing the transformed value.
    /// </returns>
    public static async Task<OneOfAssertion<TOut>> Map<TValue, TOut>(
        this Task<OneOfAssertion<TValue>> awaitableAssertion,
        Func<TValue, TOut> projector)
        where TValue : notnull
        where TOut : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Map(projector);
    }

    /// <summary>
    ///     Filters the result of a <see cref="Task{T}" /> containing a <see cref="OneOfAssertion{TValue}" />
    ///     instance by applying a specified predicate.
    /// </summary>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{T}" /> that produces a <see cref="OneOfAssertion{TValue}" />
    ///     instance to be filtered.
    /// </param>
    /// <param name="predicate">
    ///     A function to evaluate each element of the <see cref="OneOfAssertion{TValue}" />. The predicate
    ///     determines whether the assertion passes.
    /// </param>
    /// <param name="because">
    ///     An optional explanation indicating why the assertion must pass if the predicate is satisfied.
    /// </param>
    /// <typeparam name="TValue">
    ///     The type of the value contained within the <see cref="OneOfAssertion{TValue}" /> instance.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="Task{T}" /> containing the filtered <see cref="OneOfAssertion{TValue}" /> instance
    ///     that satisfies the specified predicate and ensures its consistency.
    /// </returns>
    public static async Task<OneOfAssertion<TValue>> Where<TValue>(
        this Task<OneOfAssertion<TValue>> awaitableAssertion,
        Func<TValue, bool> predicate,
        string because = "")
        where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Where(predicate, because);
    }
}
