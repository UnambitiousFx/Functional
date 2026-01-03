using System.Text.RegularExpressions;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a string result value matches the specified regular expression pattern.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string matches the pattern; otherwise a failure with ValidationError.</returns>
    public static Result<string> EnsureMatches(this Result<string> result, string pattern,
        string? message = null, string? field = null)
    {
        return result.Then(value =>
        {
            if (Regex.IsMatch(value, pattern))
            {
                return Result.Success(value);
            }

            var defaultMessage = $"Value must match pattern '{pattern}'.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<string>(new ValidationError([finalMessage]));
        });
    }

    /// <summary>
    ///     Ensures a string result value matches the specified Regex instance.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="regex">The compiled Regex instance to match against.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string matches the regex; otherwise a failure with ValidationError.</returns>
    public static Result<string> EnsureMatches(this Result<string> result, Regex regex,
        string? message = null, string? field = null)
    {
        return result.Then(value =>
        {
            if (regex.IsMatch(value))
            {
                return Result.Success(value);
            }

            var defaultMessage = $"Value must match pattern '{regex}'.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<string>(new ValidationError([finalMessage]));
        });
    }

    /// <summary>
    ///     Ensures a string result value does not match the specified regular expression pattern.
    /// </summary>
    /// <param name="result">The result instance.</param>
    /// <param name="pattern">The regular expression pattern that must not match.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string does not match the pattern; otherwise a failure with ValidationError.</returns>
    public static Result<string> EnsureDoesNotMatch(this Result<string> result, string pattern,
        string? message = null, string? field = null)
    {
        return result.Then(value =>
        {
            if (!Regex.IsMatch(value, pattern))
            {
                return Result.Success(value);
            }

            var defaultMessage = $"Value must not match pattern '{pattern}'.";
            var finalMessage = field is null
                ? message ?? defaultMessage
                : $"{field}: {message ?? defaultMessage}";

            return Result.Failure<string>(new ValidationError([finalMessage]));
        });
    }
}
