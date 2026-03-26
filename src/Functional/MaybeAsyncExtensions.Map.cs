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

    extension<TIn>(Task<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Transforms the value inside the <see cref="Maybe{TValue}" /> asynchronously using the specified mapping function
        ///     and returns a new <see cref="Maybe{TValue}" /> containing the result of the transformation.
        /// </summary>
        /// <typeparam name="TOut">
///     
/// </typeparam>
        ///     The type of the value to be returned by the mapping function, which will be encapsulated
        ///     in the resulting <see cref="Maybe{TValue}" />.
        /// <param name="map">
        ///     A function to transform the value from the source <see cref="Maybe{TValue}" /> into another value.
        /// </param>
        /// <returns>
        ///     A task representing the asynchronous operation, which results in a new <see cref="Maybe{TValue}" />
        ///     containing the transformed value if present, or the original empty state otherwise.
        /// </returns>
        public ValueTask<Maybe<TOut>> Map<TOut>(Func<TIn, TOut> map)
            where TOut : notnull {
            return new ValueTask<Maybe<TOut>>(MapCore(maybeTask, map));

            static async Task<Maybe<TOut>> MapCore(Task<Maybe<TIn>> maybeTask,
                                                   Func<TIn, TOut>  map) {
                return (await maybeTask).Map(map);
            }
        }

        /// <summary>
        ///     Transforms the value inside the <see cref="Maybe{TValue}" /> asynchronously using the specified asynchronous
        ///     mapping function and returns a new <see cref="Maybe{TValue}" /> containing the result of the transformation.
        /// </summary>
        /// <typeparam name="TOut">
///     
/// </typeparam>
        ///     The type of the value to be returned by the mapping function, which will be encapsulated
        ///     in the resulting <see cref="Maybe{TValue}" />.
        /// <param name="map">
        ///     An asynchronous function to transform the value from the source <see cref="Maybe{TValue}" /> into another value.
        /// </param>
        /// <returns>
        ///     A task representing the asynchronous operation, which results in a new <see cref="Maybe{TValue}" />
        ///     containing the transformed value if present, or the original empty state otherwise.
        /// </returns>
        public ValueTask<Maybe<TOut>> Map<TOut>(Func<TIn, ValueTask<TOut>> map)
            where TOut : notnull {
            return new ValueTask<Maybe<TOut>>(MapCore(maybeTask, map));

            static async Task<Maybe<TOut>> MapCore(Task<Maybe<TIn>>         maybeTask,
                                                   Func<TIn, ValueTask<TOut>> map) {
                return await new ValueTask<Maybe<TIn>>(maybeTask).Map(map);
            }
        }
    }
}
