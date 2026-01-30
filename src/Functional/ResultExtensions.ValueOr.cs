using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue">Value type 1.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Returns contained values when successful; otherwise provided fallback(s).
        /// </summary>
        /// <param name="fallback1">Fallback value 1.</param>
        /// <returns>The value(s) or fallback(s).</returns>
        public TValue ValueOr(TValue fallback1)
        {
            return result.Match<TValue>(value1 => value1, _ => fallback1);
        }

        /// <summary>
        ///     Returns contained values when successful; otherwise value(s) from factory.
        /// </summary>
        /// <param name="fallbackFactory">Factory producing fallback value(s).</param>
        /// <returns>The value(s) or factory value(s).</returns>
        public TValue ValueOr(Func<TValue> fallbackFactory)
        {
            return result.Match<TValue>(value1 => value1, _ => fallbackFactory());
        }

        /// <summary>
        ///     Returns the contained value when successful; otherwise the default value of the type.
        /// </summary>
        /// <returns>The contained value or the default value of the type.</returns>
        public TValue? ValueOrDefault()
        {
            return result.TryGetValue(out var value)
                ? value
                : default;
        }

        /// <summary>
        ///     Returns contained value(s); throws aggregated exception when failure.
        /// </summary>
        /// <returns>The value(s) or throws.</returns>
        public TValue ValueOrThrow()
        {
            return result.ValueOrThrow(errors => throw errors.ToException());
        }

        /// <summary>
        ///     Returns contained value(s); otherwise throws exception from factory.
        /// </summary>
        /// <param name="exceptionFactory">Factory creating exception from errors.</param>
        /// <returns>The value(s) or throws the custom exception.</returns>
        public TValue ValueOrThrow(Func<Failure, Exception> exceptionFactory)
        {
            return result.Match<TValue>(value1 => value1, e => throw exceptionFactory(e));
        }
    }
}