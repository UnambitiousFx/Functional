using UnambitiousFx.Functional.Failures;
using AggregateFailure = UnambitiousFx.Functional.Failures.AggregateFailure;

namespace UnambitiousFx.Functional;

public partial class ResultExtensions
{
    /// <summary>
    ///     Attempts to compensate for a failure by executing a rollback function. If the rollback also fails,
    ///     both errors are combined into an <see cref="Failures.AggregateFailure" />.
    /// </summary>
    /// <param name="result">The result to compensate.</param>
    /// <param name="rollback">The rollback function to execute on failure, receiving the original error.</param>
    /// <returns>
    ///     The original result if successful; the original result if rollback succeeds;
    ///     or a failure with an <see cref="Failures.AggregateFailure" /> containing both errors if rollback fails.
    /// </returns>
    public static Result Compensate(this Result result, Func<Failures.Failure, Result> rollback)
    {
        return result.Match(
            () => result,
            originalError =>
            {
                var rollbackResult = rollback(originalError);

                return rollbackResult.Match(
                    () => result,
                    rollbackError => Result.Failure(new AggregateFailure(originalError, rollbackError)));
            });
    }

    /// <summary>
    ///     Attempts to compensate for a failure by executing a rollback function. If the rollback also fails,
    ///     both errors are combined into an <see cref="AggregateFailure" />.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="result">The result to compensate.</param>
    /// <param name="rollback">The rollback function to execute on failure, receiving the original error.</param>
    /// <returns>
    ///     The original result if successful; the original result if rollback succeeds;
    ///     or a failure with an <see cref="AggregateFailure" /> containing both errors if rollback fails.
    /// </returns>
    public static Result<TValue> Compensate<TValue>(this Result<TValue> result, Func<Failure, Result> rollback)
        where TValue : notnull
    {
        return result.Match(
            () => result,
            originalError =>
            {
                var rollbackResult = rollback(originalError);

                return rollbackResult.Match(
                    () => result,
                    rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError, rollbackError)));
            });
    }

    /// <summary>
    ///     Attempts to compensate for a failure by executing a rollback function. If the rollback also fails,
    ///     an <see cref="AggregateFailure" /> containing both the original and rollback errors is returned.
    /// </summary>
    /// <param name="result">The result to compensate.</param>
    /// <param name="rollback">The rollback function to execute on failure.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <returns>
    ///     The original result if successful; the original result if the rollback succeeds; or a failure with
    ///     an <see cref="AggregateFailure" /> if the rollback fails.
    /// </returns>
    public static Result<TValue> Compensate<TValue>(this Result<TValue> result, Func<Result> rollback)
        where TValue : notnull
    {
        return result.Match(
            () => result,
            originalError =>
            {
                var rollbackResult = rollback();

                return rollbackResult.Match(
                    () => result,
                    rollbackError => Result.Failure<TValue>(new AggregateFailure(originalError, rollbackError)));
            });
    }
}