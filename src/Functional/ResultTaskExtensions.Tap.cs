using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for tasks that return functional result types.
/// </summary>
public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Provides extension methods for ResultTask to allow the execution of side effects
    ///     while preserving the original result.
    /// </summary>
    extension(ResultTask result)
    {
        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">The action to execute on success.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask Tap(Action tap)
        {
            return TapCore(result, tap).AsAsync();

            static async ValueTask<Result> TapCore(ResultTask self, Action tap)
            {
                var source = await self;
                if (source.IsSuccess)
                {
                    tap();
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask Tap(Func<ValueTask> tap)
        {
            return TapCore(result, tap).AsAsync();

            static async ValueTask<Result> TapCore(ResultTask self, Func<ValueTask> tap)
            {
                var source = await self;
                if (source.IsSuccess)
                {
                    await tap();
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the specified condition is met and the result is successful,
        ///     then returns the original result.
        /// </summary>
        /// <param name="predicate">A function that evaluates the condition to determine if the side effect should be executed.</param>
        /// <param name="tap">The action to execute if the condition is met and the result is successful.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask TapIf(Func<bool> predicate, Action tap)
        {
            return result.Tap(() =>
            {
                if (predicate())
                {
                    tap();
                }
            });
        }

        /// <summary>
        ///     Executes a side effect conditionally based on a predicate if the result is successful, then returns the original
        ///     result.
        /// </summary>
        /// <param name="predicate">A function that determines whether the side effect should be executed.</param>
        /// <param name="tap">The side effect to execute if the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask TapIf(Func<ValueTask<bool>> predicate, Func<ValueTask> tap)
        {
            return TapIfCore(result, predicate, tap).AsAsync();

            static async ValueTask<Result> TapIfCore(ResultTask self,
                Func<ValueTask<bool>> predicate,
                Func<ValueTask> tap)
            {
                var source = await self;
                if (source.IsSuccess && await predicate())
                {
                    await tap();
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is a failure, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on failure, receiving the associated <see cref="Failure" />.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask TapError(Action<Failure> tap)
        {
            return TapErrorCore(result, tap).AsAsync();

            static async ValueTask<Result> TapErrorCore(ResultTask self, Action<Failure> tap)
            {
                var source = await self;
                if (source.IsFaulted && source.TryGetError(out var error))
                {
                    tap(error);
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is a failure, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute when the result is a failure.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask TapError(Func<Failure, ValueTask> tap)
        {
            return TapErrorCore(result, tap).AsAsync();

            static async ValueTask<Result> TapErrorCore(ResultTask self, Func<Failure, ValueTask> tap)
            {
                var source = await self;
                if (source.IsFaulted && source.TryGetError(out var error))
                {
                    await tap(error);
                }

                return source;
            }
        }
    }


    /// <summary>
    ///     Contains extension methods for the <see cref="ResultTask{TValue}" /> struct to support
    ///     additional functional programming operations such as Tap and TapIf methods.
    /// </summary>
    /// <typeparam name="TValue">The type of the value encapsulated within the result.</typeparam>
    extension<TValue>(ResultTask<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">The action to perform if the result is successful.</param>
        /// <returns>The original result task unchanged.</returns>
        public ResultTask<TValue> Tap(Action<TValue> tap)
        {
            return TapCore(result, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapCore(ResultTask<TValue> self, Action<TValue> tap)
            {
                var source = await self;
                if (source.TryGetValue(out var value))
                {
                    tap(value);
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> Tap(Func<TValue, ValueTask> tap)
        {
            return TapCore(result, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapCore(ResultTask<TValue> self, Func<TValue, ValueTask> tap)
            {
                var source = await self;
                if (source.TryGetValue(out var value))
                {
                    await tap(value);
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> Tap(Action tap)
        {
            return TapCore(result, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapCore(ResultTask<TValue> self, Action tap)
            {
                var source = await self;
                if (source.IsSuccess)
                {
                    tap();
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a specified action if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">The action to execute on a successful result.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> Tap(Func<ValueTask> tap)
        {
            return TapCore(result, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapCore(ResultTask<TValue> self, Func<ValueTask> tap)
            {
                var source = await self;
                if (source.IsSuccess)
                {
                    await tap();
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a specified side effect if the given predicate evaluates to true, then returns the original result.
        /// </summary>
        /// <param name="predicate">A function that determines whether to execute the side effect, based on the result value.</param>
        /// <param name="tap">The side effect to execute when the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> TapIf(Func<TValue, bool> predicate, Action<TValue> tap)
        {
            return result.Tap(value =>
            {
                if (predicate(value))
                {
                    tap(value);
                }
            });
        }

        /// <summary>
        ///     Executes a side effect on the result if the specified predicate evaluates to true, and returns the original result.
        /// </summary>
        /// <param name="predicate">A function to evaluate the condition based on the result's value.</param>
        /// <param name="tap">An action to execute if the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> TapIf(Func<TValue, ValueTask<bool>> predicate, Func<TValue, ValueTask> tap)
        {
            return TapIfCore(result, predicate, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapIfCore(ResultTask<TValue> self,
                Func<TValue, ValueTask<bool>> predicate,
                Func<TValue, ValueTask> tap)
            {
                var source = await self;
                if (source.TryGetValue(out var value) && await predicate(value))
                {
                    await tap(value);
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the specified predicate evaluates to true, then returns the original result.
        /// </summary>
        /// <param name="predicate">A function that determines whether the side effect should be executed.</param>
        /// <param name="tap">The action to execute if the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> TapIf(Func<TValue, bool> predicate, Action tap)
        {
            return result.Tap(value =>
            {
                if (predicate(value))
                {
                    tap();
                }
            });
        }

        /// <summary>
        ///     Executes a side effect if the specified predicate evaluates to true, then returns the original result.
        /// </summary>
        /// <param name="predicate">The function to evaluate the condition for executing the side effect.</param>
        /// <param name="tap">The asynchronous function to execute if the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> TapIf(Func<TValue, ValueTask<bool>> predicate, Func<ValueTask> tap)
        {
            return TapIfCore(result, predicate, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapIfCore(ResultTask<TValue> self,
                Func<TValue, ValueTask<bool>> predicate,
                Func<ValueTask> tap)
            {
                var source = await self;
                if (source.TryGetValue(out var value) && await predicate(value))
                {
                    await tap();
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is a failure, passing the error to the provided action,
        ///     and returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on failure, receiving the error as an argument.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> TapError(Action<Failure> tap)
        {
            return TapErrorCore(result, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapErrorCore(ResultTask<TValue> self, Action<Failure> tap)
            {
                var source = await self;
                if (source.IsFaulted && source.TryGetError(out var error))
                {
                    tap(error);
                }

                return source;
            }
        }

        /// <summary>
        ///     Executes a side effect if the result is a failure, then returns the original result.
        /// </summary>
        /// <param name="tap">Asynchronous function to execute on failure.</param>
        /// <returns>The original result unchanged.</returns>
        public ResultTask<TValue> TapError(Func<Failure, ValueTask> tap)
        {
            return TapErrorCore(result, tap).AsAsync();

            static async ValueTask<Result<TValue>> TapErrorCore(ResultTask<TValue> self, Func<Failure, ValueTask> tap)
            {
                var source = await self;
                if (source.IsFaulted && source.TryGetError(out var error))
                {
                    await tap(error);
                }

                return source;
            }
        }
    }
}