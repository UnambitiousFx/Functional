using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Determines whether the result contains an error of the specified type.
    /// </summary>
    /// <typeparam name="TError">The type of error to check for. Can be an error type or exception type.</typeparam>
    /// <param name="result">The result to check for errors.</param>
    /// <returns>true if the result contains an error of the specified type; otherwise, false.</returns>
    public static bool HasError<TError>(this Result result) where TError : Error
    {
        if (result.IsSuccess)
        {
            return false;
        }

        if (result.TryGet(out var error))
        {
            return false;
        }

        return typeof(Exception).IsAssignableFrom(typeof(TError))
            ? ContainsException(error, typeof(TError))
            : ContainsError<TError>(error);
    }
}
