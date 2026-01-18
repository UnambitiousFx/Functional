using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.ValueTasks;

public partial class ResultExtensions
{
    /// <param name="awaitableResult">The awaitable result to compensate.</param>
    extension(ValueTask<Result> awaitableResult)
    {
        /// <summary>
        ///     Asynchronously attempts to compensate for a failure by executing a synchronous rollback function.
        ///     If the rollback also fails, both errors are combined into an <see cref="AggregateError" />.
        /// </summary>
        /// <param name="rollback">The synchronous rollback function to execute on failure, receiving the original error.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The result is the original result if successful;
        ///     the original result if rollback succeeds; or a failure with an <see cref="AggregateError" /> containing both errors
        ///     if rollback fails.
        /// </returns>
        public async ValueTask<Result> CompensateAsync(Func<Error, Result> rollback)
        {
            var result = await awaitableResult;
            return result.Compensate(rollback);
        }

        /// <summary>
        ///     Asynchronously attempts to compensate for a failure by executing an asynchronous rollback function.
        ///     If the rollback also fails, both errors are combined into an <see cref="AggregateError" />.
        /// </summary>
        /// <param name="rollback">The asynchronous rollback function to execute on failure, receiving the original error.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The result is the original result if successful;
        ///     the original result if rollback succeeds; or a failure with an <see cref="AggregateError" /> containing both errors
        ///     if rollback fails.
        /// </returns>
        public async ValueTask<Result> CompensateAsync(Func<Error, ValueTask<Result>> rollback)
        {
            var result = await awaitableResult;
            return await result.Match<ValueTask<Result>>(
                () => ValueTask.FromResult(result),
                async originalError =>
                {
                    var rollbackResult = await rollback(originalError);

                    return rollbackResult.Match(
                        () => result,
                        rollbackError => Result.Failure(new AggregateError(originalError, rollbackError)));
                });
        }
    }


    /// <param name="awaitableResult">The awaitable result to compensate.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(ValueTask<Result<TValue>> awaitableResult) where TValue : notnull
    {
        /// <summary>
        ///     Asynchronously attempts to compensate for a failure by executing a synchronous rollback function.
        ///     If the rollback also fails, both errors are combined into an <see cref="AggregateError" />.
        /// </summary>
        /// <param name="rollback">The synchronous rollback function to execute on failure, receiving the original error.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The result is the original result if successful;
        ///     the original result if rollback succeeds; or a failure with an <see cref="AggregateError" /> containing both errors
        ///     if rollback fails.
        /// </returns>
        public async ValueTask<Result<TValue>> CompensateAsync(Func<Error, Result> rollback)
        {
            var result = await awaitableResult;
            return result.Compensate(rollback);
        }

        /// <summary>
        ///     Asynchronously attempts to compensate for a failure by executing an asynchronous rollback function.
        ///     If the rollback also fails, both errors are combined into an <see cref="AggregateError" />.
        /// </summary>
        /// <param name="rollback">The asynchronous rollback function to execute on failure, receiving the original error.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The result is the original result if successful;
        ///     the original result if rollback succeeds; or a failure with an <see cref="AggregateError" /> containing both errors
        ///     if rollback fails.
        /// </returns>
        public async ValueTask<Result<TValue>> CompensateAsync(Func<Error, ValueTask<Result>> rollback)
        {
            var result = await awaitableResult;
            return await result.Match<ValueTask<Result<TValue>>>(
                () => ValueTask.FromResult(result),
                async originalError =>
                {
                    var rollbackResult = await rollback(originalError);

                    return rollbackResult.Match(
                        () => result,
                        rollbackError => Result.Failure<TValue>(new AggregateError(originalError, rollbackError)));
                });
        }
    }
}