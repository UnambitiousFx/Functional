using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    extension<TValue1>(Result<TValue1> result) where TValue1 : notnull
    {
        /// <summary>
        ///     Returns contained value(s); throws aggregated exception when failure.
        /// </summary>
        /// <returns>The value(s) or throws.</returns>
        public TValue1 ValueOrThrow()
        {
            return result.ValueOrThrow(errors => throw errors.ToException());
        }

        /// <summary>
        ///     Returns contained value(s); otherwise throws exception from factory.
        /// </summary>
        /// <param name="exceptionFactory">Factory creating exception from errors.</param>
        /// <returns>The value(s) or throws the custom exception.</returns>
        public TValue1 ValueOrThrow(Func<Error, Exception> exceptionFactory)
        {
            return result.Match<TValue1>(value1 => value1, e => throw exceptionFactory(e));
        }
    }
}