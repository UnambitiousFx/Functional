#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    public static async ValueTask<Result<TValue>> Flatten<TValue>(this ValueTask<Result<Result<TValue>>> resultTask)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.Flatten();
    }

    public static async ValueTask<Result<TValue>> Flatten<TValue>(this ValueTask<Result<ValueTask<Result<TValue>>>> resultTask)
        where TValue : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var innerTask))
        {
            result.TryGetError(out var error);
            return Result.Failure<TValue>(error!);
        }

        return await innerTask;
    }

    // Task extensions
    public static ValueTask<Result<TValue>> Flatten<TValue>(this Task<Result<Result<TValue>>> resultTask)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(FlattenCore(resultTask));

        static async Task<Result<TValue>> FlattenCore(Task<Result<Result<TValue>>> resultTask)
        {
            return (await resultTask).Flatten();
        }
    }

    public static ValueTask<Result<TValue>> Flatten<TValue>(this Task<Result<ValueTask<Result<TValue>>>> resultTask)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(FlattenCore(resultTask));

        static async Task<Result<TValue>> FlattenCore(Task<Result<ValueTask<Result<TValue>>>> resultTask)
        {
            return await new ValueTask<Result<ValueTask<Result<TValue>>>>(resultTask).Flatten();
        }
    }

    public static ValueTask<Result<TValue>> Flatten<TValue>(this Task<Result<Task<Result<TValue>>>> resultTask)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(FlattenCore(resultTask));

        static async Task<Result<TValue>> FlattenCore(Task<Result<Task<Result<TValue>>>> resultTask)
        {
            var result = await resultTask;
            if (!result.TryGetValue(out var innerTask))
            {
                result.TryGetError(out var error);
                return Result.Failure<TValue>(error!);
            }

            return await innerTask;
        }
    }
}

#pragma warning restore CS1591
