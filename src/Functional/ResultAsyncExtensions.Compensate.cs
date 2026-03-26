using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    /// <summary>
    ///     Attempts to compensate for a failure by executing an asynchronous rollback function.
    /// </summary>
    public static async ValueTask<Result<TValue>> Compensate<TValue>(this ValueTask<Result<TValue>>  resultTask,
                                                                     Func<Failure, ValueTask<Result>> rollback)
        where TValue : notnull
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            return result;
        }

        result.TryGetFailure(out var originalError);
        var rollbackResult = await rollback(originalError!);

        return rollbackResult.Match(
            () => result,
            rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError!, rollbackError)));
    }

    /// <summary>
    ///     Attempts to compensate for a failure by executing a rollback function.
    /// </summary>
    public static async ValueTask<Result<TValue>> Compensate<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                     Func<Failure, Result>           rollback)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.Compensate(rollback);
    }

    // Task extensions
    /// <summary>
    ///     Attempts to compensate for a failure by executing an asynchronous rollback function.
    /// </summary>
    public static ValueTask<Result<TValue>> Compensate<TValue>(this Task<Result<TValue>>       resultTask,
                                                               Func<Failure, ValueTask<Result>> rollback)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(CompensateCore(resultTask, rollback));

        static async Task<Result<TValue>> CompensateCore(Task<Result<TValue>>          resultTask,
                                                         Func<Failure, ValueTask<Result>> rollback)
        {
            return await new ValueTask<Result<TValue>>(resultTask).Compensate(rollback);
        }
    }

    /// <summary>
    ///     Attempts to compensate for a failure by executing a rollback function.
    /// </summary>
    public static ValueTask<Result<TValue>> Compensate<TValue>(this Task<Result<TValue>> resultTask,
                                                               Func<Failure, Result>     rollback)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(CompensateCore(resultTask, rollback));

        static async Task<Result<TValue>> CompensateCore(Task<Result<TValue>>  resultTask,
                                                         Func<Failure, Result> rollback)
        {
            return (await resultTask).Compensate(rollback);
        }
    }
}
