namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Executes a side effect if the result is successful, then returns the original result.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Action to execute on success.</param>
    /// <returns>The original result unchanged.</returns>
    public static Result Tap(this Result result, Action tap)
    {
        result.IfSuccess(tap);
        return result;
    }

    /// <summary>
    ///     Executes a side effect if the result is successful, then returns the original result.
    /// </summary>
    /// <typeparam name="TValue1">Value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="tap">Action to execute on success.</param>
    /// <returns>The original result unchanged.</returns>
    public static Result<TValue1> Tap<TValue1>(this Result<TValue1> result, Action<TValue1> tap) where TValue1 : notnull
    {
        result.IfSuccess(tap);
        return result;
    }
}