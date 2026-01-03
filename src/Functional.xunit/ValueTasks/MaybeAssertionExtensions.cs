namespace UnambitiousFx.Functional.xunit.ValueTasks;

/// <summary>
///     Provides extension methods for asserting instances of <see cref="Maybe{TValue}" /> types
///     within asynchronous contexts using tasks.
/// </summary>
public static class MaybeAssertionExtensions
{
    /// <summary>
    ///     Asserts the specified <see cref="Maybe{TValue}" /> instance and returns a <see cref="MaybeAssertion{TValue}" />
    ///     for further validation.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of the value held by the <see cref="Maybe{TValue}" /> instance. This type must be non-nullable.
    /// </typeparam>
    /// <param name="awaitableMaybe">
    ///     The awaited <see cref="Task{TResult}" /> result containing a <see cref="Maybe{TValue}" />
    ///     instance to be asserted.
    /// </param>
    /// <returns>
    ///     A <see cref="MaybeAssertion{TValue}" /> instance representing the assertion of the provided
    ///     <see cref="Maybe{TValue}" /> result.
    /// </returns>
    public static async ValueTask<MaybeAssertion<TValue>> ShouldBe<TValue>(this ValueTask<Maybe<TValue>> awaitableMaybe)
        where TValue : notnull
    {
        var maybe = await awaitableMaybe;
        return new MaybeAssertion<TValue>(maybe);
    }

    /// <summary>
    ///     Asserts that the provided <see cref="MaybeAssertion{TValue}" /> instance contains a
    ///     <see cref="SomeAssertion{T}" /> value, enabling further assertions or transformations.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of the value contained within the <see cref="Maybe{TValue}" />. This type must be non-nullable.
    /// </typeparam>
    /// <param name="awaitableAssertion">
    ///     An asynchronous task that, when awaited, yields a <see cref="MaybeAssertion{TValue}" /> instance to be asserted.
    /// </param>
    /// <returns>
    ///     A <see cref="SomeAssertion{TValue}" /> instance wrapping the value contained within the
    ///     <see cref="Some{TValue}" /> result of the original <see cref="MaybeAssertion{TValue}" />.
    /// </returns>
    public static async ValueTask<SomeAssertion<TValue>> Some<TValue>(
        this ValueTask<MaybeAssertion<TValue>> awaitableAssertion)
        where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Some();
    }

    /// <summary>
    ///     Ensures that the provided <see cref="MaybeAssertion{TValue}" /> instance represents a "none"
    ///     state within an asynchronous context and returns a <see cref="NoneAssertion" /> for further
    ///     validation or chaining.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of the value the <see cref="Maybe{TValue}" /> instance may hold. This type must be non-nullable.
    /// </typeparam>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{TResult}" /> containing a <see cref="MaybeAssertion{TValue}" />
    ///     instance to be verified as "none".
    /// </param>
    /// <returns>
    ///     A <see cref="NoneAssertion" /> instance representing the assertion of the "none" state
    ///     of the provided <see cref="MaybeAssertion{TValue}" />.
    /// </returns>
    public static async ValueTask<NoneAssertion> None<TValue>(this ValueTask<MaybeAssertion<TValue>> awaitableAssertion)
        where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.None();
    }

    /// <summary>
    ///     Allows adding an additional assertion to the current <see cref="SomeAssertion{TValue}" /> chain
    ///     within an asynchronous context.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of the value contained in the <see cref="SomeAssertion{TValue}" /> instance. This type must be
    ///     non-nullable.
    /// </typeparam>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="Task{TResult}" /> containing a <see cref="SomeAssertion{TValue}" /> instance
    ///     to be further asserted.
    /// </param>
    /// <param name="assert">
    ///     An action representing the assertion to be applied to the value contained in the
    ///     <see cref="SomeAssertion{TValue}" />.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> containing the updated <see cref="SomeAssertion{TValue}" />
    ///     post-assertion, allowing for further chaining or transformations.
    /// </returns>
    public static async ValueTask<SomeAssertion<TValue>> And<TValue>(
        this ValueTask<SomeAssertion<TValue>> awaitableAssertion,
        Action<TValue> assert) where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.And(assert);
    }

    /// <summary>
    ///     Transforms the underlying value of a <see cref="SomeAssertion{TValue}" /> within an asynchronous context
    ///     and returns a new <see cref="SomeAssertion{TOut}" /> with the transformed value.
    /// </summary>
    /// <typeparam name="TValue">
    ///     The type of the value contained in the original <see cref="SomeAssertion{TValue}" /> instance.
    ///     This type must be non-nullable.
    /// </typeparam>
    /// <typeparam name="TOut">
    ///     The type of the transformed value to be contained in the resulting <see cref="SomeAssertion{TOut}" /> instance.
    ///     This type must also be non-nullable.
    /// </typeparam>
    /// <param name="awaitableAssertion">
    ///     An awaited <see cref="Task{TResult}" /> containing an instance of
    ///     <see cref="SomeAssertion{TValue}" /> to be transformed.
    /// </param>
    /// <param name="map">
    ///     A transformation function that takes the value of type <typeparamref name="TValue" />
    ///     and returns a corresponding value of type <typeparamref name="TOut" />.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> containing a <see cref="SomeAssertion{TOut}" /> instance
    ///     with the transformed value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="map" /> function is null.
    /// </exception>
    public static async ValueTask<SomeAssertion<TOut>> Map<TValue, TOut>(
        this ValueTask<SomeAssertion<TValue>> awaitableAssertion,
        Func<TValue, TOut> map) where TValue : notnull
        where TOut : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Map(map);
    }
}
