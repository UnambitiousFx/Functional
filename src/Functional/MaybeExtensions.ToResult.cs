using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <param name="maybe">The maybe instance.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    extension<TValue>(Maybe<TValue> maybe) where TValue : notnull
    {
        /// <summary>
        ///     Converts a Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with the provided error.
        /// </summary>
        /// <param name="failure">The error to use when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided error.</returns>
        public Result<TValue> ToResult(Failures.Failure failure)
        {
            return maybe.Match(
                some: Result.Success,
                none: () => Result.Failure<TValue>(failure));
        }

        /// <summary>
        ///     Converts a Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with an error created by the factory function.
        /// </summary>
        /// <param name="errorFactory">Factory function to create an error when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the error from the factory.</returns>
        public Result<TValue> ToResult(Func<Failure> errorFactory)
        {
            return maybe.Match(
                some: Result.Success,
                none: () => Result.Failure<TValue>(errorFactory()));
        }

        /// <summary>
        ///     Converts a Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with an error containing the provided message.
        /// </summary>
        /// <param name="message">The error message to use when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided message.</returns>
        public Result<TValue> ToResult(string message)
        {
            return maybe.Match(
                some: Result.Success,
                none: () => Result.Failure<TValue>(message));
        }
    }
}
