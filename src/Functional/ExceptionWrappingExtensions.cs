using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

/// <summary>
///     Extension methods for wrapping exceptions into functional errors.
/// </summary>
public static class ExceptionWrappingExtensions
{
    /// <summary>
    ///     Wraps an <see cref="Exception" /> into an <see cref="ExceptionalError" /> domain error.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    /// <param name="messageOverride">Optional message override; if null uses the exception's message.</param>
    /// <param name="extra">Optional extra metadata to attach.</param>
    /// <returns>An <see cref="ExceptionalError" /> instance representing the exception.</returns>
    public static ExceptionalError Wrap(this Exception exception,
        string? messageOverride = null,
        IReadOnlyDictionary<string, object?>? extra = null) =>
        new(exception, messageOverride, extra);

    /// <summary>
    ///     Alias for <see cref="Wrap" /> for readability when used in fluent flows.
    /// </summary>
    public static ExceptionalError AsError(this Exception exception,
        string? messageOverride = null,
        IReadOnlyDictionary<string, object?>? extra = null) =>
        exception.Wrap(messageOverride, extra);

    /// <summary>
    ///     Wraps the exception as an ExceptionalError while also prepending a context prefix to its message.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    /// <param name="context">Context prefix (if null or empty returns standard Wrap behavior).</param>
    /// <param name="messageOverride">Optional explicit message override (applied after prefix).</param>
    /// <param name="extra">Optional extra metadata.</param>
    public static ExceptionalError WrapAndPrepend(this Exception exception,
        string context,
        string? messageOverride = null,
        IReadOnlyDictionary<string, object?>? extra = null)
    {
        if (string.IsNullOrEmpty(context))
        {
            return exception.Wrap(messageOverride, extra);
        }

        var finalMessage = context + (messageOverride ?? exception.Message);
        return new ExceptionalError(exception, finalMessage, extra);
    }
}
