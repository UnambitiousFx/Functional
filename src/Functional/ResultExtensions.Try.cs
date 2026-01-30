namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Executes an action that may throw, catching exceptions and converting them to a Result failure.
    /// </summary>
    /// <param name="result">The result instance to bind the action to.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>A result indicating success if no exception occurs, or a failure result if an exception is thrown.</returns>
    public static Result Try(this Result result, Action action)
    {
        return result.Bind(() =>
        {
            try
            {
                action();
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        });
    }

    /// <summary>
    ///     Executes a function that may throw, catching exceptions and converting to Result failure.
    /// </summary>
    /// <typeparam name="TValue">Input value type 1.</typeparam>
    /// <typeparam name="TOut">Output value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A new result with transformed value(s) or failure.</returns>
    public static Result<TOut> Try<TValue, TOut>(this Result<TValue> result, Func<TValue, TOut> func)
        where TValue : notnull where TOut : notnull
    {
        return result.Bind(value =>
        {
            try
            {
                var newValue = func(value);
                return Result.Success(newValue);
            }
            catch (Exception ex)
            {
                return Result.Failure<TOut>(ex);
            }
        });
    }
}