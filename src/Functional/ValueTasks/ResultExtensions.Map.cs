namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async Map transforming success value(s) using an async mapping function.
    /// </summary>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    /// <typeparam name="TOut1">Output value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="map">The async mapping function.</param>
    /// <returns>A task with the transformed result.</returns>
    public static ValueTask<Result<TOut1>> MapAsync<TValue1, TOut1>(this Result<TValue1> result,
        Func<TValue1, ValueTask<TOut1>> map) where TValue1 : notnull where TOut1 : notnull
    {
        return result.BindAsync(async value =>
        {
            var newValue = await map(value).ConfigureAwait(false);
            return Result.Success(newValue);
        });
    }

    /// <param name="awaitableResult">The awaitable result instance.</param>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    extension<TValue1>(ValueTask<Result<TValue1>> awaitableResult) where TValue1 : notnull
    {
        /// <summary>
        ///     Async Map awaiting result then transforming using a sync mapping function.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="map">The mapping function.</param>
        /// <returns>A task with the transformed result.</returns>
        public async ValueTask<Result<TOut1>> MapAsync<TOut1>(Func<TValue1, TOut1> map) where TOut1 : notnull
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return result.Map(map);
        }

        /// <summary>
        ///     Async Map awaiting result then transforming using an async mapping function.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="map">The async mapping function.</param>
        /// <returns>A task with the transformed result.</returns>
        public ValueTask<Result<TOut1>> MapAsync<TOut1>(Func<TValue1, ValueTask<TOut1>> map) where TOut1 : notnull
        {
            return awaitableResult.BindAsync(async value =>
            {
                var newValue = await map(value).ConfigureAwait(false);
                return Result.Success(newValue);
            });
        }
    }
}