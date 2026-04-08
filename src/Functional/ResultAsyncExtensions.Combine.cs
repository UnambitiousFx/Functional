namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Combines a collection of asynchronous <see cref="Result" /> instances into a single <see cref="Result" />.
    /// </summary>
    /// <param name="resultTasks">
    ///     A collection of tasks that resolve to <see cref="Result" /> instances.
    /// </param>
    /// <returns>
    ///     A combined <see cref="Result" /> that represents the aggregated success or failure of the provided results.
    ///     If any result in the collection is a failure, the combined result will also be a failure.
    /// </returns>
    public static async ValueTask<Result> Combine(this IEnumerable<ValueTask<Result>> resultTasks) {
        var results = new List<Result>();
        foreach (var resultTask in resultTasks) {
            results.Add(await resultTask);
        }

        return results.Combine();
    }

    /// <summary>
    ///     Combines a collection of asynchronous <see cref="Result{TValue}" /> tasks into a single asynchronous result
    ///     containing a collection of all success values, or a failure if any task fails.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="resultTasks">The collection of asynchronous result tasks to combine.</param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> containing a <see cref="Result{TValue}" /> that represents either
    ///     a collection of all success values if all tasks succeed, or the first encountered failure.
    /// </returns>
    public static async ValueTask<Result<IEnumerable<TValue>>> Combine<TValue>(this IEnumerable<ValueTask<Result<TValue>>> resultTasks)
        where TValue : notnull {
        var results = new List<Result<TValue>>();
        foreach (var resultTask in resultTasks) {
            results.Add(await resultTask);
        }

        return results.Combine();
    }
}
