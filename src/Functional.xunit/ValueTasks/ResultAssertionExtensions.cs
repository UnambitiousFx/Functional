namespace UnambitiousFx.Functional.xunit.ValueTasks;

/// <summary>
///     Provides extension methods for asserting instances of <see cref="Result" /> or <see cref="Result{TValue}" />
///     asynchronously.
/// </summary>
public static class ResultAssertionExtensions
{
    /// <summary>
    ///     Asserts the specified <see cref="Result" /> and returns a <see cref="ResultAssertion" /> instance for further
    ///     validation.
    /// </summary>
    /// <param name="awaitableResult">The awaited result to be asserted.</param>
    /// <returns>A <see cref="ResultAssertion" /> instance representing the assertion of the provided result.</returns>
    public static async ValueTask<ResultAssertion> ShouldBe(this ValueTask<Result> awaitableResult)
    {
        var result = await awaitableResult;
        return result.ShouldBe();
    }

    /// <summary>
    ///     Asserts the specified <see cref="Task{Result}" /> and returns a <see cref="ResultAssertion" /> instance for further
    ///     validation.
    /// </summary>
    /// <param name="awaitableResult">The awaited result to be asserted.</param>
    /// <returns>A <see cref="ResultAssertion" /> instance representing the assertion of the provided result.</returns>
    public static async ValueTask<ResultAssertion<TValue>> ShouldBe<TValue>(
        this ValueTask<Result<TValue>> awaitableResult)
        where TValue : notnull
    {
        var result = await awaitableResult;
        return result.ShouldBe();
    }

    /// <summary>
    ///     Asserts that the provided <see cref="ResultAssertion" /> instance represents a successful outcome,
    ///     enabling further validation of the success state.
    /// </summary>
    /// <param name="awaitableAssertion">The awaited assertion to be validated as successful.</param>
    /// <returns>A <see cref="SuccessAssertion" /> that represents the success state of the provided assertion.</returns>
    public static async ValueTask<SuccessAssertion> Success(this ValueTask<ResultAssertion> awaitableAssertion)
    {
        var assertion = await awaitableAssertion;
        return assertion.Success();
    }

    /// <summary>
    ///     Asserts that the provided <see cref="ResultAssertion{TValue}" /> represents a successful outcome and returns a
    ///     <see cref="SuccessAssertion{TValue}" /> instance for further validation of the encapsulated value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value within the successful result assertion.</typeparam>
    /// <param name="awaitableAssertion">The awaited result assertion to validate success against.</param>
    /// <returns>A <see cref="SuccessAssertion{TValue}" /> instance encapsulating the value of the successful result assertion.</returns>
    public static async ValueTask<SuccessAssertion<TValue>> Success<TValue>(
        this ValueTask<ResultAssertion<TValue>> awaitableAssertion)
        where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Success();
    }

    /// <summary>
    ///     Asserts that the provided <see cref="ResultAssertion" /> represents a failure and returns a
    ///     <see cref="FailureAssertion" /> instance for further validation of error details.
    /// </summary>
    /// <param name="awaitableAssertion">The <see cref="Task{TResult}" /> representing the awaited assertion.</param>
    /// <returns>A <see cref="FailureAssertion" /> instance for chaining additional failure-related validations.</returns>
    public static async ValueTask<FailureAssertion> Failure(this ValueTask<ResultAssertion> awaitableAssertion)
    {
        var assertion = await awaitableAssertion;
        return assertion.Failure();
    }

    /// <summary>
    ///     Asserts that the specified <see cref="ResultAssertion{TValue}" /> represents a failure state and returns
    ///     a <see cref="FailureAssertion" /> instance for further validation.
    /// </summary>
    /// <param name="awaitableAssertion">The awaited assertion to be validated as failure.</param>
    /// <typeparam name="TValue">The type of the result's value.</typeparam>
    /// <returns>A <see cref="FailureAssertion" /> instance representing the failure of the provided assertion.</returns>
    public static async ValueTask<FailureAssertion> Failure<TValue>(
        this ValueTask<ResultAssertion<TValue>> awaitableAssertion)
        where TValue : notnull
    {
        var assertion = await awaitableAssertion;
        return assertion.Failure();
    }

    /// <summary>
    ///     Chains an additional assertion on the successful result of the given assertion, applying the specified action
    ///     to the underlying value.
    /// </summary>
    /// <param name="awaitableAssertion">The awaited successful assertion to chain the additional assertion onto.</param>
    /// <param name="assertion">The action to be performed on the underlying value of the successful assertion.</param>
    /// <typeparam name="TValue">The type of the non-nullable value within the successful assertion.</typeparam>
    /// <returns>
    ///     A <see cref="SuccessAssertion{TValue}" /> instance representing the assertion after applying the additional
    ///     action.
    /// </returns>
    public static async ValueTask<SuccessAssertion<TValue>> And<TValue>(
        this ValueTask<SuccessAssertion<TValue>> awaitableAssertion,
        Action<TValue> assertion)
        where TValue : notnull
    {
        var assertionResult = await awaitableAssertion;
        return assertionResult.And(assertion);
    }

    /// <summary>
    ///     Transforms the value of the specified <see cref="SuccessAssertion{TValue}" /> asynchronously using the provided
    ///     mapping function and returns a <see cref="SuccessAssertion{TOut}" /> instance containing the projected value.
    /// </summary>
    /// <param name="awaitableAssertion">
    ///     The awaited <see cref="SuccessAssertion{TValue}" /> instance whose value is to be transformed.
    /// </param>
    /// <param name="map">
    ///     A function that projects the value of type <typeparamref name="TValue" /> into a new value of type
    ///     <typeparamref name="TOut" />.
    /// </param>
    /// <typeparam name="TValue">The type of the value contained within the original success assertion.</typeparam>
    /// <typeparam name="TOut">The type of the projected value returned by the mapping function.</typeparam>
    /// <returns>
    ///     A new <see cref="SuccessAssertion{TOut}" /> instance containing the value projected using the mapping function.
    /// </returns>
    public static async ValueTask<SuccessAssertion<TOut>> Map<TValue, TOut>(
        this ValueTask<SuccessAssertion<TValue>> awaitableAssertion,
        Func<TValue, TOut> map)
        where TValue : notnull where TOut : notnull
    {
        var assertionResult = await awaitableAssertion;
        return assertionResult.Map(map);
    }

    /// <summary>
    ///     Appends an assertion for the error message to the current <see cref="FailureAssertion" /> instance.
    /// </summary>
    /// <param name="awaitableAssertion">The awaited <see cref="FailureAssertion" /> to assert against.</param>
    /// <param name="expectedMessage">The expected error message to validate.</param>
    /// <returns>The updated <see cref="FailureAssertion" /> instance with the message assertion applied.</returns>
    public static async ValueTask<FailureAssertion> AndMessage(
        this ValueTask<FailureAssertion> awaitableAssertion,
        string expectedMessage)
    {
        var assertionResult = await awaitableAssertion;
        return assertionResult.AndMessage(expectedMessage);
    }
}
