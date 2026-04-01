namespace UnambitiousFx.Functional;

public static partial class MaybeAsyncExtensions {
    extension<TValue>(Maybe<TValue> maybe)
        where TValue : notnull {
        /// <summary>
        ///     Executes a side effect when a value is present and returns the original maybe.
        /// </summary>
        public Maybe<TValue> TapSome(Action<TValue> tap) {
            return maybe.Tap(tap);
        }

        /// <summary>
        ///     Executes a side effect when no value is present and returns the original maybe.
        /// </summary>
        public Maybe<TValue> TapNone(Action tap) {
            if (maybe.IsNone) {
                tap();
            }

            return maybe;
        }
    }

    extension<TIn>(ValueTask<Maybe<TIn>> maybeTask)
        where TIn : notnull {
        /// <summary>
        ///     Executes a specified side effect when the Maybe contains a value (Some) and
        ///     returns the original Maybe.
        /// </summary>
        /// <param name="tap">The action to perform if the Maybe contains a value.</param>
        /// <returns>The original Maybe instance.</returns>
        public async ValueTask<Maybe<TIn>> TapSome(Action<TIn> tap) {
            var maybe = await maybeTask;
            return maybe.TapSome(tap);
        }

        /// <summary>
        ///     Executes an asynchronous side effect when a value is present and returns the original maybe.
        /// </summary>
        /// <param name="tap">The asynchronous side effect to execute if the value is present.</param>
        /// <returns>The original maybe, passed through unchanged, after executing the asynchronous side effect.</returns>
        public async ValueTask<Maybe<TIn>> TapSome(Func<TIn, ValueTask> tap) {
            var maybe = await maybeTask;
            if (maybe.Some(out var value)) {
                await tap(value);
            }

            return maybe;
        }

        /// <summary>
        ///     Executes a side effect when the current instance represents a "None" state
        ///     and returns the original <see cref="Maybe{TValue}" /> instance.
        /// </summary>
        /// <param name="tap">
        ///     An action to execute if the current instance represents a "None" state.
        /// </param>
        /// <returns>
        ///     The original <see cref="Maybe{TValue}" /> instance.
        /// </returns>
        public async ValueTask<Maybe<TIn>> TapNone(Action tap) {
            var maybe = await maybeTask;
            return maybe.TapNone(tap);
        }

        /// <summary>
        ///     Executes a side effect when no value is present and returns the original maybe.
        /// </summary>
        /// <param name="tap">
        ///     A function that represents the side effect to execute when the maybe is empty.
        /// </param>
        /// <returns>
        ///     The original maybe after executing the provided side effect, if applicable.
        /// </returns>
        public async ValueTask<Maybe<TIn>> TapNone(Func<ValueTask> tap) {
            var maybe = await maybeTask;
            if (maybe.IsNone) {
                await tap();
            }

            return maybe;
        }
    }
}
