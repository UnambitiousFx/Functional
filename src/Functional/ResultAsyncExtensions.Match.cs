using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions - Result<TIn>
    /// <summary>
    ///     Pattern matches the async result, returning a value from the appropriate function.
    /// </summary>
    public static async ValueTask<TOut> Match<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                         Func<TIn, TOut>             onSuccess,
                                                         Func<Failure, TOut>         onFailure)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Pattern matches the async result using asynchronous functions, returning a value.
    /// </summary>
    public static async ValueTask<TOut> Match<TIn, TOut>(this ValueTask<Result<TIn>>    resultTask,
                                                         Func<TIn, ValueTask<TOut>>     onSuccess,
                                                         Func<Failure, ValueTask<TOut>> onFailure)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.TryGetValue(out var value))
        {
            return await onSuccess(value);
        }

        result.TryGetError(out var error);
        return await onFailure(error!);
    }

    // Task extensions - Result<TIn>
    /// <summary>
    ///     Pattern matches the async result, returning a value from the appropriate function.
    /// </summary>
    public static ValueTask<TOut> Match<TIn, TOut>(this Task<Result<TIn>>  resultTask,
                                                   Func<TIn, TOut>         onSuccess,
                                                   Func<Failure, TOut>     onFailure)
        where TIn : notnull
    {
        return new ValueTask<TOut>(MatchCore(resultTask, onSuccess, onFailure));

        static async Task<TOut> MatchCore(Task<Result<TIn>>    resultTask,
                                          Func<TIn, TOut>      onSuccess,
                                          Func<Failure, TOut>  onFailure)
        {
            return (await resultTask).Match(onSuccess, onFailure);
        }
    }

    /// <summary>
    ///     Pattern matches the async result using asynchronous functions, returning a value.
    /// </summary>
    public static ValueTask<TOut> Match<TIn, TOut>(this Task<Result<TIn>>         resultTask,
                                                   Func<TIn, ValueTask<TOut>>     onSuccess,
                                                   Func<Failure, ValueTask<TOut>> onFailure)
        where TIn : notnull
    {
        return new ValueTask<TOut>(MatchCore(resultTask, onSuccess, onFailure));

        static async Task<TOut> MatchCore(Task<Result<TIn>>             resultTask,
                                          Func<TIn, ValueTask<TOut>>     onSuccess,
                                          Func<Failure, ValueTask<TOut>> onFailure)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Match(onSuccess, onFailure);
        }
    }
}
