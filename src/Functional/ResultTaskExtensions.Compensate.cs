using UnambitiousFx.Functional.Failures;
using AggregateFailure = UnambitiousFx.Functional.Failures.AggregateFailure;

namespace UnambitiousFx.Functional;

public static partial class ResultTaskExtensions
{
    /// <summary>
    ///     Executes a compensating action if the given <see cref="ResultTask" /> represents a failure.
    /// </summary>
    /// <param name="result">The <see cref="ResultTask" /> to apply compensation to.</param>
    /// <param name="rollback">
    ///     A function that performs a rollback action, accepting the error that caused the failure and returning a new
    ///     <see cref="Result" />.
    ///     If the rollback action also fails, an aggregated error is returned.
    /// </param>
    /// <returns>
    ///     A new <see cref="ResultTask" /> that represents the outcome of the compensation.
    ///     If the original <see cref="ResultTask" /> succeeded, it is returned as-is. If it failed, the result of the rollback
    ///     is returned.
    /// </returns>
    public static ResultTask Compensate(this ResultTask result, Func<Failures.Failure, Result> rollback)
    {
        return CompensateCore(result, rollback).AsAsync();

        static async ValueTask<Result> CompensateCore(ResultTask self, Func<Failures.Failure, Result> rollback)
        {
            var source = await self;
            return source.Match(
                () => source,
                originalError =>
                {
                    var rollbackResult = rollback(originalError);

                    return rollbackResult.Match(
                        () => source,
                        rollbackError => Result.Failure(new AggregateFailure(originalError, rollbackError)));
                });
        }
    }

    /// <summary>
    ///     Provides a mechanism to handle and recover from errors in a <see cref="ResultTask" />
    ///     by executing a compensating action based on the error encountered.
    /// </summary>
    /// <param name="result">
    ///     The <see cref="ResultTask" /> to process, which may represent success or an error.
    /// </param>
    /// <param name="rollback">
    ///     A function to execute when the <paramref name="result" /> represents a failure.
    ///     It accepts the error as input and returns a new <see cref="ResultTask" />.
    /// </param>
    /// <returns>
    ///     A <see cref="ResultTask" /> that represents the result of applying the compensating
    ///     action, or the original result if the compensating action does not resolve the error.
    /// </returns>
    public static ResultTask Compensate(this ResultTask result, Func<Failure, ResultTask> rollback)
    {
        return CompensateCore(result, rollback).AsAsync();

        static async ValueTask<Result> CompensateCore(ResultTask self, Func<Failure, ResultTask> rollback)
        {
            var source = await self;
            return await source.Match<ValueTask<Result>>(
                () => ValueTask.FromResult(source),
                async originalError =>
                {
                    var rollbackResult = await rollback(originalError);
                    return rollbackResult.Match(
                        () => source,
                        rollbackError => Result.Failure(new AggregateFailure(originalError, rollbackError)));
                });
        }
    }

