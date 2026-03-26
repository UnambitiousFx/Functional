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

    public static async ValueTask<Result<TOut>> Bind<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                Func<TIn, Result<TOut>>     bind)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    public static async ValueTask<Result<TOut>> Bind<TIn, TOut>(this ValueTask<Result<TIn>>        resultTask,
                                                                Func<TIn, ValueTask<Result<TOut>>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value)) {
            result.TryGetError(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await bind(value);
    }

    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Action<TIn>                 tap)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Tap(tap);
    }

    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Func<TIn, ValueTask>        tap)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.TryGetValue(out var value)) {
            await tap(value);
        }

        return result;
    }

    public static async ValueTask<Result<TIn>> Ensure<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                           Func<TIn, bool>             predicate,
                                                           Func<TIn, Failure>          errorFactory)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Ensure(predicate, errorFactory);
    }

    public static async ValueTask<Result<TIn>> Ensure<TIn>(this ValueTask<Result<TIn>>    resultTask,
                                                           Func<TIn, ValueTask<bool>>     predicate,
                                                           Func<TIn, Failure>             errorFactory)
        where TIn : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            return result;
        }

        return await predicate(value)
                   ? result
                   : Result.Failure<TIn>(errorFactory(value));
    }

    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                            Func<Failure, TIn>          recover)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Recover(recover);
    }

    public static async ValueTask<Result<TIn>> Recover<TIn>(this ValueTask<Result<TIn>>     resultTask,
                                                            Func<Failure, ValueTask<TIn>>   recover)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            return result;
        }

        result.TryGetError(out var error);
        return Result.Success(await recover(error!));
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

    public static ValueTask<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>>  resultTask,
                                                          Func<TIn, Result<TOut>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(BindCore(resultTask, bind));

        static async Task<Result<TOut>> BindCore(Task<Result<TIn>>       resultTask,
                                                 Func<TIn, Result<TOut>> bind)
        {
            return (await resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>>             resultTask,
                                                          Func<TIn, ValueTask<Result<TOut>>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(BindCore(resultTask, bind));

        static async Task<Result<TOut>> BindCore(Task<Result<TIn>>                  resultTask,
                                                 Func<TIn, ValueTask<Result<TOut>>> bind)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask,
                                                  Action<TIn>            tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapCore(resultTask, tap));

        static async Task<Result<TIn>> TapCore(Task<Result<TIn>> resultTask,
                                               Action<TIn>       tap)
        {
            return (await resultTask).Tap(tap);
        }
    }

    public static ValueTask<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask,
                                                  Func<TIn, ValueTask>   tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapCore(resultTask, tap));

        static async Task<Result<TIn>> TapCore(Task<Result<TIn>>    resultTask,
                                               Func<TIn, ValueTask> tap)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Tap(tap);
        }
    }

    public static ValueTask<Result<TIn>> Ensure<TIn>(this Task<Result<TIn>> resultTask,
                                                     Func<TIn, bool>        predicate,
                                                     Func<TIn, Failure>     errorFactory)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(EnsureCore(resultTask, predicate, errorFactory));

        static async Task<Result<TIn>> EnsureCore(Task<Result<TIn>>  resultTask,
                                                  Func<TIn, bool>    predicate,
                                                  Func<TIn, Failure> errorFactory)
        {
            return (await resultTask).Ensure(predicate, errorFactory);
        }
    }

    public static ValueTask<Result<TIn>> Ensure<TIn>(this Task<Result<TIn>>        resultTask,
                                                     Func<TIn, ValueTask<bool>>    predicate,
                                                     Func<TIn, Failure>            errorFactory)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(EnsureCore(resultTask, predicate, errorFactory));

        static async Task<Result<TIn>> EnsureCore(Task<Result<TIn>>         resultTask,
                                                  Func<TIn, ValueTask<bool>> predicate,
                                                  Func<TIn, Failure>         errorFactory)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Ensure(predicate, errorFactory);
        }
    }

    public static ValueTask<Result<TIn>> Recover<TIn>(this Task<Result<TIn>> resultTask,
                                                      Func<Failure, TIn>     recover)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(RecoverCore(resultTask, recover));

        static async Task<Result<TIn>> RecoverCore(Task<Result<TIn>>  resultTask,
                                                   Func<Failure, TIn> recover)
        {
            return (await resultTask).Recover(recover);
        }
    }

    public static ValueTask<Result<TIn>> Recover<TIn>(this Task<Result<TIn>>          resultTask,
                                                      Func<Failure, ValueTask<TIn>>   recover)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(RecoverCore(resultTask, recover));

        static async Task<Result<TIn>> RecoverCore(Task<Result<TIn>>           resultTask,
                                                   Func<Failure, ValueTask<TIn>> recover)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Recover(recover);
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
