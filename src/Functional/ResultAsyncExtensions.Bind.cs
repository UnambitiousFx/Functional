#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
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
        if (!result.TryGetValue(out var value))
        {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await bind(value);
    }

    public static async ValueTask<Result> Bind<TIn>(this ValueTask<Result<TIn>>    resultTask,
                                                    Func<TIn, Result>              bind)
        where TIn : notnull
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    public static async ValueTask<Result> Bind<TIn>(this ValueTask<Result<TIn>>        resultTask,
                                                    Func<TIn, ValueTask<Result>>       bind)
        where TIn : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            result.TryGetFailure(out var error);
            return Result.Failure(error!);
        }

        return await bind(value);
    }

    public static async ValueTask<Result> Bind(this ValueTask<Result> resultTask,
                                               Func<Result>           bind)
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    public static async ValueTask<Result> Bind(this ValueTask<Result>      resultTask,
                                               Func<ValueTask<Result>>     bind)
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            return result;
        }

        return await bind();
    }

    public static async ValueTask<Result<TOut>> Bind<TOut>(this ValueTask<Result>      resultTask,
                                                           Func<Result<TOut>>          bind)
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Bind(bind);
    }

    public static async ValueTask<Result<TOut>> Bind<TOut>(this ValueTask<Result>           resultTask,
                                                           Func<ValueTask<Result<TOut>>>    bind)
        where TOut : notnull
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            result.TryGetFailure(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await bind();
    }

    // Task extensions
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

    public static ValueTask<Result> Bind<TIn>(this Task<Result<TIn>> resultTask,
                                              Func<TIn, Result>      bind)
        where TIn : notnull
    {
        return new ValueTask<Result>(BindCore(resultTask, bind));

        static async Task<Result> BindCore(Task<Result<TIn>>  resultTask,
                                           Func<TIn, Result>  bind)
        {
            return (await resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result> Bind<TIn>(this Task<Result<TIn>>           resultTask,
                                              Func<TIn, ValueTask<Result>>     bind)
        where TIn : notnull
    {
        return new ValueTask<Result>(BindCore(resultTask, bind));

        static async Task<Result> BindCore(Task<Result<TIn>>           resultTask,
                                           Func<TIn, ValueTask<Result>> bind)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result> Bind(this Task<Result> resultTask,
                                         Func<Result>      bind)
    {
        return new ValueTask<Result>(BindCore(resultTask, bind));

        static async Task<Result> BindCore(Task<Result> resultTask,
                                           Func<Result> bind)
        {
            return (await resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result> Bind(this Task<Result>           resultTask,
                                         Func<ValueTask<Result>>    bind)
    {
        return new ValueTask<Result>(BindCore(resultTask, bind));

        static async Task<Result> BindCore(Task<Result>             resultTask,
                                           Func<ValueTask<Result>>  bind)
        {
            return await new ValueTask<Result>(resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result<TOut>> Bind<TOut>(this Task<Result>     resultTask,
                                                     Func<Result<TOut>>    bind)
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(BindCore(resultTask, bind));

        static async Task<Result<TOut>> BindCore(Task<Result>       resultTask,
                                                 Func<Result<TOut>> bind)
        {
            return (await resultTask).Bind(bind);
        }
    }

    public static ValueTask<Result<TOut>> Bind<TOut>(this Task<Result>                resultTask,
                                                     Func<ValueTask<Result<TOut>>>    bind)
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(BindCore(resultTask, bind));

        static async Task<Result<TOut>> BindCore(Task<Result>                   resultTask,
                                                 Func<ValueTask<Result<TOut>>>  bind)
        {
            return await new ValueTask<Result>(resultTask).Bind(bind);
        }
    }
}

#pragma warning restore CS1591
