namespace UnambitiousFx.Functional;

public static partial class MaybeAsyncExtensions {
    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Executes one of two functions based on whether the asynchronous maybe contains a value and returns the result.
        /// </summary>
        /// <param name="some">Function to execute when a value is present.</param>
        /// <param name="none">Function to execute when no value is present.</param>
        /// <returns>The result of executing either the some or none function.</returns>
        public async ValueTask<TOut> Match<TOut>(Func<TIn, TOut> some,
                                                 Func<TOut>      none) {
            var maybe = await maybeTask;
            return maybe.Match(some, none);
        }

        /// <summary>
        ///     Executes one of two asynchronous functions based on whether the asynchronous maybe contains a value and returns
        ///     the result.
        /// </summary>
        /// <param name="some">Asynchronous function to execute when a value is present.</param>
        /// <param name="none">Asynchronous function to execute when no value is present.</param>
        /// <returns>The result of executing either the some or none asynchronous function.</returns>
        public async ValueTask<TOut> Match<TOut>(Func<TIn, ValueTask<TOut>> some,
                                                 Func<ValueTask<TOut>>      none) {
            var maybe = await maybeTask;
            if (maybe.Some(out var value)) {
                return await some(value);
            }

            return await none();
        }

        /// <summary>
        ///     Executes the appropriate action based on whether the asynchronous maybe contains a value.
        /// </summary>
        /// <param name="some">Action to execute when a value is present.</param>
        /// <param name="none">Action to execute when no value is present.</param>
        public async ValueTask Switch(Action<TIn> some,
                                      Action      none) {
            var maybe = await maybeTask;
            maybe.Switch(some, none);
        }

        /// <summary>
        ///     Executes the appropriate asynchronous delegate based on whether the value is present or absent.
        /// </summary>
        /// <param name="some">
        ///     The asynchronous delegate to execute if a value is present.
        /// </param>
        /// <param name="none">
        ///     The asynchronous delegate to execute if no value is present.
        /// </param>
        public async ValueTask Switch(Func<TIn, ValueTask> some,
                                      Func<ValueTask>      none) {
            var maybe = await maybeTask;
            if (maybe.Some(out var value)) {
                await some(value);
                return;
            }

            await none();
        }
    }

}
