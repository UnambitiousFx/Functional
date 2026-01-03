using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async TapBoth executing different side effects with async functions.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="onSuccess">Async function to execute on success.</param>
    /// <param name="onFailure">Async function to execute on failure.</param>
    /// <returns>A task with the original result unchanged.</returns>
    public static async ValueTask<Result> TapBothAsync(this Result result,
        Func<ValueTask> onSuccess,
        Func<Error, ValueTask> onFailure)
    {
        if (result.TryGet(out var error))
        {
            await onSuccess();
        }
        else
        {
            await onFailure(error);
        }

        return result;
    }

    /// <summary>
    ///     Async TapBoth executing different side effects with async functions.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="onSuccess">Async function to execute on success.</param>
    /// <param name="onFailure">Async function to execute on failure.</param>
    /// <returns>A task with the original result unchanged.</returns>
    public static async ValueTask<Result<TValue1>> TapBothAsync<TValue1>(this Result<TValue1> result,
        Func<TValue1, ValueTask> onSuccess, Func<Error, ValueTask> onFailure) where TValue1 : notnull
    {
        if (result.TryGet(out var value1, out var err))
        {
            await onSuccess(value1);
        }
        else
        {
            await onFailure(err);
        }

        return result;
    }

    /// <param name="awaitableResult">The awaitable result instance.</param>
    extension(ValueTask<Result> awaitableResult)
    {
        /// <summary>
        ///     Async TapBoth executing different side effects on an awaitable result with sync actions.
        /// </summary>
        /// <param name="onSuccess">Action to execute on success.</param>
        /// <param name="onFailure">Action to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result> TapBothAsync(Action onSuccess,
            Action<Error> onFailure)
        {
            var result = await awaitableResult;
            result.Match(onSuccess, onFailure);
            return result;
        }

        /// <summary>
        ///     Async TapBoth executing different side effects on an awaitable result with async functions.
        /// </summary>
        /// <param name="onSuccess">Async function to execute on success.</param>
        /// <param name="onFailure">Async function to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result> TapBothAsync(Func<ValueTask> onSuccess, Func<Error, ValueTask> onFailure)
        {
            var result = await awaitableResult;
            return await result.TapBothAsync(onSuccess, onFailure);
        }
    }


    /// <param name="awaitableResult">The awaitable result instance.</param>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    extension<TValue1>(ValueTask<Result<TValue1>> awaitableResult) where TValue1 : notnull
    {
        /// <summary>
        ///     Async TapBoth executing different side effects on an awaitable result with sync actions.
        /// </summary>
        /// <param name="onSuccess">Action to execute on success.</param>
        /// <param name="onFailure">Action to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result<TValue1>> TapBothAsync(Action<TValue1> onSuccess,
            Action<Error> onFailure)
        {
            var result = await awaitableResult;
            result.Match(onSuccess, onFailure);
            return result;
        }

        /// <summary>
        ///     Async TapBoth executing different side effects on an awaitable result with async functions.
        /// </summary>
        /// <param name="onSuccess">Async function to execute on success.</param>
        /// <param name="onFailure">Async function to execute on failure.</param>
        /// <returns>A task with the original result unchanged.</returns>
        public async ValueTask<Result<TValue1>> TapBothAsync(Func<TValue1, ValueTask> onSuccess,
            Func<Error, ValueTask> onFailure)
        {
            var result = await awaitableResult;
            return await result.TapBothAsync(onSuccess, onFailure);
        }
    }
}
