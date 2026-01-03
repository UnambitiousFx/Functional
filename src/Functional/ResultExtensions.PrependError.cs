namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Prepends a prefix to the error message of the first error in the result.
    /// </summary>
    /// <param name="result">The result to prepend error message to.</param>
    /// <param name="prefix">The prefix to prepend to the error message.</param>
    /// <returns>
    ///     A new result with the prepended error message if the original result failed, otherwise the original successful
    ///     result.
    /// </returns>
    public static Result PrependError(this Result result, string prefix)
    {
        if (string.IsNullOrEmpty(prefix) || result.IsSuccess)
        {
            return result; // no-op
        }

        return result.MapError(error => error with { Message = prefix + error.Message });
    }

    /// <summary>
    ///     Prepends a prefix to the error message of the first error in the result.
    /// </summary>
    /// <param name="result">The result to prepend error message to.</param>
    /// <param name="prefix">The prefix to prepend to the error message.</param>
    /// <returns>
    ///     A new result with the prepended error message if the original result failed, otherwise the original successful
    ///     result.
    /// </returns>
    public static Result<T1> PrependError<T1>(this Result<T1> result, string prefix) where T1 : notnull
    {
        if (string.IsNullOrEmpty(prefix) || result.IsSuccess)
        {
            return result; // no-op
        }

        return result.MapError(error => error with { Message = prefix + error.Message });
    }
}
