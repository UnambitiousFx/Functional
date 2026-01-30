namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result Bind(Func<Result> bind)
        {
            return result.Match(() => bind().WithMetadata(result.Metadata),
                err => Result.Failure(err).WithMetadata(result.Metadata));
        }

        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result<TOut> Bind<TOut>(Func<Result<TOut>> bind) where TOut : notnull
        {
            return result.Match(() => bind().WithMetadata(result.Metadata),
                err => Result.Failure<TOut>(err).WithMetadata(result.Metadata));
        }

        /// <summary>
        ///     Chains a function that returns a new result, propagating any failures.
        /// </summary>
        /// <param name="bind">The function to execute if the current result is successful.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public ResultTask Bind(Func<ResultTask> bind)
        {
            return result.Match<ValueTask<Result>>(async () =>
                    {
                        var result = await bind();
                        return result.WithMetadata(result.Metadata);
                    },
                    err => ValueTask.FromResult(Result.Failure(err).WithMetadata(result.Metadata)))
                .AsAsync();
        }

        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public ResultTask<TOut> Bind<TOut>(Func<ResultTask<TOut>> bind)
            where TOut : notnull
        {
            return result.Match<ValueTask<Result<TOut>>>(async () =>
                    {
                        var result = await bind();
                        return result.WithMetadata(result.Metadata);
                    },
                    err => ValueTask.FromResult(Result.Failure<TOut>(err).WithMetadata(result.Metadata)))
                .AsAsync();
        }
    }

    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue">Input value type 1.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result Bind(Func<TValue, Result> bind)
        {
            return result.Match(v => bind(v).WithMetadata(result.Metadata),
                err => Result.Failure(err).WithMetadata(result.Metadata));
        }

        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result<TOut> Bind<TOut>(Func<TValue, Result<TOut>> bind) where TOut : notnull
        {
            return result.Match(v => bind(v).WithMetadata(result.Metadata),
                err => Result.Failure<TOut>(err).WithMetadata(result.Metadata));
        }

        /// <summary>
        ///     Async Bind chaining an async function that returns a Result.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask Bind(Func<TValue, ResultTask> bind)
        {
            return result.Match<ValueTask<Result>>(async v =>
            {
                var response = await bind(v);
                return response.WithMetadata(result.Metadata);
            }, err =>
            {
                var response = Result.Failure(err)
                    .WithMetadata(result.Metadata);
                return ValueTask.FromResult(response);
            }).AsAsync();
        }

        /// <summary>
        ///     Async Bind chaining an async function that returns a Result.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask<TOut> Bind<TOut>(Func<TValue, ResultTask<TOut>> bind) where TOut : notnull
        {
            return result.Match<ValueTask<Result<TOut>>>(async v =>
            {
                var response = await bind(v);
                return response.WithMetadata(result.Metadata);
            }, err =>
            {
                var response = Result.Failure<TOut>(err).WithMetadata(result.Metadata);
                return ValueTask.FromResult(response);
            }).AsAsync();
        }
    }
}