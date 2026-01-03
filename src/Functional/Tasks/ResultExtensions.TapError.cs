using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async TapError executing a side effect on failure with async function.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Async function to execute on failure.</param>
    /// <returns>A task with the original result unchanged.</returns>
    public static async Task<Result> TapErrorAsync(this Result result, Func<Error, Task> tap)
    {
        if (!result.TryGet(out var err))
        {
            await tap(err);
        }

        return result;
    }

    /// <summary>
    ///     Async TapError executing a side effect on failure with async function.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Async function to execute on failure.</param>
    /// <returns>A task with the original result unchanged.</returns>
    public static async Task<Result<TValue1>> TapErrorAsync<TValue1>(this Result<TValue1> result,
        Func<Error, Task> tap) where TValue1 : notnull
    {
        if (!result.TryGet(out Error? err))
        {
            await tap(err);
        }

        return result;
    }

    /// <param name="awaitableResult">The awaitable result instance.</param>
    extension(Task<Result> awaitableResult)
    {
        /// <summary>
        ///     Async TapError executing a side effect on an awaitable result with sync action.
        /// </summary>
        /// <param name="tap">Action to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async Task<Result> TapErrorAsync(Action<Error> tap)
        {
            var result = await awaitableResult;
            result.IfFailure(tap);
            return result;
        }

        /// <summary>
        ///     Async TapError executing a side effect on an awaitable result with async function.
        /// </summary>
        /// <param name="tap">Async function to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async Task<Result> TapErrorAsync(Func<Error, Task> tap)
        {
            var result = await awaitableResult;
            return await result.TapErrorAsync(tap);
        }
    }


    /// <param name="awaitableResult">The awaitable result instance.</param>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    extension<TValue1>(Task<Result<TValue1>> awaitableResult) where TValue1 : notnull
    {
        /// <summary>
        ///     Async TapError executing a side effect on an awaitable result with sync action.
        /// </summary>
        /// <param name="tap">Action to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async Task<Result<TValue1>> TapErrorAsync(Action<Error> tap)
        {
            var result = await awaitableResult;
            result.IfFailure(tap);
            return result;
        }

        /// <summary>
        ///     Async TapError executing a side effect on an awaitable result with async function.
        /// </summary>
        /// <param name="tap">Async function to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async Task<Result<TValue1>> TapErrorAsync(Func<Error, Task> tap)
        {
            var result = await awaitableResult;
            return await result.TapErrorAsync(tap);
        }
    }
}
