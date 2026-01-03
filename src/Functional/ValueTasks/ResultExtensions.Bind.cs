namespace UnambitiousFx.Functional.ValueTasks;

public static partial class ResultExtensions
{
    /// <param name="result">The result instance.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Async Bind chaining an async function that returns a Result.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ValueTask<Result> BindAsync(Func<ValueTask<Result>> bind,
            bool copyReasonsAndMetadata = true)
        {
            return result.Match<ValueTask<Result>>(async () =>
            {
                var response = await bind();
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

                return new ValueTask<Result>(response);
            });
        }

        /// <summary>
        ///     Async Bind chaining an async function that returns a Result.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ValueTask<Result<TOut1>> BindAsync<TOut1>(Func<ValueTask<Result<TOut1>>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            return result.Match<ValueTask<Result<TOut1>>>(async () =>
            {
                var response = await bind();
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

                return new ValueTask<Result<TOut1>>(response);
            });
        }
    }

    /// <param name="awaitable">The awaitable result instance.</param>
    extension(ValueTask<Result> awaitable)
    {
        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result> BindAsync(Func<Result> bind,
            bool copyReasonsAndMetadata = true)
        {
            var result = await awaitable;
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
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result> BindAsync(Func<ValueTask<Result>> bind,
            bool copyReasonsAndMetadata = true)
        {
            var result = await awaitable;
            return await result.BindAsync(bind, copyReasonsAndMetadata);
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result<TOut1>> BindAsync<TOut1>(Func<Result<TOut1>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            var result = await awaitable;
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

        /// <summary>
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result<TOut1>> BindAsync<TOut1>(Func<ValueTask<Result<TOut1>>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            var result = await awaitable;
            return await result.BindAsync(bind, copyReasonsAndMetadata);
        }
    }


    /// <param name="result">The result instance.</param>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    extension<TValue1>(Result<TValue1> result) where TValue1 : notnull
    {
        /// <summary>
        ///     Async Bind chaining an async function that returns a Result.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ValueTask<Result> BindAsync(Func<TValue1, ValueTask<Result>> bind, bool copyReasonsAndMetadata = true)
        {
            return result.Match<ValueTask<Result>>(async v =>
            {
                var response = await bind(v);
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

                return new ValueTask<Result>(response);
            });
        }

        /// <summary>
        ///     Async Bind chaining an async function that returns a Result.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ValueTask<Result<TOut1>> BindAsync<TOut1>(Func<TValue1, ValueTask<Result<TOut1>>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            return result.Match<ValueTask<Result<TOut1>>>(async v =>
            {
                var response = await bind(v);
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

                return new ValueTask<Result<TOut1>>(response);
            });
        }
    }

    /// <param name="awaitable">The awaitable result instance.</param>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    extension<TValue1>(ValueTask<Result<TValue1>> awaitable) where TValue1 : notnull
    {
        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result> BindAsync(Func<TValue1, Result> bind, bool copyReasonsAndMetadata = true)
        {
            var result = await awaitable;
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
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result> BindAsync(Func<TValue1, ValueTask<Result>> bind,
            bool copyReasonsAndMetadata = true)
        {
            var result = await awaitable;
            return await result.BindAsync(bind, copyReasonsAndMetadata);
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result<TOut1>> BindAsync<TOut1>(Func<TValue1, Result<TOut1>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            var result = await awaitable;
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

        /// <summary>
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <typeparam name="TOut1">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <param name="copyReasonsAndMetadata">Whether to copy reasons and metadata from original result.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public async ValueTask<Result<TOut1>> BindAsync<TOut1>(Func<TValue1, ValueTask<Result<TOut1>>> bind,
            bool copyReasonsAndMetadata = true) where TOut1 : notnull
        {
            var result = await awaitable;
            return await result.BindAsync(bind, copyReasonsAndMetadata);
        }
    }
}
