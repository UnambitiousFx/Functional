using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result to recover from.</param>
    /// <typeparam name="TValue1">The type of the first value.</typeparam>
    extension<TValue1>(Result<TValue1> result) where TValue1 : notnull
    {
        /// <summary>
        ///     Recovers from a failed result by providing fallback values through a recovery function.
        /// </summary>
        /// <param name="recover">A function that takes the errors and returns fallback values.</param>
        /// <returns>A successful result with the fallback values if the original result failed; otherwise, the original result.</returns>
        public Result<TValue1> Recover(Func<Error, TValue1> recover)
        {
            if (result.TryGet(out _, out var error))
            {
                return result;
            }

            var fallback = recover(error);
            return Result.Success(fallback);
        }

        /// <summary>
        ///     Recovers from a failed result by providing specific fallback values.
        /// </summary>
        /// <param name="fallback1">The fallback value for the first parameter.</param>
        /// <returns>A successful result with the fallback values if the original result failed; otherwise, the original result.</returns>
        public Result<TValue1> Recover(TValue1 fallback1) => result.Recover(_ => fallback1);
    }
}
