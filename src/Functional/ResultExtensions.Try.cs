namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Executes a function that may throw, catching exceptions and converting to Result failure.
    /// </summary>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    /// <typeparam name="TOut1">Output value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="func">The function to execute.</param>
    /// <returns>A new result with transformed value(s) or failure.</returns>
    public static Result<TOut1> Try<TValue1, TOut1>(this Result<TValue1> result, Func<TValue1, TOut1> func)
        where TValue1 : notnull where TOut1 : notnull
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
                return Result.Failure<TOut1>(ex);
            }
        });
    }
}