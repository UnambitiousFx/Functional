namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Maybe operations.
/// </summary>
public static partial class MaybeAsyncExtensions {
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Returns the contained value if present; otherwise returns the provided fallback value.
        /// </summary>
        /// <param name="fallback">The fallback value to use when None.</param>
        /// <returns>The contained value or the fallback.</returns>
        public async ValueTask<TIn> ValueOr(TIn fallback) {
            var maybe = await maybeTask;
            return maybe.ValueOr(fallback);
        }

        /// <summary>
        ///     Returns the contained value if present; otherwise returns the value created by the factory.
        /// </summary>
        /// <param name="fallbackFactory">The factory to create a fallback value when None.</param>
        /// <returns>The contained value or the fallback created by the factory.</returns>
        public async ValueTask<TIn> ValueOr(Func<TIn> fallbackFactory) {
            var maybe = await maybeTask;
            return maybe.ValueOr(fallbackFactory);
        }

        /// <summary>
        ///     Returns the contained value if present; otherwise returns the value created by the asynchronous factory.
        /// </summary>
        /// <param name="fallbackFactory">The asynchronous factory to create a fallback value when None.</param>
        /// <returns>The contained value or the fallback created by the asynchronous factory.</returns>
        public async ValueTask<TIn> ValueOr(Func<ValueTask<TIn>> fallbackFactory) {
            var maybe = await maybeTask;
            if (maybe.Some(out var value)) {
                return value;
            }

            return await fallbackFactory();
        }
    }
}
