namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Maybe operations.
/// </summary>
public static partial class MaybeAsyncExtensions {
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Transforms the value contained in the asynchronous maybe using the specified mapping function
        ///     and returns a new asynchronous maybe containing the transformed value. If no value is present,
        ///     the result remains an empty asynchronous maybe.
        /// </summary>
        /// <param name="map">
        ///     A function to transform the value of the maybe if present. The function takes a value of type
        ///     <typeparamref name="TIn" />
        ///     and returns a value of type <typeparamref name="TOut" />.
        /// </param>
        /// <returns>
        ///     A task representing the result of applying the mapping function to the value of
        ///     the maybe, or an empty maybe if no value was present.
        /// </returns>
        public async ValueTask<Maybe<TOut>> Map<TOut>(Func<TIn, TOut> map)
            where TOut : notnull {
            var maybe = await maybeTask;
            return maybe.Map(map);
        }

        /// <summary>
        ///     Transforms the value contained in the asynchronous maybe using the specified asynchronous mapping function
        ///     and returns a new asynchronous maybe containing the transformed value. If no value is present,
        ///     the result remains an empty asynchronous maybe.
        /// </summary>
        /// <param name="map">
        ///     An asynchronous function to transform the value of the maybe if present. The function takes a value of type
        ///     <typeparamref name="TIn" /> and returns a task with a value of type <typeparamref name="TOut" />.
        /// </param>
        /// <returns>
        ///     A task representing the result of applying the asynchronous mapping function to
        ///     the value of the maybe, or an empty maybe if no value was present.
        /// </returns>
        public async ValueTask<Maybe<TOut>> Map<TOut>(Func<TIn, ValueTask<TOut>> map)
            where TOut : notnull {
            var maybe = await maybeTask;
            if (!maybe.Some(out var value)) {
                return Maybe.None<TOut>();
            }

            return Maybe.Some(await map(value));
        }
    }
}
