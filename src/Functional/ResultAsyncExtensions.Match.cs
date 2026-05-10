using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
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
        if (result.TryGetValue(out var value)) {
            return await onSuccess(value);
        }

        result.TryGetFailure(out var error);
        return await onFailure(error!);
    }

    /// <summary>
    ///     Executes the appropriate function based on the outcome of the asynchronous <see cref="Result" />.
    /// </summary>
    /// <typeparam name="TOut">
    ///     The type of the output value returned by the <paramref name="onSuccess" /> or
    ///     <paramref name="onFailure" /> function.
    /// </typeparam>
    /// <param name="resultTask">The asynchronous <see cref="Result" /> to evaluate.</param>
    /// <param name="onSuccess">The function to execute if the result represents success.</param>
    /// <param name="onFailure">The function to execute if the result represents failure.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the value returned by the <paramref name="onSuccess" /> or <paramref name="onFailure" />
    ///     function.
    /// </returns>
    public static async ValueTask Match<TOut>(this ValueTask<Result> resultTask,
                                              Func<TOut>             onSuccess,
                                              Func<Failure, TOut>    onFailure)
    {
        var result = await resultTask;
        result.Match(onSuccess, onFailure);
    }

    /// <summary>
    ///     Executes the specified actions based on the success or failure state of an asynchronous <see cref="Result" />.
    /// </summary>
    /// <param name="resultTask">
    ///     The asynchronous <see cref="Result" /> to evaluate.
    /// </param>
    /// <param name="onSuccess">
    ///     The action to execute if the <see cref="Result" /> represents a successful operation.
    /// </param>
    /// <param name="onFailure">
    ///     The action to execute if the <see cref="Result" /> represents a failure.
    /// </param>
    public static async ValueTask Match(this ValueTask<Result> resultTask,
                                        Action                 onSuccess,
                                        Action<Failure>        onFailure)
    {
        var result = await resultTask;
        result.Match(onSuccess, onFailure);
    }
}
