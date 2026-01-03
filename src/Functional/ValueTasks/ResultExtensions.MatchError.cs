using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Asynchronously matches the first error of the specified type in the result and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TError">The type of error to match. Must be a class implementing IError.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The result to match errors for.</param>
    /// <param name="onMatch">The async function to execute when a matching error is found.</param>
    /// <param name="onElse">The async function to execute when no matching error is found.</param>
    /// <returns>A task containing the result of executing either onMatch or onElse function.</returns>
    public static ValueTask<TOut> MatchErrorAsync<TError, TOut>(this Result result,
        Func<TError, ValueTask<TOut>> onMatch, Func<ValueTask<TOut>> onElse) where TError : class, IError
    {
        return result.GetError<TError>()
            .Match(onMatch, onElse);
    }

    /// <summary>
    ///     Asynchronously matches the first error of the specified type in the result and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TError">The type of error to match. Must be a class implementing IError.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="awaitableResult">The awaitable result to match errors for.</param>
    /// <param name="onMatch">The async function to execute when a matching error is found.</param>
    /// <param name="onElse">The async function to execute when no matching error is found.</param>
    /// <returns>A task containing the result of executing either onMatch or onElse function.</returns>
    public static async ValueTask<TOut> MatchErrorAsync<TError, TOut>(this ValueTask<Result> awaitableResult,
        Func<TError, ValueTask<TOut>> onMatch, Func<ValueTask<TOut>> onElse) where TError : class, IError
    {
        var result = await awaitableResult;
        return await result.MatchErrorAsync(onMatch, onElse);
    }

    /// <summary>
    ///     Asynchronously matches the first error of the specified type in the result and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TError">The type of error to match. Must be a class implementing IError.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <typeparam name="TValue1">The type of the first value in the result.</typeparam>
    /// <param name="result">The result to match errors for.</param>
    /// <param name="onMatch">The async function to execute when a matching error is found.</param>
    /// <param name="onElse">The async function to execute when no matching error is found.</param>
    /// <returns>A task containing the result of executing either onMatch or onElse function.</returns>
    public static ValueTask<TOut> MatchErrorAsync<TError, TValue1, TOut>(this Result<TValue1> result,
        Func<TError, ValueTask<TOut>> onMatch, Func<ValueTask<TOut>> onElse)
        where TError : class, IError where TValue1 : notnull
    {
        return result.GetError<TError>()
            .Match(onMatch, onElse);
    }

    /// <summary>
    ///     Asynchronously matches the first error of the specified type in the result and executes the corresponding function.
    /// </summary>
    /// <typeparam name="TError">The type of error to match. Must be a class implementing IError.</typeparam>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <typeparam name="TValue1">The type of the first value in the result.</typeparam>
    /// <param name="awaitableResult">The awaitable result to match errors for.</param>
    /// <param name="onMatch">The async function to execute when a matching error is found.</param>
    /// <param name="onElse">The async function to execute when no matching error is found.</param>
    /// <returns>A task containing the result of executing either onMatch or onElse function.</returns>
    public static async ValueTask<TOut> MatchErrorAsync<TError, TValue1, TOut>(
        this ValueTask<Result<TValue1>> awaitableResult, Func<TError, ValueTask<TOut>> onMatch,
        Func<ValueTask<TOut>> onElse) where TError : class, IError where TValue1 : notnull
    {
        var result = await awaitableResult;
        return await result.MatchErrorAsync(onMatch, onElse);
    }
}