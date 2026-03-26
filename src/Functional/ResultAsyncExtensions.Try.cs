#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    public static async ValueTask<Result> Try(this ValueTask<Result> resultTask,
                                              Action                  action)
    {
        var result = await resultTask;
        return result.Try(action);
    }

    public static async ValueTask<Result> Try(this ValueTask<Result> resultTask,
                                              Func<ValueTask>        action)
    {
        var result = await resultTask;
        return await result.Bind(async () =>
        {
            try
            {
                await action();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        });
    }

    public static async ValueTask<Result<TOut>> Try<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                Func<TIn, TOut>             func)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Try(func);
    }

    public static async ValueTask<Result<TOut>> Try<TIn, TOut>(this ValueTask<Result<TIn>>    resultTask,
                                                                Func<TIn, ValueTask<TOut>>     func)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return await result.Bind(async value =>
        {
            try
            {
                var newValue = await func(value);
                return Result.Success(newValue);
            }
            catch (Exception ex)
            {
                return Result.Failure<TOut>(ex);
            }
        });
    }

    // Task extensions
    public static ValueTask<Result> Try(this Task<Result> resultTask,
                                        Action            action)
    {
        return new ValueTask<Result>(TryCore(resultTask, action));

        static async Task<Result> TryCore(Task<Result> resultTask,
                                          Action       action)
        {
            return (await resultTask).Try(action);
        }
    }

    public static ValueTask<Result> Try(this Task<Result>   resultTask,
                                        Func<ValueTask>     action)
    {
        return new ValueTask<Result>(TryCore(resultTask, action));

        static async Task<Result> TryCore(Task<Result>    resultTask,
                                          Func<ValueTask> action)
        {
            return await new ValueTask<Result>(resultTask).Try(action);
        }
    }

    public static ValueTask<Result<TOut>> Try<TIn, TOut>(this Task<Result<TIn>> resultTask,
                                                          Func<TIn, TOut>        func)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(TryCore(resultTask, func));

        static async Task<Result<TOut>> TryCore(Task<Result<TIn>> resultTask,
                                                Func<TIn, TOut>   func)
        {
            return (await resultTask).Try(func);
        }
    }

    public static ValueTask<Result<TOut>> Try<TIn, TOut>(this Task<Result<TIn>>         resultTask,
                                                          Func<TIn, ValueTask<TOut>>    func)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(TryCore(resultTask, func));

        static async Task<Result<TOut>> TryCore(Task<Result<TIn>>          resultTask,
                                                Func<TIn, ValueTask<TOut>> func)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Try(func);
        }
    }
}

#pragma warning restore CS1591
