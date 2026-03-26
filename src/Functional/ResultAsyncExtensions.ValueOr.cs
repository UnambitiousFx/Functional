using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
    /// <summary>
    ///     Returns contained value when successful; otherwise provided fallback.
    /// </summary>
    public static async ValueTask<TValue> ValueOr<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                          TValue                          fallback)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.ValueOr(fallback);
    }

    /// <summary>
    ///     Returns contained value when successful; otherwise value from factory.
    /// </summary>
    public static async ValueTask<TValue> ValueOr<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                          Func<TValue>                    fallbackFactory)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.ValueOr(fallbackFactory);
    }

    /// <summary>
    ///     Returns contained value when successful; otherwise value from asynchronous factory.
    /// </summary>
    public static async ValueTask<TValue> ValueOr<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                          Func<ValueTask<TValue>>         fallbackFactory)
        where TValue : notnull
    {
        var result = await resultTask;
        if (result.TryGetValue(out var value))
        {
            return value;
        }

        return await fallbackFactory();
    }

    /// <summary>
    ///     Returns the contained value when successful; otherwise the default value of the type.
    /// </summary>
    public static async ValueTask<TValue?> ValueOrDefault<TValue>(this ValueTask<Result<TValue>> resultTask)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.ValueOrDefault();
    }

    // Task extensions
    /// <summary>
    ///     Returns contained value when successful; otherwise provided fallback.
    /// </summary>
    public static ValueTask<TValue> ValueOr<TValue>(this Task<Result<TValue>> resultTask,
                                                    TValue                    fallback)
        where TValue : notnull
    {
        return new ValueTask<TValue>(ValueOrCore(resultTask, fallback));

        static async Task<TValue> ValueOrCore(Task<Result<TValue>> resultTask,
                                              TValue               fallback)
        {
            return (await resultTask).ValueOr(fallback);
        }
    }

    /// <summary>
    ///     Returns contained value when successful; otherwise value from factory.
    /// </summary>
    public static ValueTask<TValue> ValueOr<TValue>(this Task<Result<TValue>> resultTask,
                                                    Func<TValue>              fallbackFactory)
        where TValue : notnull
    {
        return new ValueTask<TValue>(ValueOrCore(resultTask, fallbackFactory));

        static async Task<TValue> ValueOrCore(Task<Result<TValue>> resultTask,
                                              Func<TValue>         fallbackFactory)
        {
            return (await resultTask).ValueOr(fallbackFactory);
        }
    }

    /// <summary>
    ///     Returns contained value when successful; otherwise value from asynchronous factory.
    /// </summary>
    public static ValueTask<TValue> ValueOr<TValue>(this Task<Result<TValue>>   resultTask,
                                                    Func<ValueTask<TValue>>     fallbackFactory)
        where TValue : notnull
    {
        return new ValueTask<TValue>(ValueOrCore(resultTask, fallbackFactory));

        static async Task<TValue> ValueOrCore(Task<Result<TValue>>    resultTask,
                                              Func<ValueTask<TValue>> fallbackFactory)
        {
            return await new ValueTask<Result<TValue>>(resultTask).ValueOr(fallbackFactory);
        }
    }

    /// <summary>
    ///     Returns the contained value when successful; otherwise the default value of the type.
    /// </summary>
    public static ValueTask<TValue?> ValueOrDefault<TValue>(this Task<Result<TValue>> resultTask)
        where TValue : notnull
    {
        return new ValueTask<TValue?>(ValueOrDefaultCore(resultTask));

        static async Task<TValue?> ValueOrDefaultCore(Task<Result<TValue>> resultTask)
        {
            return (await resultTask).ValueOrDefault();
        }
    }
}
