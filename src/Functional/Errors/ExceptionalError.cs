using System.Diagnostics;

namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Represents an error that wraps an exception, preserving the original exception for inspection.
/// </summary>
[DebuggerDisplay("ExceptionalError: {Exception.GetType().Name,nq}: {Message}")]
[DebuggerTypeProxy(typeof(ExceptionalErrorDebugView))]
public sealed record ExceptionalError : Error
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ExceptionalError" /> class with an exception.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    /// <param name="messageOverride">Optional message to override the exception's message.</param>
    /// <param name="extra">Optional additional metadata about the error.</param>
    public ExceptionalError(Exception exception,
        string? messageOverride = null,
        IReadOnlyDictionary<string, object?>? extra = null)
        : base(ErrorCodes.Exception,
            messageOverride ?? exception.Message,
            Merge(extra, [
                new KeyValuePair<string, object?>("exceptionType", exception.GetType()
                    .FullName)
            ]))
    {
        MessageOverride = messageOverride;
        Extra = extra;
        Exception = exception;
    }

    /// <summary>
    ///     Gets the wrapped exception.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    ///     Gets the message override if one was provided, otherwise null.
    /// </summary>
    public string? MessageOverride { get; }

    /// <summary>
    ///     Gets the additional metadata provided when creating this error.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Extra { get; }
}
