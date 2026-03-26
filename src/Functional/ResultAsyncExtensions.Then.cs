#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    public static async ValueTask<Result> Then(this ValueTask<Result> resultTask,
                                               Func<Result>            then)
    {
        var result = await resultTask;
        return result.Then(then);
    }

    public static async ValueTask<Result> Then(this ValueTask<Result>      resultTask,
                                               Func<ValueTask<Result>>     then)
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            return result;
        }

        return await then();
    }

    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>>        resultTask,
                                                                Func<TValue, Result<TValue>>          then)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.Then(then);
    }

    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>>             resultTask,
                                                                Func<TValue, ValueTask<Result<TValue>>>    then)
        where TValue : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            return result;
        }

        var response = await then(value);
        return response.WithMetadata(result.Metadata);
    }

    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                Func<TValue, Result>           then)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.Then(then);
    }

    public static async ValueTask<Result<TValue>> Then<TValue>(this ValueTask<Result<TValue>>      resultTask,
                                                                Func<TValue, ValueTask<Result>>    then)
        where TValue : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            return result;
        }

        var thenResult = await then(value);
        if (!thenResult.TryGetFailure(out var error))
        {
            return result;
        }

        var failResult = Result.Failure<TValue>(error);
        return failResult.WithMetadata(result.Metadata);
    }

    // Task extensions
    public static ValueTask<Result> Then(this Task<Result> resultTask,
                                         Func<Result>      then)
    {
        return new ValueTask<Result>(ThenCore(resultTask, then));

        static async Task<Result> ThenCore(Task<Result> resultTask,
                                           Func<Result> then)
        {
            return (await resultTask).Then(then);
        }
    }

    public static ValueTask<Result> Then(this Task<Result>          resultTask,
                                         Func<ValueTask<Result>>    then)
    {
        return new ValueTask<Result>(ThenCore(resultTask, then));

        static async Task<Result> ThenCore(Task<Result>             resultTask,
                                           Func<ValueTask<Result>>  then)
        {
            return await new ValueTask<Result>(resultTask).Then(then);
        }
    }

    public static ValueTask<Result<TValue>> Then<TValue>(this Task<Result<TValue>>          resultTask,
                                                          Func<TValue, Result<TValue>>      then)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(ThenCore(resultTask, then));

        static async Task<Result<TValue>> ThenCore(Task<Result<TValue>>         resultTask,
                                                   Func<TValue, Result<TValue>> then)
        {
            return (await resultTask).Then(then);
        }
    }

    public static ValueTask<Result<TValue>> Then<TValue>(this Task<Result<TValue>>                 resultTask,
                                                          Func<TValue, ValueTask<Result<TValue>>>  then)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(ThenCore(resultTask, then));

        static async Task<Result<TValue>> ThenCore(Task<Result<TValue>>                    resultTask,
                                                   Func<TValue, ValueTask<Result<TValue>>> then)
        {
            return await new ValueTask<Result<TValue>>(resultTask).Then(then);
        }
    }

    public static ValueTask<Result<TValue>> Then<TValue>(this Task<Result<TValue>>   resultTask,
                                                          Func<TValue, Result>        then)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(ThenCore(resultTask, then));

        static async Task<Result<TValue>> ThenCore(Task<Result<TValue>>  resultTask,
                                                   Func<TValue, Result>  then)
        {
            return (await resultTask).Then(then);
        }
    }

    public static ValueTask<Result<TValue>> Then<TValue>(this Task<Result<TValue>>        resultTask,
                                                          Func<TValue, ValueTask<Result>> then)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(ThenCore(resultTask, then));

        static async Task<Result<TValue>> ThenCore(Task<Result<TValue>>           resultTask,
                                                   Func<TValue, ValueTask<Result>> then)
        {
            return await new ValueTask<Result<TValue>>(resultTask).Then(then);
        }
    }
}

#pragma warning restore CS1591
