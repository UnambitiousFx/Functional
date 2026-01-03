using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Asynchronously maps errors in the result using the specified mapping function.
    /// </summary>
    /// <param name="result">The result to map errors for.</param>
    /// <param name="mapError">The async function to map errors.</param>
    /// <returns>
    ///     A task with a new result containing mapped errors if the original result failed, otherwise the original
    ///     successful result.
    /// </returns>
    public static ValueTask<Result> MapErrorAsync(this Result result,
        Func<Error, ValueTask<Error>> mapError)
    {
        return result.Match<ValueTask<Result>>(
            () => new ValueTask<Result>(Result.Success()),
            async ex => Result.Failure(await mapError(ex))
        );
    }

    /// <summary>
    ///     Asynchronously maps errors in the result using the specified mapping function.
    /// </summary>
    /// <param name="awaitableResult">The awaitable result to map errors for.</param>
    /// <param name="mapError">The async function to map errors.</param>
    /// <returns>
    ///     A task with a new result containing mapped errors if the original result failed, otherwise the original
    ///     successful result.
    /// </returns>
    public static async ValueTask<Result> MapErrorAsync(this ValueTask<Result> awaitableResult,
        Func<Error, ValueTask<Error>> mapError)
    {
        var result = await awaitableResult;
        return await result.MapErrorAsync(mapError);
    }

    /// <summary>
    ///     Asynchronously maps errors in the result using the specified mapping function.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="result">The result to map errors for.</param>
    /// <param name="mapError">The async function to map errors.</param>
    /// <returns>
    ///     A task with a new result containing mapped errors if the original result failed, otherwise the original
    ///     successful result.
    /// </returns>
    public static ValueTask<Result<T1>> MapErrorAsync<T1>(this Result<T1> result,
        Func<Error, ValueTask<Error>> mapError) where T1 : notnull
    {
        return result.Match<ValueTask<Result<T1>>>(
            value1 => new ValueTask<Result<T1>>(Result.Success(value1)),
            async ex => Result.Failure<T1>(await mapError(ex))
        );
    }

    /// <summary>
    ///     Asynchronously maps errors in the result using the specified mapping function.
    /// </summary>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <param name="awaitableResult">The awaitable result to map errors for.</param>
    /// <param name="mapError">The async function to map errors.</param>
    /// <returns>
    ///     A task with a new result containing mapped errors if the original result failed, otherwise the original
    ///     successful result.
    /// </returns>
    public static async ValueTask<Result<T1>> MapErrorAsync<T1>(this ValueTask<Result<T1>> awaitableResult,
        Func<Error, ValueTask<Error>> mapError) where T1 : notnull
    {
        var result = await awaitableResult;
        return await result.MapErrorAsync(mapError);
    }
}