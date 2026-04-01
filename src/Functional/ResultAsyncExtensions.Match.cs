using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Pattern matches the async result, returning a value from the appropriate function.
    /// </summary>
    public static async ValueTask<TOut> Match<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                         Func<TIn, TOut>             onSuccess,
                                                         Func<Failure, TOut>         onFailure)
        where TIn : notnull {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Pattern matches the async result using asynchronous functions, returning a value.
    /// </summary>
    public static async ValueTask<TOut> Match<TIn, TOut>(this ValueTask<Result<TIn>>    resultTask,
                                                         Func<TIn, ValueTask<TOut>>     onSuccess,
                                                         Func<Failure, ValueTask<TOut>> onFailure)
        where TIn : notnull {
        var result = await resultTask;
        if (result.TryGetValue(out var value)) {
            return await onSuccess(value);
        }

        result.TryGetFailure(out var error);
        return await onFailure(error!);
    }
}
