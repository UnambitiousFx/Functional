namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Maybe operations.
/// </summary>
public static partial class MaybeAsyncExtensions {
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Returns the current Maybe if it is Some; otherwise returns the fallback value.
        /// </summary>
        /// <param name="fallback">The fallback Maybe to use when None.</param>
        /// <returns>The current Maybe if Some; otherwise the fallback.</returns>
        public async ValueTask<Maybe<TIn>> OrElse(Maybe<TIn> fallback) {
            var maybe = await maybeTask;
            return maybe.OrElse(fallback);
        }

        /// <summary>
        ///     Returns the current Maybe if it is Some; otherwise returns the fallback created by the factory.
        /// </summary>
        /// <param name="fallbackFactory">The factory to create a fallback Maybe when None.</param>
        /// <returns>The current Maybe if Some; otherwise the fallback from the factory.</returns>
        public async ValueTask<Maybe<TIn>> OrElse(Func<Maybe<TIn>> fallbackFactory) {
            var maybe = await maybeTask;
            return maybe.OrElse(fallbackFactory);
        }

        /// <summary>
        ///     Returns the current Maybe if it is Some; otherwise returns the fallback created by the asynchronous factory.
        /// </summary>
        /// <param name="fallbackFactory">The asynchronous factory to create a fallback Maybe when None.</param>
        /// <returns>The current Maybe if Some; otherwise the fallback from the asynchronous factory.</returns>
        public async ValueTask<Maybe<TIn>> OrElse(Func<ValueTask<Maybe<TIn>>> fallbackFactory) {
            var maybe = await maybeTask;
            if (maybe.IsSome) {
                return maybe;
            }

            return await fallbackFactory();
        }
    }
}
