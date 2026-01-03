namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    extension<TValue1>(Result<TValue1> result) where TValue1 : notnull
    {
        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result Bind(Func<TValue1, Result> bind,
            bool copyReasonsAndMetadata = true)
        {
            return result.Match(v =>
            {
                var response = bind(v);
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            }, err =>
            {
                var response = Result.Failure(err);
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            });
        }

        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result<TOut1> Bind<TOut1>(Func<TValue1, Result<TOut1>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            return result.Match(v =>
            {
                var response = bind(v);
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            }, err =>
            {
                var response = Result.Failure<TOut1>(err);
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            });
        }
    }

    /// <param name="result">The result instance.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result Bind(Func<Result> bind, bool copyReasonsAndMetadata = true)
        {
            return result.Match(() =>
            {
                var response = bind();
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            }, err =>
            {
                var response = Result.Failure(err);
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            });
        }

        /// <summary>
        ///     Chains a function that returns a Result, propagating failures.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>The result from the bind function, or a failure result.</returns>
        public Result<TOut1> Bind<TOut1>(Func<Result<TOut1>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            return result.Match(() =>
            {
                var response = bind();
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            }, err =>
            {
                var response = Result.Failure<TOut1>(err);
                if (copyReasonsAndMetadata)
                {
                    response = response.WithMetadata(result.Metadata);
                }

                return response;
            });
        }
    }
}
