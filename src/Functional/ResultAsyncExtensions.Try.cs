namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Executes a synchronous action on the result of a <see cref="ValueTask{Result}" />.
    ///     If the result is successful, the action is invoked; otherwise, the failure state is preserved.
    /// </summary>
    /// <param name="resultTask">The asynchronous Result wrapped in a ValueTask.</param>
    /// <param name="action">The action to be executed if the result is successful.</param>
    /// <return>
    ///     Returns a new Result preserving the original failure state if present,
    ///     or the result of executing the action if the original result is successful.
    /// </return>
    public static async ValueTask<Result> Try(this ValueTask<Result> resultTask,
                                              Action                 action) {
        var result = await resultTask;
        return result.Try(action);
    }

    /// <summary>
    ///     Executes the provided asynchronous action if the initial <see cref="Result" /> is successful.
    ///     If the action fails, the resulting failure is captured and returned as a <see cref="Result" />.
    /// </summary>
    /// <param name="resultTask">
    ///     A <see cref="ValueTask{Result}" /> representing the asynchronous operation that produces a result.
    /// </param>
    /// <param name="action">
    ///     A function representing the asynchronous action to execute if the initial result is successful.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> representing the result of the operation. If the initial result is successful
    ///     and the action completes successfully, the method returns a successful result. If the action fails,
    ///     the method returns a failure result containing the exception thrown by the action.
    /// </returns>
    public static async ValueTask<Result> Try(this ValueTask<Result> resultTask,
                                              Func<ValueTask>        action) {
        var result = await resultTask;
        return await result.Bind(async () => {
            try {
                await action();
                return Result.Success();
            }
            catch (Exception ex) {
                return Result.Failure(ex);
            }
        });
    }

    /// <summary>
    ///     Executes a specified synchronous function on the value of a successful <see cref="Result{TValue}" />
    ///     asynchronously.
    ///     If the <see cref="Result{TValue}" /> represents a failure, the function is not executed,
    ///     and the failure is propagated.
    /// </summary>
    /// <param name="resultTask">The asynchronous <see cref="Result{TValue}" /> to process.</param>
    /// <param name="func">The function to execute if the <see cref="Result{TValue}" /> is successful.</param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> containing a new <see cref="Result{TOut}" />
    ///     obtained by applying the function on the successful value,
    ///     or the propagated failure if the <see cref="Result{TValue}" /> is not successful.
    /// </returns>
    public static async ValueTask<Result<TOut>> Try<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                               Func<TIn, TOut>             func)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        return result.Try(func);
    }

    /// <summary>
    ///     Attempts to execute the specified asynchronous function on the successful result of the given
    ///     <see cref="Result{TIn}" />,
    ///     and returns a new <see cref="Result{TOut}" /> containing the outcome of the operation.
    /// </summary>
    /// <typeparam name="TIn">The type of the value in the input result.</typeparam>
    /// <typeparam name="TOut">The type of the value in the resulting result.</typeparam>
    /// <param name="resultTask">An asynchronous operation representing a <see cref="Result{TIn}" />.</param>
    /// <param name="func">
    ///     A function to apply to the value contained in the input result.
    ///     The function should return a task that produces a value of type <typeparamref name="TOut" />.
    /// </param>
    /// <returns>
    ///     An asynchronous operation that produces a <see cref="Result{TOut}" />.
    ///     If the operation succeeds, the result contains the value returned by <paramref name="func" />.
    ///     If an exception occurs during the operation, the result will represent a failure with the corresponding exception.
    /// </returns>
    public static async ValueTask<Result<TOut>> Try<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                               Func<TIn, ValueTask<TOut>>  func)
        where TIn : notnull
        where TOut : notnull {
        var result = await resultTask;
        return await result.Bind(async value => {
            try {
                var newValue = await func(value);
                return Result.Success(newValue);
            }
            catch (Exception ex) {
                return Result.Failure<TOut>(ex);
            }
        });
    }
}
