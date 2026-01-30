using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous tasks that yield results.
///     This static class is designed to simplify operations on tasks returning result types,
///     enabling composition and improved readability in functional programming contexts.
/// </summary>
public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Contains extension methods for the <see cref="ResultTask{TValue}" /> type, providing
    ///     functionality to retrieve or transform the result value using default values, fallback
    ///     mechanisms, or custom exception handling.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    extension<TValue>(ResultTask<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Returns the value of the <see cref="ResultTask{TValue}" /> if it is successful,
        ///     or the specified fallback value if it has failed.
        /// </summary>
        /// <param name="fallback">The fallback value to use if the result is a failure.</param>
        /// <returns>A <see cref="ValueTask{TValue}" /> containing the result value or the fallback value.</returns>
        public ValueTask<TValue> ValueOr(TValue fallback)
        {
            return ValueOrCore(result, fallback);

            static async ValueTask<TValue> ValueOrCore(ResultTask<TValue> self, TValue fallback)
            {
                var source = await self;
                return source.Match(value => value, _ => fallback);
            }
        }

        /// <summary>
        ///     Returns the value from the result task if successful, or the specified fallback value otherwise.
        /// </summary>
        /// <param name="fallback">The fallback value to return if the result is unsuccessful.</param>
        /// <returns>A <see cref="ValueTask{TResult}" /> containing the value or the fallback if unsuccessful.</returns>
        public ValueTask<TValue> ValueOr(ValueTask<TValue> fallback)
        {
            return ValueOrCore(result, fallback);

            static async ValueTask<TValue> ValueOrCore(ResultTask<TValue> self, ValueTask<TValue> fallback)
            {
                var source = await self;
                return await source.Match<ValueTask<TValue>>(value => ValueTask.FromResult(value),
                    _ => fallback);
            }
        }

        /// <summary>
        ///     Returns the value of the <see cref="ResultTask{TValue}" /> if it is successful,
        ///     or the value provided by the specified fallback factory if it has failed.
        /// </summary>
        /// <param name="fallbackFactory">
        ///     A factory function that provides the fallback value to return
        ///     if the result represents a failure.
        /// </param>
        /// <returns>
        ///     A <see cref="ValueTask{TValue}" /> containing the value from the result upon success,
        ///     or the fallback value provided by the factory if the result is a failure.
        /// </returns>
        public ValueTask<TValue> ValueOr(Func<TValue> fallbackFactory)
        {
            return ValueOrCore(result, fallbackFactory);

            static async ValueTask<TValue> ValueOrCore(ResultTask<TValue> self, Func<TValue> fallbackFactory)
            {
                var source = await self;
                return source.Match(value => value, _ => fallbackFactory());
            }
        }

        /// <summary>
        ///     Retrieves the value of the <see cref="ResultTask{TValue}" /> if it represents a successful result,
        ///     or invokes the specified fallback factory to obtain a fallback value if the result indicates a failure.
        /// </summary>
        /// <param name="fallbackFactory">A factory function that produces the fallback value if the result is a failure.</param>
        /// <returns>
        ///     A <see cref="ValueTask{TValue}" /> that contains either the successful result value
        ///     or the value provided by the fallback factory.
        /// </returns>
        public ValueTask<TValue> ValueOr(Func<ValueTask<TValue>> fallbackFactory)
        {
            return ValueOrCore(result, fallbackFactory);

            static async ValueTask<TValue> ValueOrCore(ResultTask<TValue> self,
                Func<ValueTask<TValue>> fallbackFactory)
            {
                var source = await self;
                return await source.Match<ValueTask<TValue>>(value => ValueTask.FromResult(value),
                    _ => fallbackFactory());
            }
        }

        /// <summary>
        ///     Returns the value contained within the <see cref="ResultTask{TValue}" /> if it is successful,
        ///     or the default value of the type <typeparamref name="TValue" /> if it is a failure.
        /// </summary>
        /// <returns>
        ///     A <see cref="ValueTask{TResult}" /> representing the value contained in the result if successful,
        ///     or the default value of type <typeparamref name="TValue" /> if the result indicates failure.
        /// </returns>
        public ValueTask<TValue?> ValueOrDefault()
        {
            return ValueOrDefaultCore(result);

            static async ValueTask<TValue?> ValueOrDefaultCore(ResultTask<TValue> self)
            {
                var source = await self;
                return source.TryGetValue(out var value)
                    ? value
                    : default;
            }
        }

        /// <summary>
        ///     Retrieves the successful value of the <see cref="ResultTask{TValue}" /> or throws an exception if the operation
        ///     failed.
        /// </summary>
        /// <returns>
        ///     A <see cref="ValueTask{TResult}" /> that resolves to the successful value if the result is successful.
        ///     Throws an exception if the result represents a failure.
        /// </returns>
        public ValueTask<TValue> ValueOrThrow()
        {
            return result.ValueOrThrow((Func<Failure, Exception>)Func);

            Exception Func(Failure failures)
            {
                throw failures.ToException();
            }
        }

        /// <summary>
        ///     Extracts the value from a successful <see cref="ResultTask{TValue}" /> or throws an exception
        ///     if the result represents a failure.
        /// </summary>
        /// <param name="exceptionFactory">
        ///     A function that maps the <see cref="Failure" /> from a failed result into an
        ///     <see cref="Exception" /> to be thrown.
        /// </param>
        /// <returns>
        ///     The value contained in a successful result.
        /// </returns>
        /// <exception cref="Exception">
        ///     Thrown if the result is a failure, using the exception provided by the specified
        ///     <paramref name="exceptionFactory" />.
        /// </exception>
        public ValueTask<TValue> ValueOrThrow(Func<Failure, Exception> exceptionFactory)
        {
            return ValueOrThrowCore(result, exceptionFactory);

            static async ValueTask<TValue> ValueOrThrowCore(ResultTask<TValue> self,
                Func<Failure, Exception> exceptionFactory)
            {
                var source = await self;
                return source.Match<TValue>(value => value, e => throw exceptionFactory(e));
            }
        }

        /// <summary>
        ///     Retrieves the value from a <see cref="ResultTask{TValue}" /> if successful; otherwise, throws an exception.
        /// </summary>
        /// <param name="exceptionFactory">
        ///     A factory function that takes an <see cref="Failure" /> and produces the exception to throw if the result is a
        ///     failure.
        /// </param>
        /// <returns>
        ///     A <see cref="ValueTask{TResult}" /> that resolves to the value contained in the successful result.
        /// </returns>
        /// <exception cref="Exception">
        ///     Thrown when the result is a failure and the <paramref name="exceptionFactory" /> produces an exception.
        /// </exception>
        public ValueTask<TValue> ValueOrThrow(Func<Failure, ValueTask<Exception>> exceptionFactory)
        {
            return ValueOrThrowCore(result, exceptionFactory);

            static async ValueTask<TValue> ValueOrThrowCore(ResultTask<TValue> self,
                Func<Failure, ValueTask<Exception>> exceptionFactory)
            {
                var source = await self;
                return await source.Match<ValueTask<TValue>>(
                    value => ValueTask.FromResult(value),
                    async e => throw await exceptionFactory(e));
            }
        }
    }
}