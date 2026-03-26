using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions - Result
    public static async ValueTask<Result> Tap(this ValueTask<Result> resultTask,
                                              Action                  tap)
    {
        var result = await resultTask;
        return result.Tap(tap);
    }

    public static async ValueTask<Result> Tap(this ValueTask<Result> resultTask,
                                              Func<ValueTask>        tap)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await tap();
        }

        return result;
    }

    public static async ValueTask<Result> TapFailure(this ValueTask<Result> resultTask,
                                                    Action<Failure>        tap)
    {
        var result = await resultTask;
        return result.TapFailure(tap);
    }

    public static async ValueTask<Result> TapFailure(this ValueTask<Result>   resultTask,
                                                    Func<Failure, ValueTask> tap)
    {
        var result = await resultTask;
        if (result.TryGetFailure(out var error))
        {
            await tap(error);
        }

        return result;
    }

    // ValueTask extensions - Result<TValue>
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
        if (result.TryGetValue(out var value))
        {
            await tap(value);
        }

        return result;
    }

    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Action                      tap)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Tap(tap);
    }

    public static async ValueTask<Result<TIn>> Tap<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                        Func<ValueTask>             tap)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await tap();
        }

        return result;
    }

    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Action<TIn>                 tap)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.TapIf(predicate, tap);
    }

    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Func<TIn, ValueTask>        tap)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.TryGetValue(out var value) && predicate(value))
        {
            await tap(value);
        }

        return result;
    }

    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Action                      tap)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.TapIf(predicate, tap);
    }

    public static async ValueTask<Result<TIn>> TapIf<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                          Func<TIn, bool>             predicate,
                                                          Func<ValueTask>             tap)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.TryGetValue(out var value) && predicate(value))
        {
            await tap();
        }

        return result;
    }

    public static async ValueTask<Result<TIn>> TapFailure<TIn>(this ValueTask<Result<TIn>> resultTask,
                                                             Action<Failure>            tap)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.TapFailure(tap);
    }

    public static async ValueTask<Result<TIn>> TapFailure<TIn>(this ValueTask<Result<TIn>>  resultTask,
                                                             Func<Failure, ValueTask>     tap)
        where TIn : notnull
    {
        var result = await resultTask;
        if (result.TryGetFailure(out var error))
        {
            await tap(error);
        }

        return result;
    }

    // Task extensions - Result
    public static ValueTask<Result> Tap(this Task<Result> resultTask,
                                        Action            tap)
    {
        return new ValueTask<Result>(TapCore(resultTask, tap));

        static async Task<Result> TapCore(Task<Result> resultTask,
                                          Action       tap)
        {
            return (await resultTask).Tap(tap);
        }
    }

    public static ValueTask<Result> Tap(this Task<Result>   resultTask,
                                        Func<ValueTask>     tap)
    {
        return new ValueTask<Result>(TapCore(resultTask, tap));

        static async Task<Result> TapCore(Task<Result>    resultTask,
                                          Func<ValueTask> tap)
        {
            return await new ValueTask<Result>(resultTask).Tap(tap);
        }
    }

    public static ValueTask<Result> TapFailure(this Task<Result>   resultTask,
                                             Action<Failure>    tap)
    {
        return new ValueTask<Result>(TapFailureCore(resultTask, tap));

        static async Task<Result> TapFailureCore(Task<Result>    resultTask,
                                               Action<Failure> tap)
        {
            return (await resultTask).TapFailure(tap);
        }
    }

    public static ValueTask<Result> TapFailure(this Task<Result>            resultTask,
                                             Func<Failure, ValueTask>    tap)
    {
        return new ValueTask<Result>(TapFailureCore(resultTask, tap));

        static async Task<Result> TapFailureCore(Task<Result>                resultTask,
                                               Func<Failure, ValueTask>    tap)
        {
            return await new ValueTask<Result>(resultTask).TapFailure(tap);
        }
    }

    // Task extensions - Result<TValue>
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

    public static ValueTask<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask,
                                                  Action                 tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapCore(resultTask, tap));

        static async Task<Result<TIn>> TapCore(Task<Result<TIn>> resultTask,
                                               Action            tap)
        {
            return (await resultTask).Tap(tap);
        }
    }

    public static ValueTask<Result<TIn>> Tap<TIn>(this Task<Result<TIn>> resultTask,
                                                  Func<ValueTask>        tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapCore(resultTask, tap));

        static async Task<Result<TIn>> TapCore(Task<Result<TIn>> resultTask,
                                               Func<ValueTask>   tap)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Tap(tap);
        }
    }

    public static ValueTask<Result<TIn>> TapIf<TIn>(this Task<Result<TIn>> resultTask,
                                                    Func<TIn, bool>       predicate,
                                                    Action<TIn>           tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapIfCore(resultTask, predicate, tap));

        static async Task<Result<TIn>> TapIfCore(Task<Result<TIn>> resultTask,
                                                 Func<TIn, bool>   predicate,
                                                 Action<TIn>       tap)
        {
            return (await resultTask).TapIf(predicate, tap);
        }
    }

    public static ValueTask<Result<TIn>> TapIf<TIn>(this Task<Result<TIn>>   resultTask,
                                                    Func<TIn, bool>         predicate,
                                                    Func<TIn, ValueTask>    tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapIfCore(resultTask, predicate, tap));

        static async Task<Result<TIn>> TapIfCore(Task<Result<TIn>>      resultTask,
                                                 Func<TIn, bool>        predicate,
                                                 Func<TIn, ValueTask>   tap)
        {
            return await new ValueTask<Result<TIn>>(resultTask).TapIf(predicate, tap);
        }
    }

    public static ValueTask<Result<TIn>> TapIf<TIn>(this Task<Result<TIn>> resultTask,
                                                    Func<TIn, bool>       predicate,
                                                    Action                tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapIfCore(resultTask, predicate, tap));

        static async Task<Result<TIn>> TapIfCore(Task<Result<TIn>> resultTask,
                                                 Func<TIn, bool>   predicate,
                                                 Action            tap)
        {
            return (await resultTask).TapIf(predicate, tap);
        }
    }

    public static ValueTask<Result<TIn>> TapIf<TIn>(this Task<Result<TIn>> resultTask,
                                                    Func<TIn, bool>       predicate,
                                                    Func<ValueTask>       tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapIfCore(resultTask, predicate, tap));

        static async Task<Result<TIn>> TapIfCore(Task<Result<TIn>> resultTask,
                                                 Func<TIn, bool>   predicate,
                                                 Func<ValueTask>   tap)
        {
            return await new ValueTask<Result<TIn>>(resultTask).TapIf(predicate, tap);
        }
    }

    public static ValueTask<Result<TIn>> TapFailure<TIn>(this Task<Result<TIn>> resultTask,
                                                       Action<Failure>        tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapFailureCore(resultTask, tap));

        static async Task<Result<TIn>> TapFailureCore(Task<Result<TIn>> resultTask,
                                                    Action<Failure>   tap)
        {
            return (await resultTask).TapFailure(tap);
        }
    }

    public static ValueTask<Result<TIn>> TapFailure<TIn>(this Task<Result<TIn>>       resultTask,
                                                       Func<Failure, ValueTask>     tap)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(TapFailureCore(resultTask, tap));

        static async Task<Result<TIn>> TapFailureCore(Task<Result<TIn>>           resultTask,
                                                    Func<Failure, ValueTask>    tap)
        {
            return await new ValueTask<Result<TIn>>(resultTask).TapFailure(tap);
        }
    }
}

#pragma warning restore CS1591
