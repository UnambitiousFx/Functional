namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Maybe operations.
/// </summary>
public static partial class MaybeAsyncExtensions {
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Filters the asynchronous maybe value using a predicate, returning None when the predicate fails.
        /// </summary>
        /// <param name="predicate">The predicate to apply to the contained value.</param>
        /// <returns>
        ///     A task containing the original maybe if the predicate holds; otherwise None.
        /// </returns>
        public async ValueTask<Maybe<TIn>> Filter(Func<TIn, bool> predicate) {
            var maybe = await maybeTask;
            return maybe.Filter(predicate);
        }

        /// <summary>
        ///     Filters the asynchronous maybe value using an asynchronous predicate, returning None when the predicate fails.
        /// </summary>
        /// <param name="predicate">The asynchronous predicate to apply to the contained value.</param>
        /// <returns>
        ///     A task containing the original maybe if the predicate holds; otherwise None.
        /// </returns>
        public async ValueTask<Maybe<TIn>> Filter(Func<TIn, ValueTask<bool>> predicate) {
            var maybe = await maybeTask;
            if (!maybe.Some(out var value)) {
                return maybe;
            }

            return await predicate(value)
                       ? maybe
                       : Maybe.None<TIn>();
        }
    }
}
