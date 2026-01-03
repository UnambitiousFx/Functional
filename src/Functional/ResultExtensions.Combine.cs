using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Combines multiple results into a single result, aggregating all errors if any result failed.
    /// </summary>
    /// <param name="results">The collection of results to combine.</param>
    /// <returns>A successful result if all results succeeded, otherwise a failure result with aggregated errors.</returns>
    public static Result Combine(this IEnumerable<Result> results)
    {
        var errors = new List<Error>();
        foreach (var result in results)
        {
            if (!result.TryGet(out var err))
            {
                errors.Add(err);
            }
        }

        return errors.Count != 0
            ? Result.Failure(new AggregateError(errors))
            : Result.Success();
    }
}
