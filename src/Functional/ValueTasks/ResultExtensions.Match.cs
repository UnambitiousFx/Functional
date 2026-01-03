using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async Match for pattern matching on Result, executing success or failure handler.
    /// </summary>
    public static async ValueTask<TOut> MatchAsync<TOut, TValue1>(this ValueTask<Result<TValue1>> awaitableResult,
        Func<TValue1, ValueTask<TOut>> success, Func<Error, ValueTask<TOut>> failure)
        where TOut : notnull where TValue1 : notnull
    {
        var result = await awaitableResult.ConfigureAwait(false);
        return await result.Match(success, failure);
    }

    /// <summary>
    ///     Async Match for pattern matching on Result, executing success or failure handler.
    /// </summary>
    public static async ValueTask MatchAsync<TValue1>(this ValueTask<Result<TValue1>> awaitableResult,
        Func<TValue1, ValueTask> success, Func<Error, ValueTask> failure) where TValue1 : notnull
    {
        var result = await awaitableResult.ConfigureAwait(false);
        await result.Match(success, failure);
    }
}