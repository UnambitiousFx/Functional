using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public partial class ResultExtensions
{
    /// <summary>
    /// Attempts to compensate for a failure by executing a rollback function. If the rollback also fails,
    /// both errors are combined into an <see cref="AggregateError"/>.
    /// </summary>
    /// <param name="result">The result to compensate.</param>
    /// <param name="rollback">The rollback function to execute on failure, receiving the original error.</param>
    /// <returns>
    /// The original result if successful; the original result if rollback succeeds;
    /// or a failure with an <see cref="AggregateError"/> containing both errors if rollback fails.
    /// </returns>
    public static Result Compensate(this Result result, Func<Error, Result> rollback)
    {
        return result.Match(
            () => result,
            originalError =>
            {
                var rollbackResult = rollback(originalError);

                return rollbackResult.Match(
                    () => result,
                    rollbackError => Result.Failure(new AggregateError(originalError, rollbackError)));
            });
    }

    /// <summary>
    /// Attempts to compensate for a failure by executing a rollback function. If the rollback also fails,
    /// both errors are combined into an <see cref="AggregateError"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="result">The result to compensate.</param>
    /// <param name="rollback">The rollback function to execute on failure, receiving the original error.</param>
    /// <returns>
    /// The original result if successful; the original result if rollback succeeds;
    /// or a failure with an <see cref="AggregateError"/> containing both errors if rollback fails.
    /// </returns>
    public static Result<TValue> Compensate<TValue>(this Result<TValue> result, Func<Error, Result> rollback)
        where TValue : notnull
    {
        return result.Match(
            () => result,
            originalError =>
            {
                var rollbackResult = rollback(originalError);

                return rollbackResult.Match(
                    () => result,
                    rollbackError => Result.Failure<TValue>(new AggregateError(originalError, rollbackError)));
            });
    }
}