namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async Tap executing a side effect on success with async function.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Async function to execute on success.</param>
    /// <returns>A task with the original result unchanged.</returns>
    public static async ValueTask<Result> TapAsync(this Result result, Func<ValueTask> tap)
    {
        if (result.IsSuccess)
        {
            await tap();
        }

        return result;
    }

    /// <summary>
    ///     Async Tap executing a side effect on success with async function.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Async function to execute on success.</param>
    /// <returns>A task with the original result unchanged.</returns>
    public static async ValueTask<Result<TValue1>> TapAsync<TValue1>(this Result<TValue1> result,
        Func<TValue1, ValueTask> tap) where TValue1 : notnull
    {
        if (result.TryGet(out TValue1? value1))
        {
            await tap(value1);
        }

        return result;
    }

    /// <param name="awaitableResult">The awaitable result instance.</param>
    extension(ValueTask<Result> awaitableResult)
    {
        /// <summary>
        ///     Async Tap executing a side effect on an awaitable result with sync action.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result> TapAsync(Action tap)
        {
            var result = await awaitableResult;
            result.IfSuccess(tap);
            return result;
        }

        /// <summary>
        ///     Async Tap executing a side effect on an awaitable result with async function.
        /// </summary>
        /// <param name="tap">Async function to execute on success.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result> TapAsync(Func<ValueTask> tap)
        {
            var result = await awaitableResult;
            return await result.TapAsync(tap);
        }
    }


    /// <param name="awaitableResult">The awaitable result instance.</param>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    extension<TValue1>(ValueTask<Result<TValue1>> awaitableResult) where TValue1 : notnull
    {
        /// <summary>
        ///     Async Tap executing a side effect on an awaitable result with sync action.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result<TValue1>> TapAsync(Action<TValue1> tap)
        {
            var result = await awaitableResult;
            result.IfSuccess(tap);
            return result;
        }

        /// <summary>
        ///     Async Tap executing a side effect on an awaitable result with async function.
        /// </summary>
        /// <param name="tap">Async function to execute on success.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result<TValue1>> TapAsync(Func<TValue1, ValueTask> tap)
        {
            var result = await awaitableResult;
            return await result.TapAsync(tap);
        }
    }
}
