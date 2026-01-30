using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Combines multiple asynchronous tasks that produce <see cref="Result" /> into a single asynchronous operation.
    /// </summary>
    /// <param name="tasks">The collection of asynchronous tasks to combine.</param>
    /// <returns>
    ///     An asynchronous task containing a combined <see cref="Result" />, indicating success if all tasks succeed, or
    ///     an aggregation of errors if any fail.
    /// </returns>
    public static ResultTask Combine(this IEnumerable<ResultTask> tasks)
    {
        return CombineCore(tasks)
            .AsAsync();

        static async ValueTask<Result> CombineCore(IEnumerable<ResultTask> tasks)
        {
            var errors = new List<Failure>();
            foreach (var t in tasks)
            {
                var r = await t;
                if (r.TryGetError(out var error))
                {
                    errors.Add(error);
                }
            }

            if (errors.Count != 0)
            {
                return new AggregateFailure(errors);
            }

            return Result.Success();
        }
    }

    /// <summary>
    ///     Combines a collection of <see cref="ResultTask{TValue}" /> instances into a single
    ///     <see cref="ResultTask{TValue}" />
    ///     that represents the aggregated result of the input tasks.
    /// </summary>
    /// <typeparam name="TValue">The type of the values contained in the input tasks.</typeparam>
    /// <param name="tasks">The collection of tasks to combine.</param>
    /// <returns>A single <see cref="ResultTask{TValue}" /> that represents the combined result of the input tasks.</returns>
    public static ResultTask<IEnumerable<TValue>> Combine<TValue>(this IEnumerable<ResultTask<TValue>> tasks)
        where TValue : notnull
    {
        return CombineCore(tasks)
            .AsAsync();

        static async ValueTask<Result<IEnumerable<TValue>>> CombineCore(
            IEnumerable<ResultTask<TValue>> tasks)
        {
            var errors = new List<Failure>();
            var values = new List<TValue>();
            foreach (var t in tasks)
            {
                var r = await t;
                if (!r.TryGet(out var value, out var error))
                {
                    errors.Add(error);
                }
                else
                {
                    values.Add(value);
                }
            }

            if (errors.Count != 0)
            {
                return new AggregateFailure(errors);
            }

            return values;
        }
    }
}