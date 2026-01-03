using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a projected inner reference value is not null. If null, returns a Failure Result with a ValidationError.
    /// </summary>
    /// <typeparam name="T">The result value type.</typeparam>
    /// <typeparam name="TInner">The inner reference type to check for null.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="selector">Function to select the inner value to check.</param>
    /// <param name="message">Validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the inner value is not null; otherwise a failure with ValidationError.</returns>
    public static Result<T> EnsureNotNull<T, TInner>(this Result<T> result, Func<T, TInner?> selector, string message,
        string? field = null) where T : notnull where TInner : class
    {
        return result.Then(value =>
        {
            var inner = selector(value);
            if (inner is not null)
            {
                return Result.Success(value);
            }

            var finalMessage = field is null
                ? message
                : $"{field}: {message}";
            return Result.Failure<T>(new ValidationError([finalMessage]));
        });
    }
}
