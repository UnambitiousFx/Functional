using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result to recover from.</param>
    /// <typeparam name="TValue">The type of the first value.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Recovers from a failed result by providing fallback values through a recovery function.
        /// </summary>
        /// <param name="recoverFunc">A function that takes the errors and returns fallback values.</param>
        /// <returns>A successful result with the fallback values if the original result failed; otherwise, the original result.</returns>
        public Result<TValue> Recover(Func<Failure, TValue> recoverFunc)
        {
            if (!result.TryGetError(out var error))
            {
                return result;
            }

            var fallback = recoverFunc(error);
            return Result.Success(fallback);
        }

        /// <summary>
        ///     Recovers from a failed result by providing specific fallback values.
        /// </summary>
        /// <param name="fallbackFunc">The fallback value for the first parameter.</param>
        /// <returns>A successful result with the fallback values if the original result failed; otherwise, the original result.</returns>
        public Result<TValue> Recover(TValue fallbackFunc)
        {
            return result.Recover(_ => fallbackFunc);
        }
    }
}