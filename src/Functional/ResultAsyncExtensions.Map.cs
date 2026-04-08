namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions {
    /// <summary>
    ///     Provides extension methods for transforming asynchronous Result operations
    ///     into new Result types by applying mapping logic.
    /// </summary>
    extension(ValueTask<Result> resultTask) {
        /// <summary>
        ///     Transforms the value inside the asynchronous result using an asynchronous mapping function.
        /// </summary>
        /// <typeparam name="TOut">
        ///     The type of the transformed value.
        /// </typeparam>
        /// <param name="map">
        ///     The asynchronous mapping function to transform the success value.
        /// </param>
        /// <returns>
        ///     A task producing a <see cref="Result{TOut}" />, which contains the transformed value if the operation succeeds,
        ///     or the original error if it fails.
        /// </returns>
        public async ValueTask<Result<TOut>> Map<TOut>(Func<ValueTask<TOut>> map)
            where TOut : notnull {
            var result = await resultTask;
            return result.TryGetFailure(out var error)
                       ? Result.Failure<TOut>(error)
                       : Result.Success(await map());
        }

        /// <summary>
        ///     Transforms the value inside the result asynchronously using a mapping function.
        /// </summary>
        /// <typeparam name="TOut">The type of the transformed value.</typeparam>
        /// <param name="map">
        ///     The mapping function to apply to the result. If the result is successful, this function is executed
        ///     to yield the new transformed value.
        /// </param>
        /// <returns>
        ///     A new <see cref="Result{TOut}" /> containing the transformed value if the original result was successful,
        ///     or the original error if the result was a failure.
        /// </returns>
        public async ValueTask<Result<TOut>> Map<TOut>(Func<TOut> map)
            where TOut : notnull {
            var result = await resultTask;
            return result.TryGetFailure(out var error)
                       ? Result.Failure<TOut>(error)
                       : Result.Success(map());
        }
    }

    /// <param name="resultTask">The asynchronous result to apply the mapping function on.</param>
    /// <typeparam name="TIn">The type of the input value in the result.</typeparam>
    extension<TIn>(ValueTask<Result<TIn>> resultTask)
        where TIn : notnull {
        /// <summary>
        ///     Transforms the value inside the async result using an asynchronous mapping function.
        /// </summary>
        public async ValueTask<Result<TOut>> Map<TOut>(Func<TIn, ValueTask<TOut>> map)
            where TOut : notnull {
            var result = await resultTask;
            if (!result.TryGetValue(out var value)) {
                result.TryGetFailure(out var error);
                return Result.Failure<TOut>(error!);
            }

            return Result.Success(await map(value));
        }

        /// <summary>
        ///     Transforms the value within the asynchronous result using the provided mapping function
        ///     that operates on a synchronous input.
        /// </summary>
        /// <typeparam name="TOut">The type of the output value after mapping.</typeparam>
        /// <param name="map">The synchronous mapping function to transform the input value.</param>
        /// <returns>A task representing the asynchronous operation, resulting in a transformed result.</returns>
        public async ValueTask<Result<TOut>> Map<TOut>(Func<TIn, TOut> map)
            where TOut : notnull {
            var result = await resultTask;
            return result.Map(map);
        }
    }
}
