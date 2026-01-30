using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>The original result unchanged.</returns>
        public Result Tap(Action tap)
        {
            result.IfSuccess(tap);
            return result;
        }

        /// <summary>
        ///     Executes a side effect if the result is successful and the specified predicate evaluates to true,
        ///     then returns the original result.
        /// </summary>
        /// <param name="predicate">Function to evaluate a condition.</param>
        /// <param name="tap">Action to execute if the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public Result TapIf(Func<bool> predicate, Action tap)
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
        ///     Executes a side effect if the result is a failure, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on failure.</param>
        /// <returns>The original result unchanged.</returns>
        public Result TapError(Action<Failure> tap)
        {
            result.IfFailure(tap);
            return result;
        }
    }


    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue">Value type 1.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Executes a side effect if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on success.</param>
        /// <returns>The original result unchanged.</returns>
        public Result<TValue> Tap(Action<TValue> tap)
        {
            result.IfSuccess(tap);
            return result;
        }

        /// <summary>
        ///     Executes a specified action if the result is successful, then returns the original result.
        /// </summary>
        /// <param name="tap">The action to execute if the result is successful.</param>
        /// <returns>The original result unchanged.</returns>
        public Result<TValue> Tap(Action tap)
        {
            result.IfSuccess(tap);
            return result;
        }

        /// <summary>
        ///     Executes a specified action on the result value if the result is successful
        ///     and the given predicate evaluates to true, then returns the original result.
        /// </summary>
        /// <param name="predicate">
        ///     A function that determines whether the action should be executed based on the result value.
        /// </param>
        /// <param name="tap">The action to execute if the predicate evaluates to true.</param>
        /// <returns>The original result unchanged.</returns>
        public Result<TValue> TapIf(Func<TValue, bool> predicate, Action<TValue> tap)
        {
            return result.Tap(v =>
            {
                if (predicate(v))
                {
                    tap(v);
                }
            });
        }

        /// <summary>
        ///     Executes an action if the result is successful and the specified predicate is true, then returns the original
        ///     result.
        /// </summary>
        /// <param name="predicate">The predicate function to evaluate the value.</param>
        /// <param name="tap">The action to execute if the predicate is true.</param>
        /// <returns>The original result unchanged.</returns>
        public Result<TValue> TapIf(Func<TValue, bool> predicate, Action tap)
        {
            return result.Tap(v =>
            {
                if (predicate(v))
                {
                    tap();
                }
            });
        }

        /// <summary>
        ///     Executes a side effect if the result is a failure, then returns the original result.
        /// </summary>
        /// <param name="tap">Action to execute on failure.</param>
        /// <returns>The original result unchanged.</returns>
        public Result<TValue> TapError(Action<Failure> tap)
        {
            result.IfFailure(tap);
            return result;
        }
    }
}