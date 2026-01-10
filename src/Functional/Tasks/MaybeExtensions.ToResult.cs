using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tasks;

public static partial class MaybeExtensions
{
    /// <param name="maybeTask">The Task-wrapped maybe instance.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    extension<TValue>(Task<Maybe<TValue>> maybeTask) where TValue : notnull
    {
        /// <summary>
        ///     Converts a Task-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with the provided error.
        /// </summary>
        /// <param name="error">The error to use when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided error.</returns>
        public async Task<Result<TValue>> ToResult(Error error)
        {
            var maybe = await maybeTask;
            return maybe.ToResult(error);
        }

        /// <summary>
        ///     Converts a Task-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with an error created by the factory function.
        /// </summary>
        /// <param name="errorFactory">Factory function to create an error when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the error from the factory.</returns>
        public async Task<Result<TValue>> ToResult(Func<Error> errorFactory)
        {
            var maybe = await maybeTask;
            return maybe.ToResult(errorFactory);
        }

        /// <summary>
        ///     Converts a Task-wrapped Maybe to a Result. If the Maybe is Some, returns Success with the value.
        ///     If the Maybe is None, returns Failure with an error containing the provided message.
        /// </summary>
        /// <param name="message">The error message to use when the maybe is None.</param>
        /// <returns>A Result that is successful if the maybe is Some; otherwise a failure with the provided message.</returns>
        public async Task<Result<TValue>> ToResult(string message)
        {
            var maybe = await maybeTask;
            return maybe.ToResult(message);
        }
    }
}
