using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

public static partial class MaybeExtensions
{
    /// <summary>
    ///     Executes a side effect when a value is present and returns the original maybe.
    /// </summary>
    public static Maybe<TValue> TapSome<TValue>(this Maybe<TValue> maybe, Action<TValue> tap)
        where TValue : notnull
    {
        return maybe.Tap(tap);
    }

    /// <summary>
    ///     Executes a side effect when no value is present and returns the original maybe.
    /// </summary>
    public static Maybe<TValue> TapNone<TValue>(this Maybe<TValue> maybe, Action tap)
        where TValue : notnull
    {
        if (maybe.IsNone)
        {
            tap();
        }

        return maybe;
    }

    public static async ValueTask<Maybe<TOut>> Map<TIn, TOut>(
        this ValueTask<Maybe<TIn>> maybeTask,
        Func<TIn, TOut> map)
        where TIn : notnull
        where TOut : notnull
    {
        var maybe = await maybeTask;
        return maybe.Map(map);
    }

    public static async ValueTask<Maybe<TOut>> Bind<TIn, TOut>(
        this ValueTask<Maybe<TIn>> maybeTask,
        Func<TIn, Maybe<TOut>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        var maybe = await maybeTask;
        return maybe.Bind(bind);
    }

    public static async ValueTask<Maybe<TOut>> Bind<TIn, TOut>(
        this ValueTask<Maybe<TIn>> maybeTask,
        Func<TIn, ValueTask<Maybe<TOut>>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        var maybe = await maybeTask;
        if (!maybe.Some(out var value))
        {
            return Maybe.None<TOut>();
        }

        return await bind(value);
    }

    public static async ValueTask<Maybe<TValue>> TapSome<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Action<TValue> tap)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.TapSome(tap);
    }

    public static async ValueTask<Maybe<TValue>> TapSome<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Func<TValue, ValueTask> tap)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        if (maybe.Some(out var value))
        {
            await tap(value);
        }

        return maybe;
    }

    public static async ValueTask<Maybe<TValue>> TapNone<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Action tap)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.TapNone(tap);
    }

    public static async ValueTask<Maybe<TValue>> TapNone<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Func<ValueTask> tap)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        if (maybe.IsNone)
        {
            await tap();
        }

        return maybe;
    }

    public static async ValueTask<Result<TValue>> ToResult<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Failure failure)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        return maybe.ToResult(failure);
    }

    public static async ValueTask Switch<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Action<TValue> some,
        Action none)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        maybe.Switch(some, none);
    }

    public static async ValueTask Switch<TValue>(
        this ValueTask<Maybe<TValue>> maybeTask,
        Func<TValue, ValueTask> some,
        Func<ValueTask> none)
        where TValue : notnull
    {
        var maybe = await maybeTask;
        if (maybe.Some(out var value))
        {
            await some(value);
            return;
        }

        await none();
    }

    public static ValueTask<Maybe<TOut>> Map<TIn, TOut>(
        this Task<Maybe<TIn>> maybeTask,
        Func<TIn, TOut> map)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Maybe<TOut>>(MapCore(maybeTask, map));

        static async Task<Maybe<TOut>> MapCore(Task<Maybe<TIn>> maybeTask, Func<TIn, TOut> map)
        {
            return (await maybeTask).Map(map);
        }
    }

    public static ValueTask<Maybe<TOut>> Bind<TIn, TOut>(
        this Task<Maybe<TIn>> maybeTask,
        Func<TIn, Maybe<TOut>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Maybe<TOut>>(BindCore(maybeTask, bind));

        static async Task<Maybe<TOut>> BindCore(Task<Maybe<TIn>> maybeTask, Func<TIn, Maybe<TOut>> bind)
        {
            return (await maybeTask).Bind(bind);
        }
    }

    public static ValueTask<Maybe<TOut>> Bind<TIn, TOut>(
        this Task<Maybe<TIn>> maybeTask,
        Func<TIn, ValueTask<Maybe<TOut>>> bind)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Maybe<TOut>>(BindCore(maybeTask, bind));

        static async Task<Maybe<TOut>> BindCore(Task<Maybe<TIn>> maybeTask, Func<TIn, ValueTask<Maybe<TOut>>> bind)
        {
            return await new ValueTask<Maybe<TIn>>(maybeTask).Bind(bind);
        }
    }

    public static ValueTask<Maybe<TValue>> TapSome<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Action<TValue> tap)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(TapSomeCore(maybeTask, tap));

        static async Task<Maybe<TValue>> TapSomeCore(Task<Maybe<TValue>> maybeTask, Action<TValue> tap)
        {
            return (await maybeTask).TapSome(tap);
        }
    }

    public static ValueTask<Maybe<TValue>> TapSome<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Func<TValue, ValueTask> tap)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(TapSomeCore(maybeTask, tap));

        static async Task<Maybe<TValue>> TapSomeCore(Task<Maybe<TValue>> maybeTask, Func<TValue, ValueTask> tap)
        {
            return await new ValueTask<Maybe<TValue>>(maybeTask).TapSome(tap);
        }
    }

    public static ValueTask<Maybe<TValue>> TapNone<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Action tap)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(TapNoneCore(maybeTask, tap));

        static async Task<Maybe<TValue>> TapNoneCore(Task<Maybe<TValue>> maybeTask, Action tap)
        {
            return (await maybeTask).TapNone(tap);
        }
    }

    public static ValueTask<Maybe<TValue>> TapNone<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Func<ValueTask> tap)
        where TValue : notnull
    {
        return new ValueTask<Maybe<TValue>>(TapNoneCore(maybeTask, tap));

        static async Task<Maybe<TValue>> TapNoneCore(Task<Maybe<TValue>> maybeTask, Func<ValueTask> tap)
        {
            return await new ValueTask<Maybe<TValue>>(maybeTask).TapNone(tap);
        }
    }

    public static ValueTask<Result<TValue>> ToResult<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Failure failure)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(ToResultCore(maybeTask, failure));

        static async Task<Result<TValue>> ToResultCore(Task<Maybe<TValue>> maybeTask, Failure failure)
        {
            return (await maybeTask).ToResult(failure);
        }
    }

    public static ValueTask Switch<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Action<TValue> some,
        Action none)
        where TValue : notnull
    {
        return new ValueTask(SwitchCore(maybeTask, some, none));

        static async Task SwitchCore(Task<Maybe<TValue>> maybeTask, Action<TValue> some, Action none)
        {
            (await maybeTask).Switch(some, none);
        }
    }

    public static ValueTask Switch<TValue>(
        this Task<Maybe<TValue>> maybeTask,
        Func<TValue, ValueTask> some,
        Func<ValueTask> none)
        where TValue : notnull
    {
        return new ValueTask(SwitchCore(maybeTask, some, none));

        static async Task SwitchCore(
            Task<Maybe<TValue>> maybeTask,
            Func<TValue, ValueTask> some,
            Func<ValueTask> none)
        {
            await new ValueTask<Maybe<TValue>>(maybeTask).Switch(some, none);
        }
    }
}

#pragma warning restore CS1591