using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
  
    /// <summary>
    ///     Executes a side effect if the result is a failure, then returns the original result.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Action to execute on failure.</param>
    /// <returns>The original result unchanged.</returns>
    public static Result TapError(this Result result, Action<Error> tap)
    {
        result.IfFailure(tap);
        return result;
    }

    /// <summary>
    ///     Executes a side effect if the result is a failure, then returns the original result.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Action to execute on failure.</param>
    /// <returns>The original result unchanged.</returns>
    public static Result<TValue1> TapError<TValue1>(this Result<TValue1> result, Action<Error> tap)
        where TValue1 : notnull
    {
        result.IfFailure(tap);
        return result;
    }
}