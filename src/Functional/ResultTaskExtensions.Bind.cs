namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    extension<TValue>(ResultTask<TValue> resultTask) where TValue : notnull
    {
        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask Bind(Func<TValue, Result> bind)
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result> BindCore(ResultTask<TValue> self, Func<TValue, Result> bind)
            {
                var source = await self;
                return source.Match(
                    value => bind(value).WithMetadata(source.Metadata),
                    error => Result.Failure(error).WithMetadata(source.Metadata));
            }
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask Bind(Func<TValue, ValueTask<Result>> bind)
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result> BindCore(ResultTask<TValue> self, Func<TValue, ValueTask<Result>> bind)
            {
                var source = await self;
                return await source.Match<ValueTask<Result>>(
                    async value =>
                    {
                        var result = await bind(value);
                        return result.WithMetadata(source.Metadata);
                    },
                    error => ValueTask.FromResult(Result.Failure(error).WithMetadata(source.Metadata)));
            }
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask<TOut> Bind<TOut>(Func<TValue, Result<TOut>> bind)
            where TOut : notnull
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result<TOut>> BindCore(ResultTask<TValue> self, Func<TValue, Result<TOut>> bind)
            {
                var source = await self;
                return source.Match(
                    value => bind(value).WithMetadata(source.Metadata),
                    error => Result.Failure<TOut>(error).WithMetadata(source.Metadata));
            }
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask<TOut> Bind<TOut>(Func<TValue, ValueTask<Result<TOut>>> bind) where TOut : notnull
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result<TOut>> BindCore(ResultTask<TValue> self,
                Func<TValue, ValueTask<Result<TOut>>> bind)
            {
                var source = await self;
                return await source.Match<ValueTask<Result<TOut>>>(
                    async value =>
                    {
                        var result = await bind(value);
                        return result.WithMetadata(source.Metadata);
                    },
                    error => ValueTask.FromResult(Result.Failure<TOut>(error).WithMetadata(source.Metadata)));
            }
        }
    }


    /// <summary>
    ///     Provides extension methods for <see cref="ResultTask" /> to enable chaining of result transformations
    ///     by performing asynchronous and synchronous bind operations.
    /// </summary>
    extension(ResultTask resultTask)
    {
        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask Bind(Func<Result> bind)
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result> BindCore(ResultTask self, Func<Result> bind)
            {
                var source = await self;
                return source.Match(
                    () => bind().WithMetadata(source.Metadata),
                    error => Result.Failure(error).WithMetadata(source.Metadata));
            }
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask Bind(Func<ValueTask<Result>> bind)
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result> BindCore(ResultTask self, Func<ValueTask<Result>> bind)
            {
                var source = await self;
                return await source.Match<ValueTask<Result>>(
                    async () =>
                    {
                        var result = await bind();
                        return result.WithMetadata(source.Metadata);
                    },
                    error => ValueTask.FromResult(Result.Failure(error).WithMetadata(source.Metadata)));
            }
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining a sync function.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask<TOut> Bind<TOut>(Func<Result<TOut>> bind)
            where TOut : notnull
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result<TOut>> BindCore(ResultTask self, Func<Result<TOut>> bind)
            {
                var source = await self;
                return source.Match(
                    () => bind().WithMetadata(source.Metadata),
                    error => Result.Failure<TOut>(error).WithMetadata(source.Metadata));
            }
        }

        /// <summary>
        ///     Async Bind awaiting result then chaining an async function.
        /// </summary>
        /// <typeparam name="TOut">Output value type 1.</typeparam>
        /// <param name="bind">The async function to execute if the result is successful.</param>
        /// <returns>A task with the result from the bind function.</returns>
        public ResultTask<TOut> Bind<TOut>(Func<ValueTask<Result<TOut>>> bind)
            where TOut : notnull
        {
            return BindCore(resultTask, bind).AsAsync();

            static async ValueTask<Result<TOut>> BindCore(ResultTask self, Func<ValueTask<Result<TOut>>> bind)
            {
                var source = await self;
                return await source.Match<ValueTask<Result<TOut>>>(
                    async () =>
                    {
                        var result = await bind();
                        return result.WithMetadata(source.Metadata);
                    },
                    error => ValueTask.FromResult(Result.Failure<TOut>(error).WithMetadata(source.Metadata)));
            }
        }
    }
}