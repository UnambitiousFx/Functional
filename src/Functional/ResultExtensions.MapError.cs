using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <param name="result">The result to map errors for.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Maps errors in the result using the specified mapping function.
        /// </summary>
        /// <param name="mapError">The function to map individual errors.</param>
        /// <returns>A new result with mapped errors if the original result failed, otherwise the original successful result.</returns>
        public Result MapError(Func<Error, Error> mapError)
        {
            return result.Match(
                Result.Success,
                ex => Result.Failure(mapError(ex)));
        }
    }

    /// <param name="result">The result containing the error to map.</param>
    extension<T1>(Result<T1> result) where T1 : notnull
    {
        /// <summary>
        ///     Maps the error in the result using the specified mapping function and optionally preserves metadata.
        /// </summary>
        /// <param name="mapError">The function to map the error.</param>
        /// <returns>A new result with the mapped error if the original result failed; otherwise, the original successful result.</returns>
        public Result<T1> MapError(Func<Error, Error> mapError)
        {
            return result.Match(
                Result.Success,
                ex => Result.Failure<T1>(mapError(ex)));
        }
    }
}