using UnambitiousFx.Functional.Failures;

#pragma warning disable CS1591

namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for working with asynchronous Result operations.
/// </summary>
public static partial class ResultAsyncExtensions
{
    // ValueTask extensions
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

    public static async ValueTask<Result<TIn>> Ensure<TIn>(this ValueTask<Result<TIn>>         resultTask,
                                                           Func<TIn, ValueTask<bool>>          predicate,
                                                           Func<TIn, ValueTask<Failure>>       errorFactory)
        where TIn : notnull
    {
        var result = await resultTask;
        if (!result.TryGetValue(out var value))
        {
            return result;
        }

        return await predicate(value)
                   ? result
                   : Result.Failure<TIn>(await errorFactory(value));
    }

    // Task extensions
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

    public static ValueTask<Result<TIn>> Ensure<TIn>(this Task<Result<TIn>>              resultTask,
                                                     Func<TIn, ValueTask<bool>>          predicate,
                                                     Func<TIn, ValueTask<Failure>>       errorFactory)
        where TIn : notnull
    {
        return new ValueTask<Result<TIn>>(EnsureCore(resultTask, predicate, errorFactory));

        static async Task<Result<TIn>> EnsureCore(Task<Result<TIn>>                 resultTask,
                                                  Func<TIn, ValueTask<bool>>        predicate,
                                                  Func<TIn, ValueTask<Failure>>     errorFactory)
        {
            return await new ValueTask<Result<TIn>>(resultTask).Ensure(predicate, errorFactory);
        }
    }
}

#pragma warning restore CS1591
