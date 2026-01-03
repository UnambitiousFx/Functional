using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Throws an exception if the result represents a failure.
    /// </summary>
    /// <param name="result">The result instance to evaluate for failure.</param>
    public static void ThrowIfFailed(this Result result)
    {
        result.IfFailure(error => throw error.ToException());
    }
}