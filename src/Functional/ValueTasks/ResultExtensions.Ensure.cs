using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Validates the result values asynchronously with a predicate and returns a failure with the provided exception if
    ///     validation fails.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="predicate">The async validation predicate.</param>
    /// <param name="errorFactory">Factory function to create an exception when validation fails.</param>
    /// <returns>A ValueTask representing the async operation with the result.</returns>
    public static ValueTask<Result<TValue1>> EnsureAsync<TValue1>(this Result<TValue1> result,
        Func<TValue1, ValueTask<bool>> predicate, Func<TValue1, ValueTask<Error>> errorFactory) where TValue1 : notnull
    {
        return result.ThenAsync(async value =>
        {
            if (await predicate(value))
            {
                return Result.Success(value);
            }

            var ex = await errorFactory(value);
            return Result.Failure<TValue1>(ex);
        });
    }

    /// <summary>
    ///     Validates the awaitable result values asynchronously with a predicate.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="awaitableResult">The ValueTask of result to await.</param>
    /// <param name="predicate">The async validation predicate.</param>
    /// <param name="errorFactory">Factory function to create an exception when validation fails.</param>
    /// <returns>A ValueTask representing the async operation with the result.</returns>
    public static async ValueTask<Result<TValue1>> EnsureAsync<TValue1>(this ValueTask<Result<TValue1>> awaitableResult,
        Func<TValue1, ValueTask<bool>> predicate, Func<TValue1, ValueTask<Error>> errorFactory) where TValue1 : notnull
    {
        var result = await awaitableResult;
        return await result.EnsureAsync(predicate, errorFactory);
    }
}
