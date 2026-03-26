using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    /// <summary>
    ///     Executes a side effect on the error if the result is a failure, then returns the original result.
    /// </summary>
    public static async ValueTask<Result<TValue>> TapFailure<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                     Action<Failure>                 action)
        where TValue : notnull
    {
        var result = await resultTask;
        if (!result.IsSuccess && result.TryGetError(out var error))
        {
            action(error!);
        }

        return result;
    }

    /// <summary>
    ///     Executes an asynchronous side effect on the error if the result is a failure, then returns the original result.
    /// </summary>
    public static async ValueTask<Result<TValue>> TapFailure<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                     Func<Failure, ValueTask>        action)
        where TValue : notnull
    {
        var result = await resultTask;
        if (!result.IsSuccess && result.TryGetError(out var error))
        {
            await action(error!);
        }

        return result;
    }

    // Task extensions
    /// <summary>
    ///     Executes a side effect on the error if the result is a failure, then returns the original result.
    /// </summary>
    public static ValueTask<Result<TValue>> TapFailure<TValue>(this Task<Result<TValue>> resultTask,
                                                               Action<Failure>           action)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(TapFailureCore(resultTask, action));

        static async Task<Result<TValue>> TapFailureCore(Task<Result<TValue>> resultTask,
                                                         Action<Failure>      action)
        {
            return await new ValueTask<Result<TValue>>(resultTask).TapFailure(action);
        }
    }

    /// <summary>
    ///     Executes an asynchronous side effect on the error if the result is a failure, then returns the original result.
    /// </summary>
    public static ValueTask<Result<TValue>> TapFailure<TValue>(this Task<Result<TValue>>    resultTask,
                                                               Func<Failure, ValueTask>     action)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(TapFailureCore(resultTask, action));

        static async Task<Result<TValue>> TapFailureCore(Task<Result<TValue>>     resultTask,
                                                         Func<Failure, ValueTask> action)
        {
            return await new ValueTask<Result<TValue>>(resultTask).TapFailure(action);
        }
    }
}
