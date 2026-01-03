namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Async Then chaining an async transformation that returns a Result of the same type.
    /// </summary>
    /// <typeparam name="T1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="then">The async transformation function.</param>
    /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
    /// <returns>A task with the new result.</returns>
    public static async ValueTask<Result<T1>> ThenAsync<T1>(this Result<T1> result,
        Func<T1, ValueTask<Result<T1>>> then, bool copyReasonsAndMetadata = true) where T1 : notnull
    {
        if (!result.TryGet(out T1? value))
        {
            return result;
        }

        var response = await then(value).ConfigureAwait(false);
        if (copyReasonsAndMetadata)
        {
            response = response.WithMetadata(result.Metadata);
        }

        return response;
    }

    /// <param name="awaitableResult">The awaitable result instance.</param>
    /// <typeparam name="T1">Value type 1.</typeparam>
    extension<T1>(ValueTask<Result<T1>> awaitableResult) where T1 : notnull
    {
        /// <summary>
        ///     Async Then awaiting result then chaining a sync transformation.
        /// </summary>
        /// <param name="then">The transformation function.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the new result.</returns>
        public async ValueTask<Result<T1>> ThenAsync(Func<T1, Result<T1>> then, bool copyReasonsAndMetadata = true)
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return result.Then(then, copyReasonsAndMetadata);
        }

        /// <summary>
        ///     Async Then awaiting result then chaining an async transformation.
        /// </summary>
        /// <param name="then">The async transformation function.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the new result.</returns>
        public async ValueTask<Result<T1>> ThenAsync(Func<T1, ValueTask<Result<T1>>> then,
            bool copyReasonsAndMetadata = true)
        {
            var result = await awaitableResult.ConfigureAwait(false);
            return await result.ThenAsync(then, copyReasonsAndMetadata).ConfigureAwait(false);
        }
    }
}
