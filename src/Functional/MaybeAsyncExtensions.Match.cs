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

    extension<TIn>(Task<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Executes one of two functions based on whether the asynchronous maybe contains a value and returns the result.
        /// </summary>
        /// <param name="some">Function to execute when a value is present.</param>
        /// <param name="none">Function to execute when no value is present.</param>
        /// <returns>The result of executing either the some or none function.</returns>
        public ValueTask<TOut> Match<TOut>(Func<TIn, TOut> some,
                                           Func<TOut>      none) {
            return new ValueTask<TOut>(MatchCore(maybeTask, some, none));

            static async Task<TOut> MatchCore(Task<Maybe<TIn>> maybeTask,
                                              Func<TIn, TOut>  some,
                                              Func<TOut>       none) {
                return (await maybeTask).Match(some, none);
            }
        }

        /// <summary>
        ///     Executes one of two asynchronous functions based on whether the asynchronous maybe contains a value and returns
        ///     the result.
        /// </summary>
        /// <param name="some">Asynchronous function to execute when a value is present.</param>
        /// <param name="none">Asynchronous function to execute when no value is present.</param>
        /// <returns>The result of executing either the some or none asynchronous function.</returns>
        public ValueTask<TOut> Match<TOut>(Func<TIn, ValueTask<TOut>> some,
                                           Func<ValueTask<TOut>>      none) {
            return new ValueTask<TOut>(MatchCore(maybeTask, some, none));

            static async Task<TOut> MatchCore(Task<Maybe<TIn>>           maybeTask,
                                              Func<TIn, ValueTask<TOut>> some,
                                              Func<ValueTask<TOut>>      none) {
                return await new ValueTask<Maybe<TIn>>(maybeTask).Match(some, none);
            }
        }

        /// <summary>
        ///     Invokes the specified actions based on the state of the asynchronous <see cref="Maybe{TValue}" />.
        ///     Executes the <paramref name="some" /> action if a value is present, or the <paramref name="none" />
        ///     action if no value is present, without returning a result.
        /// </summary>
        ///     The type of the value encapsulated by the <see cref="Maybe{TValue}" /> in the asynchronous context.
        /// <param name="some">
        ///     The action to be executed if the <see cref="Maybe{TValue}" /> contains a value.
        /// </param>
        /// <param name="none">
        ///     The action to be executed if the <see cref="Maybe{TValue}" /> is empty.
        /// </param>
        /// <returns>
        ///     A <see cref="ValueTask" /> that represents the asynchronous operation.
        /// </returns>
        public ValueTask Switch(Action<TIn> some,
                                Action      none) {
            return new ValueTask(SwitchCore(maybeTask, some, none));

            static async Task SwitchCore(Task<Maybe<TIn>> maybeTask,
                                         Action<TIn>      some,
                                         Action           none) {
                (await maybeTask).Switch(some, none);
            }
        }

        /// <summary>
        ///     Executes the appropriate asynchronous action based on whether a value is present or absent in the maybe.
        /// </summary>
        /// <param name="some">
        ///     An asynchronous function to invoke if a value is present.
        /// </param>
        /// <param name="none">
        ///     An asynchronous function to invoke if no value is present.
        /// </param>
        /// <returns>
        ///     A <see cref="ValueTask" /> representing the asynchronous operation.
        /// </returns>
        public ValueTask Switch(Func<TIn, ValueTask> some,
                                Func<ValueTask>      none) {
            return new ValueTask(SwitchCore(maybeTask, some, none));

            static async Task SwitchCore(Task<Maybe<TIn>>     maybeTask,
                                         Func<TIn, ValueTask> some,
                                         Func<ValueTask>      none) {
                await new ValueTask<Maybe<TIn>>(maybeTask).Switch(some, none);
            }
        }
    }
}
