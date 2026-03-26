using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                            Func<Failure, TIn>          recover)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Recover(recover);
    }

    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>>     resultTask,
                                                            Func<Failure, ValueTask<TIn>>   recover)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            return result;
        }

        result.TryGetFailure(out var error);
        return Result.Success(await recover(error!));
    }

    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                            TIn                         fallback)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Recover(fallback);
    }

    // Task extensions
    public static ValueTask<Result<TIn>> Recover<TIn>(this Task<Result<TIn>> resultTask,
                                                      Func<Failure, TIn>     recover)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(RecoverCore(resultTask, recover));

        static async Task<Result<TIn>> RecoverCore(Task<Result<TIn>>  resultTask,
                                                   Func<Failure, TIn> recover)
        {
            return (await resultTask).Recover(recover);
        }
    }

    public static ValueTask<Result<TIn>> Recover<TIn>(this Task<Result<TIn>>          resultTask,
                                                      Func<Failure, ValueTask<TIn>>   recover)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(RecoverCore(resultTask, recover));

        static async Task<Result<TIn>> RecoverCore(Task<Result<TIn>>           resultTask,
                                                   Func<Failure, ValueTask<TIn>> recover)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Recover(recover);
        }
    }

    public static ValueTask<Result<TIn>> Recover<TIn>(this Task<Result<TIn>> resultTask,
                                                      TIn                    fallback)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(RecoverCore(resultTask, fallback));

        static async Task<Result<TIn>> RecoverCore(Task<Result<TIn>> resultTask,
                                                   TIn               fallback)
        {
            return (await resultTask).Recover(fallback);
        }
    }
}

#pragma warning restore CS1591
