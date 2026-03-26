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

    extension<TIn>(Task<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Returns the contained value if present; otherwise returns the provided fallback value.
        /// </summary>
        /// <param name="fallback">The fallback value to use when None.</param>
        /// <returns>The contained value or the fallback.</returns>
        public ValueTask<TIn> ValueOr(TIn fallback) {
            return new ValueTask<TIn>(ValueOrCore(maybeTask, fallback));

            static async Task<TIn> ValueOrCore(Task<Maybe<TIn>> maybeTask,
                                               TIn              fallback) {
                return (await maybeTask).ValueOr(fallback);
            }
        }

        /// <summary>
        ///     Returns the contained value if present; otherwise returns the value created by the factory.
        /// </summary>
        /// <param name="fallbackFactory">The factory to create a fallback value when None.</param>
        /// <returns>The contained value or the fallback created by the factory.</returns>
        public ValueTask<TIn> ValueOr(Func<TIn> fallbackFactory) {
            return new ValueTask<TIn>(ValueOrCore(maybeTask, fallbackFactory));

            static async Task<TIn> ValueOrCore(Task<Maybe<TIn>> maybeTask,
                                               Func<TIn>        fallbackFactory) {
                return (await maybeTask).ValueOr(fallbackFactory);
            }
        }

        /// <summary>
        ///     Returns the contained value if present; otherwise returns the value created by the asynchronous factory.
        /// </summary>
        /// <param name="fallbackFactory">The asynchronous factory to create a fallback value when None.</param>
        /// <returns>The contained value or the fallback created by the asynchronous factory.</returns>
        public ValueTask<TIn> ValueOr(Func<ValueTask<TIn>> fallbackFactory) {
            return new ValueTask<TIn>(ValueOrCore(maybeTask, fallbackFactory));

            static async Task<TIn> ValueOrCore(Task<Maybe<TIn>>     maybeTask,
                                               Func<ValueTask<TIn>> fallbackFactory) {
                return await new ValueTask<Maybe<TIn>>(maybeTask).ValueOr(fallbackFactory);
            }
        }
    }
}
