namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Executes the provided action within the context of the current <see cref="ResultTask" />.
    ///     If the action throws an exception, the resulting <see cref="ResultTask" /> will encapsulate the error.
    /// </summary>
    /// <param name="result">The current <see cref="ResultTask" /> instance.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>A new <see cref="ResultTask" /> reflecting the outcome of the action execution.</returns>
    public static ResultTask Try(this ResultTask result, Action action)
    {
        return TryCore(result, action).AsAsync();

        static async ValueTask<Result> TryCore(ResultTask self, Action action)
        {
            var source = await self;
            if (source.IsFaulted)
            {
                return source;
            }

            try
            {
                action();
                return Result.Success().WithMetadata(source.Metadata);
            }
            catch (Exception ex)
            {
                return Result.Failure(ex).WithMetadata(source.Metadata);
            }
        }
    }

    /// <summary>
    ///     Executes the provided action if the result represents success, allowing for side effects but not modifying the
    ///     result.
    /// </summary>
    /// <param name="result">The initial <see cref="ResultTask" /> to evaluate for success.</param>
    /// <param name="action">The action to execute if the result represents success.</param>
    /// <returns>A <see cref="ResultTask" /> that wraps the original operation with the side-effect action applied on success.</returns>
    public static ResultTask Try(this ResultTask result, Func<ValueTask> action)
    {
        return TryCore(result, action).AsAsync();

        static async ValueTask<Result> TryCore(ResultTask self, Func<ValueTask> action)
        {
            var source = await self;
            if (source.IsFaulted)
            {
                return source;
            }

            try
            {
                await action();
                return Result.Success().WithMetadata(source.Metadata);
            }
            catch (Exception ex)
            {
                return Result.Failure(ex).WithMetadata(source.Metadata);
            }
        }
    }


    /// <summary>
    ///     Executes the specified function with the value held by the current <see cref="ResultTask{TValue}" />
    ///     if it represents a successful result. If the function execution throws an exception, the resulting
    ///     <see cref="ResultTask{TOut}" /> will encapsulate the error.
    /// </summary>
    /// <param name="result">The current <see cref="ResultTask{TValue}" /> instance.</param>
    /// <param name="func">The function to execute, which processes the value and returns a result of type.</param>
    /// <typeparam name="TValue">The type of the value held by the current <see cref="ResultTask{TValue}" />.</typeparam>
    /// <typeparam name="TOut">The type of the value produced by the executed function.</typeparam>
    /// <returns>
    ///     A new <see cref="ResultTask{TOut}" /> reflecting the outcome of the function execution, or an error if one
    ///     occurs.
    /// </returns>
    public static ResultTask<TOut> Try<TValue, TOut>(this ResultTask<TValue> result, Func<TValue, TOut> func)
        where TValue : notnull where TOut : notnull
    {
        return TryCore(result, func).AsAsync();

        static async ValueTask<Result<TOut>> TryCore(ResultTask<TValue> self, Func<TValue, TOut> func)
        {
            var source = await self;
            if (!source.TryGet(out var value, out var error))
            {
                return Result.Failure<TOut>(error).WithMetadata(source.Metadata);
            }

            try
            {
                var newValue = func(value);
                return Result.Success(newValue).WithMetadata(source.Metadata);
            }
            catch (Exception ex)
            {
                return Result.Failure<TOut>(ex).WithMetadata(source.Metadata);
            }
        }
    }

    /// <summary>
    ///     Executes the provided asynchronous function within the context of the current <see cref="ResultTask{TValue}" />,
    ///     encapsulating the result or any exception that occurs during execution.
    /// </summary>
    /// <typeparam name="TValue">The type of the value encapsulated within the current <see cref="ResultTask{TValue}" />.</typeparam>
    /// <typeparam name="TOut">The type of the value produced by the asynchronous function.</typeparam>
    /// <param name="result">The current <see cref="ResultTask{TValue}" /> instance to operate on.</param>
    /// <param name="func">
    ///     The asynchronous function to execute, accepting a value of type <typeparamref name="TValue" />
    ///     and returning a <see cref="ValueTask{TOut}" />.
    /// </param>
    /// <returns>A new <see cref="ResultTask{TOut}" /> reflecting the outcome of the function execution.</returns>
    public static ResultTask<TOut> Try<TValue, TOut>(this ResultTask<TValue> result, Func<TValue, ValueTask<TOut>> func)
        where TValue : notnull where TOut : notnull
    {
        return TryCore(result, func).AsAsync();

        static async ValueTask<Result<TOut>> TryCore(ResultTask<TValue> self, Func<TValue, ValueTask<TOut>> func)
        {
            var source = await self;
            if (!source.TryGet(out var value, out var error))
            {
                return Result.Failure<TOut>(error).WithMetadata(source.Metadata);
            }

            try
            {
                var newValue = await func(value);
                return Result.Success(newValue).WithMetadata(source.Metadata);
            }
            catch (Exception ex)
            {
                return Result.Failure<TOut>(ex).WithMetadata(source.Metadata);
            }
        }
    }
}