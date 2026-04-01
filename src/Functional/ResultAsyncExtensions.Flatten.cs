namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Asynchronously flattens a nested <see cref="Result{TValue}" /> object
    ///     contained within a <see cref="ValueTask{TResult}" /> into a single <see cref="Result{TValue}" /> object.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value contained within the result.</typeparam>
    /// <param name="resultTask">
    ///     A <see cref="ValueTask{TResult}" /> that resolves to a <see cref="Result{TValue}" />,
    ///     where the success value itself is a nested <see cref="Result{TValue}" />.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> containing a single flattened <see cref="Result{TValue}" />
    ///     with either the final success value or a failure.
    /// </returns>
    public static async ValueTask<Result<TValue>> Flatten<TValue>(this ValueTask<Result<Result<TValue>>> resultTask)
        where TValue : notnull {
        var result = await resultTask;
        return result.Flatten();
    }

    /// <summary>
    ///     Flattens an asynchronous result of a nested result into a single asynchronous result.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="resultTask">A ValueTask containing a Result of a nested asynchronous Result.</param>
    /// <returns>A ValueTask representing the flattened Result containing the type TValue.</returns>
    public static async ValueTask<Result<TValue>> Flatten<TValue>(this ValueTask<Result<ValueTask<Result<TValue>>>> resultTask)
        where TValue : notnull {
        var result = await resultTask;
        if (!result.TryGetValue(out var innerTask)) {
            result.TryGetFailure(out var error);
            return Result.Failure<TValue>(error!);
        }

        return await innerTask;
    }
}
