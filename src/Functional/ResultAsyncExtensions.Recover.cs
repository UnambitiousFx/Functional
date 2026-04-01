using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Recovers a potentially failed asynchronous <see cref="Result{TValue}" /> by using the provided recovery function
    ///     to transform a <see cref="Failure" /> into a successful result value.
    /// </summary>
    /// <typeparam name="TIn">The type of the value encapsulated in the <see cref="Result{TValue}" />.</typeparam>
    /// <param name="resultTask">
    ///     The asynchronous <see cref="ValueTask{TResult}" /> representing the result that might have failed.
    /// </param>
    /// <param name="recover">
    ///     A function that takes a <see cref="Failure" /> and provides a fallback value of type <typeparamref name="TIn" />.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> that resolves to a <see cref="Result{TValue}" /> containing either a successful
    ///     value or the original failure.
    /// </returns>
    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                            Func<Failure, TIn>          recover)
        where TIn : notnull {
        var result = await resultTask;
        return result.Recover(recover);
    }

    /// <summary>
    ///     Recovers from a failure in an asynchronous result operation by invoking a provided asynchronous fallback function.
    /// </summary>
    /// <typeparam name="TIn">The type of the value contained in the result.</typeparam>
    /// <param name="resultTask">The asynchronous result task to recover from.</param>
    /// <param name="recover">
    ///     A function that takes a <see cref="Failure" /> and returns a <see cref="ValueTask{TIn}" />
    ///     representing the recovery logic.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TIn}" /> that represents the resulting successful value or the recovery result in case
    ///     of failure.
    /// </returns>
    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>>   resultTask,
                                                            Func<Failure, ValueTask<TIn>> recover)
        where TIn : notnull {
        var result = await resultTask;
        if (result.IsSuccess) {
            return result;
        }

        result.TryGetFailure(out var error);
        return Result.Success(await recover(error!));
    }

    /// <summary>
    ///     Recovers a failed <see cref="Result{TValue}" /> asynchronously using a fallback value.
    /// </summary>
    /// <typeparam name="TIn">The type of the value contained in the <see cref="Result{TValue}" />.</typeparam>
    /// <param name="resultTask">
    ///     A <see cref="ValueTask{TResult}" /> containing the original <see cref="Result{TValue}" /> to process.
    /// </param>
    /// <param name="fallback">
    ///     A default value of type <typeparamref name="TIn" /> to return in case the original <see cref="Result{TValue}" /> is
    ///     a failure.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> containing a successful <see cref="Result{TValue}" /> with the fallback
    ///     value in case of failure.
    /// </returns>
    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                            TIn                         fallback)
        where TIn : notnull {
        var result = await resultTask;
        return result.Recover(fallback);
    }
}
