using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    public static async ValueTask<Result> Combine(this IEnumerable<ValueTask<Result>> resultTasks)
    {
        var results = new List<Result>();
        foreach (var resultTask in resultTasks)
        {
            results.Add(await resultTask);
        }

        return results.Combine();
    }

    public static async ValueTask<Result<IEnumerable<TValue>>> Combine<TValue>(this IEnumerable<ValueTask<Result<TValue>>> resultTasks)
        where TValue : notnull
    {
        var results = new List<Result<TValue>>();
        foreach (var resultTask in resultTasks)
        {
            results.Add(await resultTask);
        }

        return results.Combine();
    }

    // Task extensions
    public static ValueTask<Result> Combine(this IEnumerable<Task<Result>> resultTasks)
    {
        return new ValueTask<Result>(CombineCore(resultTasks));

        static async Task<Result> CombineCore(IEnumerable<Task<Result>> resultTasks)
        {
            var results = new List<Result>();
            foreach (var resultTask in resultTasks)
            {
                results.Add(await resultTask);
            }

            return results.Combine();
        }
    }

    public static ValueTask<Result<IEnumerable<TValue>>> Combine<TValue>(this IEnumerable<Task<Result<TValue>>> resultTasks)
        where TValue : notnull
    {
        return new ValueTask<Result<IEnumerable<TValue>>>(CombineCore(resultTasks));

        static async Task<Result<IEnumerable<TValue>>> CombineCore(IEnumerable<Task<Result<TValue>>> resultTasks)
        {
            var results = new List<Result<TValue>>();
            foreach (var resultTask in resultTasks)
            {
                results.Add(await resultTask);
            }

            return results.Combine();
        }
    }
}

#pragma warning restore CS1591
