namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Transforms the success value(s) using the provided mapping function.
    /// </summary>
    /// <typeparam name="TValue1">Input value type 1.</typeparam>
    /// <typeparam name="TOut1">Output value type 1.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="map">The mapping function.</param>
    /// <returns>A new result with transformed value(s).</returns>
    public static Result<TOut1> Map<TValue1, TOut1>(this Result<TValue1> result, Func<TValue1, TOut1> map)
        where TValue1 : notnull where TOut1 : notnull
    {
        return result.Bind(value => Result.Success(map(value)));
    }

    /// <summary>
    ///     Transforms the result into a new success result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="result">The current result instance.</param>
    /// <param name="map">The mapping function to be applied.</param>
    /// <returns>A new result containing the transformed value.</returns>
    public static Result<TOut> Map<TOut>(this Result result, Func<TOut> map) where TOut : notnull
    {
        return result.Bind(() => Result.Success(map()));
    }
}