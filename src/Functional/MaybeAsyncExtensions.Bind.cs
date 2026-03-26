namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Maybe operations.
/// </summary>
public static partial class MaybeAsyncExtensions {
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Transforms the current <see cref="Maybe{TValue}" /> asynchronously by applying a function
        ///     that returns another <see cref="Maybe{TValue}" />. Allows chaining of computations that
        ///     may fail, propagating the "None" state if it exists.
        /// </summary>
        /// <param name="bind">
        ///     A function that takes a value of type <typeparamref name="TIn" /> and returns a
        ///     <see cref="Maybe{TOut}" />. This function defines the transformation to apply to the contained value.
        /// </param>
        /// <returns>
        ///     A task containing a <see cref="Maybe{TOut}" />. Returns "None"
        ///     if the input maybe is "None", otherwise applies the provided bind function and returns the result.
        /// </returns>
        public async ValueTask<Maybe<TOut>> Bind<TOut>(Func<TIn, Maybe<TOut>> bind)
            where TOut : notnull {
            var maybe = await maybeTask;
            return maybe.Bind(bind);
        }

        /// <summary>
        ///     Transforms the value contained in the asynchronous maybe using the provided asynchronous binding function.
        ///     Returns a new asynchronous maybe containing the result of the binding function if a value is present;
        ///     otherwise, returns an asynchronous maybe with no value.
        /// </summary>
        /// <param name="bind">
        ///     A function to transform the value contained in the asynchronous maybe into another asynchronous maybe.
        /// </param>
        /// <returns>
        ///     A new asynchronous maybe containing the result of the binding function if a value is present in the original
        ///     asynchronous maybe; otherwise, an asynchronous maybe with no value.
        /// </returns>
        public async ValueTask<Maybe<TOut>> Bind<TOut>(Func<TIn, ValueTask<Maybe<TOut>>> bind)
            where TOut : notnull {
            var maybe = await maybeTask;
            if (!maybe.Some(out var value)) {
                return Maybe.None<TOut>();
            }

            return await bind(value);
        }
    }

    extension<TIn>(Task<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Transforms the value of a <see cref="Maybe{TValue}" /> asynchronously
        ///     using a specified binding function and returns the resulting <see cref="Maybe{TValue}" />.
        /// </summary>
        /// <typeparam name="TOut">
        ///     The type of the value encapsulated in the resulting <see cref="Maybe{TValue}" />.
        /// </typeparam>
        /// <param name="bind">
        ///     A function to apply to the value of the original <see cref="Maybe{TValue}" />
        ///     if it contains a value, producing a new <see cref="Maybe{TValue}" />.
        /// </param>
        /// <returns>
        ///     A task containing the transformed <see cref="Maybe{TValue}" />.
        ///     If the original <see cref="Maybe{TValue}" /> contains a value, the result of applying
        ///     the binding function is returned; otherwise, an empty <see cref="Maybe{TValue}" /> is returned.
        /// </returns>
        public ValueTask<Maybe<TOut>> Bind<TOut>(Func<TIn, Maybe<TOut>> bind)
            where TOut : notnull {
            return new ValueTask<Maybe<TOut>>(BindCore(maybeTask, bind));

            static async Task<Maybe<TOut>> BindCore(Task<Maybe<TIn>>       maybeTask,
                                                    Func<TIn, Maybe<TOut>> bind) {
                return (await maybeTask).Bind(bind);
            }
        }

        /// <summary>
        ///     Transforms the value of a <see cref="Maybe{TValue}" /> asynchronously using the given function
        ///     that returns a task. If the original Maybe has no value, the result will also have no value.
        /// </summary>
        /// <param name="bind">
        ///     A function that takes the current value of the Maybe and returns a task.
        /// </param>
        /// <returns>
        ///     A task containing the transformed Maybe, or an empty Maybe if the original had no value.
        /// </returns>
        public ValueTask<Maybe<TOut>> Bind<TOut>(Func<TIn, ValueTask<Maybe<TOut>>> bind)
            where TOut : notnull {
            return new ValueTask<Maybe<TOut>>(BindCore(maybeTask, bind));

            static async Task<Maybe<TOut>> BindCore(Task<Maybe<TIn>>                  maybeTask,
                                                    Func<TIn, ValueTask<Maybe<TOut>>> bind) {
                return await new ValueTask<Maybe<TIn>>(maybeTask).Bind(bind);
            }
        }
    }
}
