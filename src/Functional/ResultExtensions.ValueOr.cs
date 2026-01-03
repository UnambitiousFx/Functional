namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    extension<TValue1>(Result<TValue1> result) where TValue1 : notnull
    {
        /// <summary>
        ///     Returns contained values when successful; otherwise provided fallback(s).
        /// </summary>
        /// <param name="fallback1">Fallback value 1.</param>
        /// <returns>The value(s) or fallback(s).</returns>
        public TValue1 ValueOr(TValue1 fallback1)
        {
            return result.Match<TValue1>(value1 => value1, _ => fallback1);
        }

        /// <summary>
        ///     Returns contained values when successful; otherwise value(s) from factory.
        /// </summary>
        /// <param name="fallbackFactory">Factory producing fallback value(s).</param>
        /// <returns>The value(s) or factory value(s).</returns>
        public TValue1 ValueOr(Func<TValue1> fallbackFactory)
        {
            return result.Match<TValue1>(value1 => value1, _ => fallbackFactory());
        }
    }
}