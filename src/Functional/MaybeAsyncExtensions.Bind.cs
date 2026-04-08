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
}
