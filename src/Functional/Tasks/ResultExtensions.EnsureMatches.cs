using System.Text.RegularExpressions;

namespace UnambitiousFx.Functional.Tasks;

public static partial class ResultExtensions
{
    /// <summary>
    ///     Ensures a Task-wrapped string result value matches the specified regular expression pattern.
    /// </summary>
    /// <param name="resultTask">The Task-wrapped result instance.</param>
    /// <param name="pattern">The regular expression pattern to match.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string matches the pattern; otherwise a failure with ValidationError.</returns>
    public static async Task<Result<string>> EnsureMatches(this Task<Result<string>> resultTask, string pattern,
        string? message = null, string? field = null)
    {
        var result = await resultTask;
        return result.EnsureMatches(pattern, message, field);
    }

    /// <summary>
    ///     Ensures a Task-wrapped string result value matches the specified Regex instance.
    /// </summary>
    /// <param name="resultTask">The Task-wrapped result instance.</param>
    /// <param name="regex">The compiled Regex instance to match against.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string matches the regex; otherwise a failure with ValidationError.</returns>
    public static async Task<Result<string>> EnsureMatches(this Task<Result<string>> resultTask, Regex regex,
        string? message = null, string? field = null)
    {
        var result = await resultTask;
        return result.EnsureMatches(regex, message, field);
    }

    /// <summary>
    ///     Ensures a Task-wrapped string result value does not match the specified regular expression pattern.
    /// </summary>
    /// <param name="resultTask">The Task-wrapped result instance.</param>
    /// <param name="pattern">The regular expression pattern that must not match.</param>
    /// <param name="message">Optional validation error message.</param>
    /// <param name="field">Optional field name for the error message.</param>
    /// <returns>The original result if the string does not match the pattern; otherwise a failure with ValidationError.</returns>
    public static async Task<Result<string>> EnsureDoesNotMatch(this Task<Result<string>> resultTask, string pattern,
        string? message = null, string? field = null)
    {
        var result = await resultTask;
        return result.EnsureDoesNotMatch(pattern, message, field);
    }
}
