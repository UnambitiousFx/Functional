using UnambitiousFx.Functional.Failures;

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
        var errors = new List<Failure>();
        foreach (var result in results)
        {
            if (result.TryGetError(out var err))
            {
                errors.Add(err);
            }
        }

        return errors.Count != 0
            ? new AggregateFailure(errors)
            : Result.Success();
    }

    /// <summary>
    ///     Combines a collection of results of a specified type into a single result,
    ///     producing a successful result with all values if all results succeeded,
    ///     or a failure result with aggregated errors if any result failed.
    /// </summary>
    /// <typeparam name="TValue">The type of the values within the results.</typeparam>
    /// <param name="results">The collection of results to combine.</param>
    /// <returns>
    ///     A successful result containing an enumerable of all values if all results succeeded,
    ///     otherwise a failure result with aggregated errors.
    /// </returns>
    public static Result<IEnumerable<TValue>> Combine<TValue>(this IEnumerable<Result<TValue>> results)
        where TValue : notnull
    {
        var errors = new List<Failure>();
        var values = new List<TValue>();
        foreach (var result in results)
        {
            if (!result.TryGet(out var value, out var err))
            {
                errors.Add(err);
            }
            else
            {
                values.Add(value);
            }
        }

        return errors.Count != 0
            ? new AggregateFailure(errors)
            : values;
    }
}