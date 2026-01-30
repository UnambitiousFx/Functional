using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class MaybeTaskExtensions
{
    /// <param name="maybeTask">The ValueTask-wrapped maybe instance.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    extension<TValue>(MaybeTask<TValue> maybeTask) where TValue : notnull
    {
        /// <summary>
        ///     Converts a ValueTask-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with the provided error.
        /// </summary>
        /// <param name="failure">The error to use when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided error.</returns>
        public ResultTask<TValue> ToResult(Failure failure)
        {
            return ToResultCore(maybeTask, failure).AsAsync();

            static async ValueTask<Result<TValue>> ToResultCore(MaybeTask<TValue> maybeTask, Failure failure)
            {
                var maybe = await maybeTask;
                return maybe.ToResult(failure);
            }
        }

        /// <summary>
        ///     Converts a ValueTask-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with an error created by the factory function.
        /// </summary>
        /// <param name="errorFactory">Factory function to create an error when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the error from the factory.</returns>
        public ResultTask<TValue> ToResult(Func<Failure> errorFactory)
        {
            return ToResultCore(maybeTask, errorFactory)
                .AsAsync();

            static async ValueTask<Result<TValue>> ToResultCore(MaybeTask<TValue> maybeTask, Func<Failure> errorFactory)
            {
                var maybe = await maybeTask;
                return maybe.ToResult(errorFactory);
            }
        }

        /// <summary>
        ///     Converts a ValueTask-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with an error containing the provided message.
        /// </summary>
        /// <param name="message">The error message to use when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided message.</returns>
        public ResultTask<TValue> ToResult(string message)
        {
            return ToResultCore(maybeTask, message)
                .AsAsync();

            static async ValueTask<Result<TValue>> ToResultCore(MaybeTask<TValue> maybeTask, string message)
            {
                var maybe = await maybeTask;
                return maybe.ToResult(message);
            }
        }
    }
}