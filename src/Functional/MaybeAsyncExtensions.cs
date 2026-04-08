using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous operations on the <see cref="Maybe{TValue}" /> type.
///     These extensions enable mapping, binding, tapping, switching, and conversion to <see cref="Result{TValue}" />
///     for <see cref="Task" /> and <see cref="ValueTask" /> contexts encapsulating <see cref="Maybe{TValue}" /> values.
/// </summary>
/// The type of the value wrapped by the
/// <see cref="Maybe{TValue}" />
/// instance within the
/// asynchronous operation.
public static partial class MaybeExtensions {
    /// <summary>
    ///     Provides extension methods for handling asynchronous operations on the <see cref="Maybe{TValue}" /> type.
    ///     These methods allow mapping, binding, tapping, converting to a result, and switching actions
    ///     on <see cref="ValueTask{TResult}" /> encapsulating <see cref="Maybe{TValue}" /> values.
    /// </summary>
    /// The type of the value contained within the
    /// <see cref="Maybe{TValue}" />
    /// in the asynchronous
    /// operation.
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Converts a <see cref="Maybe{TValue}" /> to a <see cref="Result{TValue}" /> using the provided failure
        ///     if the <see cref="Maybe{TValue}" /> has no value.
        /// </summary>
        /// <param name="failure">
        ///     The failure to assign to the result if the <see cref="Maybe{TValue}" /> has no value.
        /// </param>
        /// <returns>
        ///     A <see cref="Result{TValue}" /> that contains the value if present or the specified failure if the value is absent.
        /// </returns>
        public async ValueTask<Result<TIn>> ToResult(Failure failure) {
            var maybe = await maybeTask;
            return maybe.ToResult(failure);
        }

        /// <summary>
        ///     Converts a <see cref="Maybe{TValue}" /> to a <see cref="Result{TValue}" /> using a failure created by the factory
        ///     if the <see cref="Maybe{TValue}" /> has no value.
        /// </summary>
        /// <param name="errorFactory">
        ///     The factory function to create a failure if the <see cref="Maybe{TValue}" /> has no value.
        /// </param>
        /// <returns>
        ///     A <see cref="Result{TValue}" /> that contains the value if present or the failure from the factory if the value is
        ///     absent.
        /// </returns>
        public async ValueTask<Result<TIn>> ToResult(Func<Failure> errorFactory) {
            var maybe = await maybeTask;
            return maybe.ToResult(errorFactory);
        }

        /// <summary>
        ///     Converts a <see cref="Maybe{TValue}" /> to a <see cref="Result{TValue}" /> using the provided error message
        ///     if the <see cref="Maybe{TValue}" /> has no value.
        /// </summary>
        /// <param name="message">
        ///     The error message to use if the <see cref="Maybe{TValue}" /> has no value.
        /// </param>
        /// <returns>
        ///     A <see cref="Result{TValue}" /> that contains the value if present or a failure with the message if the value is
        ///     absent.
        /// </returns>
        public async ValueTask<Result<TIn>> ToResult(string message) {
            var maybe = await maybeTask;
            return maybe.ToResult(message);
        }

        /// <summary>
        ///     Converts a <see cref="Maybe{TValue}" /> to a <see cref="Result{TValue}" /> using a failure created by the
        ///     asynchronous factory
        ///     if the <see cref="Maybe{TValue}" /> has no value.
        /// </summary>
        /// <param name="errorFactory">
        ///     The asynchronous factory function to create a failure if the <see cref="Maybe{TValue}" /> has no value.
        /// </param>
        /// <returns>
        ///     A <see cref="Result{TValue}" /> that contains the value if present or the failure from the asynchronous factory if
        ///     the value is absent.
        /// </returns>
        public async ValueTask<Result<TIn>> ToResult(Func<ValueTask<Failure>> errorFactory) {
            var maybe = await maybeTask;
            if (maybe.Some(out var value)) {
                return Result.Success(value);
            }

            return Result.Failure<TIn>(await errorFactory());
        }
    }
}