    /// <summary>
    ///     Executes a compensation function when the current asynchronous result task
    ///     represents a failure. The rollback function is used to produce
    ///     a new result, typically for recovery or corrective purposes.
    /// </summary>
    /// <typeparam name="TValue">The type of value encapsulated by the result.</typeparam>
    /// <param name="result">The asynchronous result task to evaluate for failure.</param>
    /// <param name="rollback">
    ///     The function to execute if the asynchronous result task represents a failure.
    ///     Accepts the error causing the failure as an input.
    /// </param>
    /// <returns>
    ///     A new result task representing the original success state if no failure occurred,
    ///     or the output of the rollback logic in case of failure.
    /// </returns>
    public static ResultTask<TValue> Compensate<TValue>(this ResultTask<TValue> result, Func<Failure, Result> rollback)
        where TValue : notnull
    {
        return CompensateCore(result, rollback).AsAsync();

        static async ValueTask<Result<TValue>> CompensateCore(ResultTask<TValue> self, Func<Failure, Result> rollback)
        {
            var source = await self;
            return source.Match(
                () => source,
                originalError =>
                {
                    var rollbackResult = rollback(originalError);

                    return rollbackResult.Match(
                        () => source,
                        rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError, rollbackError)));
                });
        }
    }

    /// <summary>
    ///     Executes a compensating function when the original asynchronous operation represented by the
    ///     <see cref="ResultTask" /> fails. The compensating function takes the failure <see cref="Failure" />
    ///     and produces a new <see cref="Result" />.
    /// </summary>
    /// <param name="result">
    ///     The <see cref="ResultTask" /> on which the compensating function is invoked if the operation fails.
    /// </param>
    /// <param name="rollback">
    ///     A function to handle the failure case. Takes the failed <see cref="Failure" /> as input and returns a new
    ///     <see cref="Result" />.
    /// </param>
    /// <returns>
    ///     A new <see cref="ResultTask" /> that holds the result of the operation after applying compensation
    ///     if the original operation fails.
    /// </returns>
    public static ResultTask<TValue> Compensate<TValue>(this ResultTask<TValue> result,
        Func<Failure, ResultTask> rollback)
        where TValue : notnull
    {
        return CompensateCore(result, rollback).AsAsync();

        static async ValueTask<Result<TValue>> CompensateCore(ResultTask<TValue> self, Func<Failure, ResultTask> rollback)
        {
            var source = await self;
            return await source.Match<ValueTask<Result<TValue>>>(
                () => ValueTask.FromResult(source),
                async originalError =>
                {
                    var rollbackResult = await rollback(originalError);
                    return rollbackResult.Match(
                        () => source,
                        rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError, rollbackError)));
                });
        }
    }

    /// <summary>
    ///     Executes a compensating operation if the original <see cref="ResultTask" /> or <see cref="ResultTask{TValue}" />
    ///     represents a failure. The specific overload determines whether the compensating operation
    ///     returns a synchronous <see cref="Result" /> or an asynchronous <see cref="ResultTask" />.
    /// </summary>
    /// <param name="result">
    ///     The original result to be compensated. This can be a <see cref="ResultTask" /> or
    ///     a <see cref="ResultTask{TValue}" /> representing the result to evaluate.
    /// </param>
    /// <param name="rollback">
    ///     A function that will execute the compensating logic in case the original result
    ///     represents a failure. The rollback function can either return a synchronous <see cref="Result" />,
    ///     a synchronous <see cref="Result{TValue}" />, or an asynchronous <see cref="ResultTask" /> based on the overload.
    /// </param>
    /// <typeparam name="TValue">
    ///     The type of the value encapsulated by the result. This generic type is only applicable
    ///     for the overloads that handle <see cref="ResultTask{TValue}" />.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ResultTask" /> or <see cref="ResultTask{TValue}" /> that represents the compensated result.
    ///     If the original result was successful, the returned result will be the same. If the original result
    ///     indicated failure, the returned result will either be the output of the rollback function or
    ///     aggregate errors from both the original and rollback failures.
    /// </returns>
    public static ResultTask<TValue> Compensate<TValue>(this ResultTask<TValue> result, Func<Result> rollback)
        where TValue : notnull
    {
        return CompensateCore(result, rollback).AsAsync();

        static async ValueTask<Result<TValue>> CompensateCore(ResultTask<TValue> self, Func<Result> rollback)
        {
            var source = await self;
            return source.Match(
                () => source,
                originalError =>
                {
                    var rollbackResult = rollback();

                    return rollbackResult.Match(
                        () => source,
                        rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError, rollbackError)));
                });
        }
    }

    /// <summary>
    ///     Executes a compensation function when the current <see cref="ResultTask" /> represents a failure.
    /// </summary>
    /// <param name="result">
    ///     The current <see cref="ResultTask" /> to be compensated if it represents a failure.
    /// </param>
    /// <param name="rollback">
    ///     A function that generates a compensating <see cref="Result" /> or <see cref="ResultTask" /> based on the error
    ///     encountered in the original <see cref="ResultTask" />.
    /// </param>
    /// <returns>
    ///     A new <see cref="ResultTask" /> instance. If the compensation succeeds, the result will represent a success.
    ///     If the compensation fails, the result will represent a failure, potentially combining the original error
    ///     with the compensation's error.
    /// </returns>
    public static ResultTask<TValue> Compensate<TValue>(this ResultTask<TValue> result, Func<ResultTask> rollback)
        where TValue : notnull
    {
        return CompensateCore(result, rollback).AsAsync();

        static async ValueTask<Result<TValue>> CompensateCore(ResultTask<TValue> self, Func<ResultTask> rollback)
        {
            var source = await self;
            return await source.Match<ValueTask<Result<TValue>>>(
                () => ValueTask.FromResult(source),
                async originalError =>
                {
                    var rollbackResult = await rollback();
                    return rollbackResult.Match(
                        () => source,
                        rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError, rollbackError)));
                });
        }
    }
}