using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions - Select
    public static async ValueTask<Result<TOut>> Select<TIn, TOut>(this ValueTask<Result<TIn>> resultTask,
                                                                  Func<TIn, TOut>             selector)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Map(selector);
    }

    public static async ValueTask<Result<TOut>> Select<TIn, TOut>(this ValueTask<Result<TIn>>    resultTask,
                                                                  Func<TIn, ValueTask<TOut>>     selector)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            result.TryGetError(out var error);
            return Result.Failure<TOut>(error!);
        }

        return Result.Success(await selector(value));
    }

    // ValueTask extensions - SelectMany
    public static async ValueTask<Result<TOut>> SelectMany<TIn, TOut>(this ValueTask<Result<TIn>>    resultTask,
                                                                      Func<TIn, Result<TOut>>        binder)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Bind(binder);
    }

    public static async ValueTask<Result<TOut>> SelectMany<TIn, TOut>(this ValueTask<Result<TIn>>        resultTask,
                                                                      Func<TIn, ValueTask<Result<TOut>>> binder)
        where TIn : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            result.TryGetError(out var error);
            return Result.Failure<TOut>(error!);
        }

        return await binder(value);
    }

    public static async ValueTask<Result<TOut>> SelectMany<TIn, TCollection, TOut>(this ValueTask<Result<TIn>>     resultTask,
                                                                                   Func<TIn, Result<TCollection>>  binder,
                                                                                   Func<TIn, TCollection, TOut>    projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        return result.Bind(left => binder(left)
                              .Map(right => projector(left, right)));
    }

    public static async ValueTask<Result<TOut>> SelectMany<TIn, TCollection, TOut>(this ValueTask<Result<TIn>>            resultTask,
                                                                                   Func<TIn, ValueTask<Result<TCollection>>> binder,
                                                                                   Func<TIn, TCollection, TOut>              projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var left))
        {
            result.TryGetError(out var error);
            return Result.Failure<TOut>(error!);
        }

        var collectionResult = await binder(left);
        return collectionResult.Map(right => projector(left, right));
    }

    // ValueTask extensions - Where
    public static async ValueTask<Result<TValue>> Where<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                Func<TValue, bool>             predicate)
        where TValue : notnull
    {
        var result = await resultTask;
        return result.Where(predicate);
    }

    public static async ValueTask<Result<TValue>> Where<TValue>(this ValueTask<Result<TValue>>  resultTask,
                                                                Func<TValue, ValueTask<bool>>   predicate)
        where TValue : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            return result;
        }

        return await predicate(value)
                   ? result
                   : Result.Failure<TValue>(new ValidationFailure("Result.Where predicate returned false."));
    }

    // Task extensions - Select
    public static ValueTask<Result<TOut>> Select<TIn, TOut>(this Task<Result<TIn>> resultTask,
                                                            Func<TIn, TOut>        selector)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(SelectCore(resultTask, selector));

        static async Task<Result<TOut>> SelectCore(Task<Result<TIn>> resultTask,
                                                   Func<TIn, TOut>   selector)
        {
            return (await resultTask).Map(selector);
        }
    }

    public static ValueTask<Result<TOut>> Select<TIn, TOut>(this Task<Result<TIn>>         resultTask,
                                                            Func<TIn, ValueTask<TOut>>     selector)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(SelectCore(resultTask, selector));

        static async Task<Result<TOut>> SelectCore(Task<Result<TIn>>          resultTask,
                                                   Func<TIn, ValueTask<TOut>> selector)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Select(selector);
        }
    }

    // Task extensions - SelectMany
    public static ValueTask<Result<TOut>> SelectMany<TIn, TOut>(this Task<Result<TIn>>      resultTask,
                                                                Func<TIn, Result<TOut>>     binder)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(SelectManyCore(resultTask, binder));

        static async Task<Result<TOut>> SelectManyCore(Task<Result<TIn>>       resultTask,
                                                       Func<TIn, Result<TOut>> binder)
        {
            return (await resultTask).Bind(binder);
        }
    }

    public static ValueTask<Result<TOut>> SelectMany<TIn, TOut>(this Task<Result<TIn>>             resultTask,
                                                                Func<TIn, ValueTask<Result<TOut>>> binder)
        where TIn : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(SelectManyCore(resultTask, binder));

        static async Task<Result<TOut>> SelectManyCore(Task<Result<TIn>>                  resultTask,
                                                       Func<TIn, ValueTask<Result<TOut>>> binder)
        {
            return await new ValueTask<Result<TIn>>(resultTask).SelectMany(binder);
        }
    }

    public static ValueTask<Result<TOut>> SelectMany<TIn, TCollection, TOut>(this Task<Result<TIn>>         resultTask,
                                                                             Func<TIn, Result<TCollection>> binder,
                                                                             Func<TIn, TCollection, TOut>   projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(SelectManyCore(resultTask, binder, projector));

        static async Task<Result<TOut>> SelectManyCore(Task<Result<TIn>>               resultTask,
                                                       Func<TIn, Result<TCollection>>  binder,
                                                       Func<TIn, TCollection, TOut>    projector)
        {
            return (await resultTask).Bind(left => binder(left)
                                              .Map(right => projector(left, right)));
        }
    }

    public static ValueTask<Result<TOut>> SelectMany<TIn, TCollection, TOut>(this Task<Result<TIn>>                    resultTask,
                                                                             Func<TIn, ValueTask<Result<TCollection>>> binder,
                                                                             Func<TIn, TCollection, TOut>              projector)
        where TIn : notnull
        where TCollection : notnull
        where TOut : notnull
    {
        return new ValueTask<Result<TOut>>(SelectManyCore(resultTask, binder, projector));

        static async Task<Result<TOut>> SelectManyCore(Task<Result<TIn>>                        resultTask,
                                                       Func<TIn, ValueTask<Result<TCollection>>> binder,
                                                       Func<TIn, TCollection, TOut>              projector)
        {
            return await new ValueTask<Result<TIn>>(resultTask).SelectMany(binder, projector);
        }
    }

    // Task extensions - Where
    public static ValueTask<Result<TValue>> Where<TValue>(this Task<Result<TValue>> resultTask,
                                                          Func<TValue, bool>        predicate)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(WhereCore(resultTask, predicate));

        static async Task<Result<TValue>> WhereCore(Task<Result<TValue>> resultTask,
                                                    Func<TValue, bool>   predicate)
        {
            return (await resultTask).Where(predicate);
        }
    }

    public static ValueTask<Result<TValue>> Where<TValue>(this Task<Result<TValue>>       resultTask,
                                                          Func<TValue, ValueTask<bool>>   predicate)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(WhereCore(resultTask, predicate));

        static async Task<Result<TValue>> WhereCore(Task<Result<TValue>>        resultTask,
                                                    Func<TValue, ValueTask<bool>> predicate)
        {
            return await new ValueTask<Result<TValue>>(resultTask).Where(predicate);
        }
    }
}

#pragma warning restore CS1591
