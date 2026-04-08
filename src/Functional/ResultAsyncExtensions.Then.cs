namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Executes a specified function if the asynchronous Result is successful,
    ///     and returns the Result of that function, otherwise propagates the failure.
    /// </summary>
    /// <param name="resultTask">
    ///     The asynchronous Result operation to evaluate.
    /// </param>
    /// <param name="then">
    ///     The function to execute if the asynchronous Result is successful.
    /// </param>
    /// <returns>
    ///     A ValueTask representing the Result of the function, or the original failure.
    /// </returns>
    public static async ValueTask<Result> Then(this ValueTask<Result> resultTask,
                                               Func<Result>           then) {
        var result = await resultTask;
        return result.Then(then);
    }

    /// <summary>
    ///     Executes a continuation function if the initial <see cref="Result" /> operation is successful,
    ///     and returns the resulting <see cref="Result" /> from the continuation function.
    ///     If the initial operation fails, the failure result is returned directly without invoking the continuation.
    /// </summary>
    /// <param name="resultTask">An asynchronous operation returning a <see cref="Result" />.</param>
    /// <param name="then">
    ///     A function to execute if the initial operation is successful. This function returns
    ///     a new asynchronous <see cref="Result" /> to propagate further.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> that completes with the result of the continuation function if the
    ///     initial operation is successful; otherwise, it completes with the failure result of the original operation.
    /// </returns>
    public static async ValueTask<Result> Then(this ValueTask<Result>  resultTask,
                                               Func<ValueTask<Result>> then) {
        var result = await resultTask;
        if (result.IsFailure) {
            return result;
        }

        return await then();
    }

    /// <summary>
    ///     Chains an asynchronous operation to be executed after the completion of the initial asynchronous Result operation.
    /// </summary>
    /// <param name="resultTask">The initial asynchronous Result operation.</param>
    /// <param name="then">The function to execute upon successful completion of the initial operation.</param>
    /// <typeparam name="TValue">The type of the value held within the Result.</typeparam>
    /// <returns>
    ///     A ValueTask containing the result of the chained operation.
    /// </returns>
    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                               Func<TValue, Result<TValue>>   then)
        where TValue : notnull {
        var result = await resultTask;
        return result.Then(then);
    }

    /// <summary>
    ///     Executes a continuation function if the asynchronous Result operation completes successfully,
    ///     returning a new asynchronous Result. The continuation function returns another asynchronous Result.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in the Result.</typeparam>
    /// <param name="resultTask">The asynchronous Result operation to evaluate.</param>
    /// <param name="then">The function to invoke if the Result operation is successful.</param>
    /// <returns>
    ///     A task containing the Result from the continuation function if the initial Result was successful,
    ///     or the original Result if it was a failure.
    /// </returns>
    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>>          resultTask,
                                                               Func<TValue, ValueTask<Result<TValue>>> then)
        where TValue : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            return result;
        }

        var response = await then(value);
        return response.WithMetadata(result.Metadata);
    }

    /// <summary>
    ///     Chains the execution of the current <see cref="Result" /> with a subsequent operation.
    /// </summary>
    /// <param name="resultTask">
    ///     The current asynchronous <see cref="Result" /> to be processed.
    /// </param>
    /// <param name="then">
    ///     A function representing the next operation to execute if the current result is successful.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> representing the asynchronous result of the chained operation.
    /// </returns>
    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                               Func<TValue, Result>           then)
        where TValue : notnull {
        var result = await resultTask;
        return result.Then(then);
    }

    /// <summary>
    ///     Executes the specified function if the current <see cref="Result" /> operation completes successfully.
    /// </summary>
    /// <param name="resultTask">An asynchronous <see cref="ValueTask{Result}" /> representing the initial result operation.</param>
    /// <param name="then">
    ///     A function to invoke if the current result operation succeeds. This function returns a new
    ///     <see cref="Result" /> to be evaluated.
    /// </param>
    /// <returns>
    ///     An asynchronous <see cref="ValueTask{Result}" /> representing the result of the function invocation, or the
    ///     original failure if the initial operation fails.
    /// </returns>
    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>>  resultTask,
                                                               Func<TValue, ValueTask<Result>> then)
        where TValue : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            return result;
        }

        var thenResult = await then(value);
        if (!thenResult.TryGetFailure(out var error)) {
            return result;
        }

        var failResult = Result.Failure<TValue>(error);
        return failResult.WithMetadata(result.Metadata);
    }
}
