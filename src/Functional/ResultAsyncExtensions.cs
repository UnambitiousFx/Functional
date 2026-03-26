using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Direct async fluent operators for <see cref="Result" /> and <see cref="Result{TValue}" /> pipelines.
/// </summary>
public static partial class ResultAsyncExtensions
{
    public static async ValueTask<Result<TOut>> Map<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                               Func<TIn, TOut>             map)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Map(map);
    }

    public static async ValueTask Switch<TIn>(this ValueTask<Result<TIn>> resultTask,
                                              Action<TIn>                 onSuccess,
                                              Action<Failure>             onFailure)
        where TIn : notnull
    {
        var result = await resultTask;
        result.Switch(onSuccess, onFailure);
    }

    public static async ValueTask Switch<TIn>(this ValueTask<Result<TIn>> resultTask,
                                              Func<TIn, ValueTask>        onSuccess,
                                              Func<Failure, ValueTask>    onFailure)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.TryGetValue(out var value)) {
            await onSuccess(value);
            return;
        }

        result.TryGetError(out var error);
        await onFailure(error!);
    }

    public static ValueTask<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask,
                                                         Func<TIn, TOut>        map)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(MapCore(resultTask, map));

        static async Task<Result<TOut>> MapCore(Task<Result<TIn>> resultTask,
                                                Func<TIn, TOut>   map)
        {
            return (await resultTask).Map(map);
        }
    }

    public static ValueTask Switch<TIn>(this Task<Result<TIn>> resultTask,
                                        Action<TIn>            onSuccess,
                                        Action<Failure>        onFailure)
        where TIn : notnull
    {
        return new ValueTask(SwitchCore(resultTask, onSuccess, onFailure));

        static async Task SwitchCore(Task<Result<TIn>> resultTask,
                                     Action<TIn>       onSuccess,
                                     Action<Failure>   onFailure)
        {
            (await resultTask).Switch(onSuccess, onFailure);
        }
    }

    public static ValueTask Switch<TIn>(this Task<Result<TIn>>   resultTask,
                                        Func<TIn, ValueTask>     onSuccess,
                                        Func<Failure, ValueTask> onFailure)
        where TIn : notnull
    {
        return new ValueTask(SwitchCore(resultTask, onSuccess, onFailure));

        static async Task SwitchCore(Task<Result<TIn>>        resultTask,
                                     Func<TIn, ValueTask>     onSuccess,
                                     Func<Failure, ValueTask> onFailure)
        {
            await new ValueTask<Result<TIn>>(resultTask).Switch(onSuccess, onFailure);
        }
    }
}

#pragma warning restore CS1591
