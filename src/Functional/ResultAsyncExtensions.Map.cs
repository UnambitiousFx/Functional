using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    /// <summary>
    ///     Transforms the value inside the async result using an asynchronous mapping function.
    /// </summary>
    public static async ValueTask<Result<TOut>> Map<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                               Func<TIn, ValueTask<TOut>>  map)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            result.TryGetError(out var error);
            return Result.Failure<TOut>(error!);
        }

        return Result.Success(await map(value));
    }

    // Task extensions
    /// <summary>
    ///     Transforms the value inside the async result using an asynchronous mapping function.
    /// </summary>
    public static ValueTask<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>>        resultTask,
                                                         Func<TIn, ValueTask<TOut>> map)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(MapCore(resultTask, map));

        static async Task<Result<TOut>> MapCore(Task<Result<TIn>>         resultTask,
                                                Func<TIn, ValueTask<TOut>> map)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Map(map);
        }
    }
}
