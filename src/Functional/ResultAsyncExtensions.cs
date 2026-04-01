using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Direct async fluent operators for <see cref="Result" /> and <see cref="Result{TValue}" /> pipelines.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Asynchronously evaluates a <see cref="Result{TValue}" /> and executes the appropriate action based on success or
    ///     failure.
    /// </summary>
    /// <typeparam name="TIn">The type of the value contained in the result.</typeparam>
    /// <param name="resultTask">A task that represents the asynchronous computation of a <see cref="Result{TIn}" />.</param>
    /// <param name="onSuccess">The action to execute if the result is successful.</param>
    /// <param name="onFailure">The action to execute if the result is a failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async ValueTask Switch<TIn>(this ValueTask<Result<TIn>> resultTask,
                                              Action<TIn>                 onSuccess,
                                              Action<Failure>             onFailure)
        where TIn : notnull {
        var result = await resultTask;
        result.Switch(onSuccess, onFailure);
    }

    /// <summary>
    ///     Executes one of two actions depending on the result of the provided task.
    /// </summary>
    /// <typeparam name="TIn">The type of the result value.</typeparam>
    /// <param name="resultTask">The asynchronous task that provides the result to evaluate.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result represents a failure.</param>
    /// <returns>A <see cref="ValueTask" /> that completes once the appropriate function has been executed.</returns>
    public static async ValueTask Switch<TIn>(this ValueTask<Result<TIn>> resultTask,
                                              Func<TIn, ValueTask>        onSuccess,
                                              Func<Failure, ValueTask>    onFailure)
        where TIn : notnull {
        var result = await resultTask;
        if (result.TryGetValue(out var value)) {
            await onSuccess(value);
            return;
        }

        result.TryGetFailure(out var error);
        await onFailure(error!);
    }
}
