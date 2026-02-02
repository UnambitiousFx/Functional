namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with tasks that return results.
/// </summary>
public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Provides extensions for working with <see cref="ResultTask" /> to chain operations based on success or failure.
    /// </summary>
    extension(ResultTask result)
    {
        /// <summary>
        ///     Executes the specified function if the <see cref="ResultTask" /> is successful.
        ///     Otherwise, propagates the error.
        /// </summary>
        /// <param name="func">
        ///     A function to be executed if the <see cref="ResultTask" /> completes successfully.
        /// </param>
        /// <returns>
        ///     A new <see cref="ResultTask" /> where the specified function has been executed if the original
        ///     task was successful, or the existing error if it was not.
        /// </returns>
        public ResultTask Then(Func<Result> func)
        {
            return ThenCore(result, func).AsAsync();

            static async ValueTask<Result> ThenCore(ResultTask self, Func<Result> func)
            {
                var source = await self;
                if (source.IsFailure)
                {
                    return source;
                }

                return func();
            }
        }

        /// <summary>
        ///     Executes the provided function if the <see cref="ResultTask" /> is successful,
        ///     propagates any failure result, and wraps the result in a new <see cref="ResultTask" />.
        /// </summary>
        /// <param name="func">
        ///     The function to execute if the <see cref="ResultTask" /> is successful.
        ///     The function should return a <see cref="ResultTask" />.
        /// </param>
        /// <returns>
        ///     A new <see cref="ResultTask" /> that represents the result of the executed function
        ///     if successful, or the propagated failure result.
        /// </returns>
        public ResultTask Then(Func<ResultTask> func)
        {
            return ThenCore(result, func).AsAsync();

            static async ValueTask<Result> ThenCore(ResultTask self, Func<ResultTask> func)
            {
                var source = await self;
                if (source.IsFailure)
                {
                    return source;
                }

                return await func();
            }
        }
    }

    /// <summary>
    ///     Provides extension methods for handling operations on <see cref="ResultTask{TValue}" />.
    /// </summary>
    extension<TValue>(ResultTask<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Chains a computation to be executed if the result is successful.
        /// </summary>
        /// <param name="then">A function that takes the successful value and returns a new result.</param>
        /// <returns>
        ///     A new result task representing the chained computation, or the original failure if the initial computation is
        ///     unsuccessful.
        /// </returns>
        public ResultTask<TValue> Then(Func<TValue, Result<TValue>> then)
        {
            return ThenCore(result, then).AsAsync();

            static async ValueTask<Result<TValue>> ThenCore(ResultTask<TValue> self, Func<TValue, Result<TValue>> then)
            {
                var source = await self;
                if (!source.TryGetValue(out var value))
                {
                    return source;
                }

                var response = then(value);
                return response.WithMetadata(source.Metadata);
            }
        }

        /// <summary>
        ///     Executes a follow-up asynchronous operation if the current <see cref="ResultTask{TValue}" /> represents a
        ///     successful result.
        /// </summary>
        /// <param name="then">
        ///     A function to be executed if the current result is successful. The function takes the result value
        ///     as input and returns a new asynchronous result operation encapsulated in a <see cref="ResultTask{TValue}" />.
        /// </param>
        /// <returns>
        ///     A <see cref="ResultTask{TValue}" /> representing the result of the asynchronous operation,
        ///     containing either the new computed value or propagating the original error.
        /// </returns>
        public ResultTask<TValue> Then(Func<TValue, ResultTask<TValue>> then)
        {
            return ThenCore(result, then).AsAsync();

            static async ValueTask<Result<TValue>> ThenCore(ResultTask<TValue> self,
                Func<TValue, ResultTask<TValue>> then)
            {
                var source = await self;
                if (!source.TryGetValue(out var value))
                {
                    return source;
                }

                var response = await then(value);
                return response.WithMetadata(source.Metadata);
            }
        }

        /// <summary>
        ///     Chains the current <see cref="ResultTask{TValue}" /> with a subsequent operation that
        ///     transforms a successful result into a new <see cref="Result" />.
        /// </summary>
        /// <param name="then">
        ///     A function that takes the successful result value as input and returns a
        ///     new <see cref="Result" />.
        /// </param>
        /// <returns>
        ///     A new <see cref="ResultTask{TValue}" /> that represents the result of applying the <paramref name="then" />
        ///     function to the successful result value of the original task, or propagates the failure if the
        ///     original task failed.
        /// </returns>
        public ResultTask<TValue> Then(Func<TValue, Result> then)
        {
            return ThenCore(result, then).AsAsync();

            static async ValueTask<Result<TValue>> ThenCore(ResultTask<TValue> self, Func<TValue, Result> then)
            {
                var source = await self;
                if (!source.TryGetValue(out var value))
                {
                    return source;
                }

                var thenResult = then(value);
                if (!thenResult.TryGetError(out var error))
                {
                    return source;
                }

                var failResult = Result.Failure<TValue>(error);
                return failResult.WithMetadata(source.Metadata);
            }
        }

        /// <summary>
        ///     Executes the specified function if the current <see cref="ResultTask{TValue}" /> is successful.
        ///     Propagates any pre-existing error or error returned from the given function.
        /// </summary>
        /// <param name="then">A function to execute on the result value if the result was successful.</param>
        /// <returns>
        ///     A new <see cref="ResultTask{TValue}" /> that represents the result of the operation.
        ///     Contains the original result value if successful, or contains an error if the operation fails.
        /// </returns>
        public ResultTask<TValue> Then(Func<TValue, ResultTask> then)
        {
            return ThenCore(result, then).AsAsync();

            static async ValueTask<Result<TValue>> ThenCore(ResultTask<TValue> self, Func<TValue, ResultTask> then)
            {
                var source = await self;
                if (!source.TryGetValue(out var value))
                {
                    return source;
                }

                var thenResult = await then(value);
                if (!thenResult.TryGetError(out var error))
                {
                    return source;
                }

                var failResult = Result.Failure<TValue>(error);
                return failResult.WithMetadata(source.Metadata);
            }
        }
    }
}