namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Chains a transformation function to the result. The transformation is executed
    ///     only if the current result is not faulted.
    /// </summary>
    /// <param name="result">The current result instance.</param>
    /// <param name="func">The transformation function to execute on the result.</param>
    /// <returns>A new result from the transformation function, or the original result if it is faulted.</returns>
    public static Result Then(this Result result, Func<Result> func)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return func();
    }

    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue">Value type</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Chains a transformation that returns a Result of the same type.
        /// </summary>
        /// <param name="then">The transformation function.</param>
        /// <returns>A new result from the then function.</returns>
        public Result<TValue> Then(Func<TValue, Result<TValue>> then)
        {
            if (!result.TryGetValue(out var value))
            {
                return result;
            }

            var response = then(value);

            return response.WithMetadata(result.Metadata);
        }

        /// <summary>
        ///     Chains a transformation that returns a Result from the provided function.
        /// </summary>
        /// <param name="then">The transformation function to execute when the result is successful.</param>
        /// <returns>
        ///     A new result obtained from the transformation function, while retaining metadata if specified.
        /// </returns>
        public Result<TValue> Then(Func<TValue, Result> then)
        {
            if (!result.TryGetValue(out var value))
            {
                return result;
            }

            var thenResult = then(value);
            if (!thenResult.TryGetError(out var error))
            {
                return result;
            }

            var failResult = Result.Failure<TValue>(error);
            return failResult.WithMetadata(result.Metadata);
        }
    }
}