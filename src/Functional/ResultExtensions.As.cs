namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for appending error messages to Result instances.
/// </summary>
public static partial class ResultExtensions
{
    /// <summary>
    /// Converts the current <see cref="Result{TValue}"/> instance to a <see cref="Result{TOut}"/> instance
    /// by replacing the success value with the specified new value, while preserving any error state.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the value in the source <see cref="Result{TValue}"/>.
    /// </typeparam>
    /// <typeparam name="TOut">
    /// The type of the value in the resulting <see cref="Result{TOut}"/>.
    /// </typeparam>
    /// <param name="result">
    /// The current <see cref="Result{TValue}"/> instance to convert.
    /// </param>
    /// <param name="value">
    /// The new value to assign to the resulting <see cref="Result{TOut}"/> if the current result is successful.
    /// </param>
    /// <returns>
    /// A <see cref="Result{TOut}"/> instance with the specified new success value if the operation succeeds.
    /// If the current result contains an error, the error state is preserved.
    /// </returns>
    public static Result<TOut> As<TValue, TOut>(this Result<TValue> result, TOut value)
        where TValue : notnull where TOut : notnull
    {
        return result.Bind(_ => Result.Success(value));
    }
}
