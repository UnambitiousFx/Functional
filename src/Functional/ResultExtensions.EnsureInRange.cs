using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a numeric result value is within the specified range (inclusive).
    /// </summary>
    /// <typeparam name="T">The result value type which must implement IComparable.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the value is in range; otherwise a failure with ValidationError.</returns>
    public static Result<T> EnsureInRange<T>(this Result<T> result, T min, T max,
        string? message = null, string? field = null)
        where T : notnull, IComparable<T>
    {
        return result.Then(value =>
        {
            if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            {
                return Result.Success(value);
            }

            var defaultMessage = $"Value must be between {min} and {max}.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<T>(new ValidationError([finalMessage]));
        });
    }

    /// <summary>
    ///     Ensures a numeric result value is greater than the specified minimum.
    /// </summary>
    /// <typeparam name="T">The result value type which must implement IComparable.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="min">The minimum value (exclusive).</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the value is greater than min; otherwise a failure with ValidationError.</returns>
    public static Result<T> EnsureGreaterThan<T>(this Result<T> result, T min,
        string? message = null, string? field = null)
        where T : notnull, IComparable<T>
    {
        return result.Then(value =>
        {
            if (value.CompareTo(min) > 0)
            {
                return Result.Success(value);
            }

            var defaultMessage = $"Value must be greater than {min}.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<T>(new ValidationError([finalMessage]));
        });
    }

    /// <summary>
    ///     Ensures a numeric result value is less than the specified maximum.
    /// </summary>
    /// <typeparam name="T">The result value type which must implement IComparable.</typeparam>
    /// <param name="result">The result instance.</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the value is less than max; otherwise a failure with ValidationError.</returns>
    public static Result<T> EnsureLessThan<T>(this Result<T> result, T max,
        string? message = null, string? field = null)
        where T : notnull, IComparable<T>
    {
        return result.Then(value =>
        {
            if (value.CompareTo(max) < 0)
            {
                return Result.Success(value);
            }

            var defaultMessage = $"Value must be less than {max}.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<T>(new ValidationError([finalMessage]));
        });
    }
}
