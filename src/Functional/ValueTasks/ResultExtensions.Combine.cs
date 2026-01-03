using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Combines a collection of asynchronous <see cref="ValueTask{TResult}" /> instances that return <see cref="Result" />
    ///     and produces a single combined <see cref="Result" />. If any of the results are failures, the combined result
    ///     will also be a failure, consolidating all encountered errors.
    /// </summary>
    /// <param name="tasks">A collection of <see cref="ValueTask{Result}" /> instances to combine.</param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> that represents the combined outcome. If all tasks succeed,
    ///     the combined result will be a success. Otherwise, it will be a failure containing all accumulated errors.
    /// </returns>
    public static async ValueTask<Result> CombineAsync(this IEnumerable<ValueTask<Result>> tasks)
    {
        var errors = new List<Error>();
        foreach (var t in tasks)
        {
            var r = await t.ConfigureAwait(false);
            if (!r.TryGet(out var error))
            {
                errors.Add(error);
            }
        }

        return errors.Count != 0
            ? Result.Failure(errors)
            : Result.Success();
    }

    /// <summary>
    ///     Combines a collection of asynchronous <see cref="ValueTask{Result}" /> instances into a single result.
    ///     If any of the results contain errors, the combined result will be a failure containing all aggregated errors.
    /// </summary>
    /// <param name="awaitableResults">
    ///     A <see cref="ValueTask{TResult}" /> that represents an enumerable of <see cref="Result" /> instances to combine.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> representing the combined result. If all results are successful,
    ///     the returned result will be a success. Otherwise, it will be a failure with aggregated errors.
    /// </returns>
    public static async ValueTask<Result> CombineAsync(this ValueTask<IEnumerable<Result>> awaitableResults)
    {
        var results = await awaitableResults.ConfigureAwait(false);
        return results.Combine();
    }
}
