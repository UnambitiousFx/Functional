namespace UnambitiousFx.Functional;

/// <summary>
///     Provides extension methods for appending error messages to Result instances.
/// </summary>
public static partial class ResultExtensions
{
    /// <summary>
    ///     Appends a suffix to the error message of the first error in the result.
    /// </summary>
    /// <param name="result">The result to append error message to.</param>
    /// <param name="suffix">The suffix to append to the error message.</param>
    /// <returns>
    ///     A new result with the appended error message if the original result failed, otherwise the original successful
    ///     result.
    /// </returns>
    public static Result AppendError(this Result result, string suffix)
    {
        if (string.IsNullOrEmpty(suffix) || result.IsSuccess)
        {
            return result; // no-op
        }

        return result.MapError(error => error with { Message = error.Message + suffix });
    }

    /// <summary>
    ///     Appends a suffix to the error message of the first error in the result.
    /// </summary>
    /// <param name="result">The result to append error message to.</param>
    /// <param name="suffix">The suffix to append to the error message.</param>
    /// <returns>
    ///     A new result with the appended error message if the original result failed, otherwise the original successful
    ///     result.
    /// </returns>
    public static Result<T1> AppendError<T1>(this Result<T1> result, string suffix) where T1 : notnull
    {
        if (string.IsNullOrEmpty(suffix) || result.IsSuccess)
        {
            return result; // no-op
        }

        return result.MapError(error => error with { Message = error.Message + suffix });
    }
}
