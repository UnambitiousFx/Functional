using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <param name="result">The result to recover from.</param>
    /// <typeparam name="TValue">The type of the first value.</typeparam>
    extension<TValue>(ResultTask<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Recovers from a failed result by providing fallback values through a recovery function.
        /// </summary>
        /// <param name="recoverFunc">A function that takes the error and returns a fallback value.</param>
        /// <returns>A successful result with the fallback value if the original result failed; otherwise, the original result.</returns>
        public ResultTask<TValue> Recover(Func<Failure, TValue> recoverFunc)
        {
            return RecoverCore(result, recoverFunc).AsAsync();

            static async ValueTask<Result<TValue>> RecoverCore(ResultTask<TValue> self, Func<Failure, TValue> recoverFunc)
            {
                var source = await self;
                if (!source.TryGetError(out var error))
                {
                    return source;
                }

                var fallback = recoverFunc(error);
                return Result.Success(fallback);
            }
        }

        /// <summary>
        ///     Recovers from a failed result by providing fallback values through a recovery function.
        /// </summary>
        /// <param name="recoverFunc">
        ///     A function that takes the error and returns fallback values as a
        ///     <see cref="ValueTask{TResult}" />.
        /// </param>
        /// <returns>A successful result with the fallback values if the original result failed; otherwise, the original result.</returns>
        public ResultTask<TValue> Recover(Func<Failure, ValueTask<TValue>> recoverFunc)
        {
            return RecoverCore(result, recoverFunc).AsAsync();

            static async ValueTask<Result<TValue>> RecoverCore(ResultTask<TValue> self,
                Func<Failure, ValueTask<TValue>> recoverFunc)
            {
                var source = await self;
                if (!source.TryGetError(out var error))
                {
                    return source;
                }

                var fallback = await recoverFunc(error);
                return Result.Success(fallback);
            }
        }

        /// <summary>
        ///     Recovers from a failed result by providing specific fallback values or functions that generate fallback values.
        /// </summary>
        /// <param name="recoverValue">
        ///     A function that takes the error and returns a fallback value.
        /// </param>
        /// <returns>
        ///     A successful result containing the fallback value if the original result failed;
        ///     otherwise, the original result.
        /// </returns>
        public ResultTask<TValue> Recover(TValue recoverValue)
        {
            return result.Recover(_ => recoverValue);
        }

        /// <summary>
        ///     Recovers from a failed result by providing fallback values or functions.
        /// </summary>
        /// <param name="recoverValue">A function that takes the error and returns fallback values.</param>
        /// <returns>A successful result with the fallback values if the original result failed; otherwise, the original result.</returns>
        public ResultTask<TValue> Recover(ValueTask<TValue> recoverValue)
        {
            return RecoverCore(result, recoverValue).AsAsync();

            static async ValueTask<Result<TValue>> RecoverCore(ResultTask<TValue> self,
                ValueTask<TValue> recoverValue)
            {
                var source = await self;
                if (!source.TryGetError(out var error))
                {
                    return source;
                }

                var fallback = await recoverValue;
                return Result.Success(fallback);
            }
        }
    }
}