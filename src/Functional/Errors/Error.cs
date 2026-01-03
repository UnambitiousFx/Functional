namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Default concrete error implementation used in tests and simple failures.
/// </summary>
public record Error : ErrorBase
{
    /// <summary>
    ///     Creates an error with default code and provided message.
    /// </summary>
    public Error(string message) : base(ErrorCodes.Error, message)
    {
    }

    /// <summary>
    ///     Creates an error with explicit code and message.
    /// </summary>
    public Error(string code,
        string message,
        IReadOnlyDictionary<string, object?>? metadata = null)
        : base(code, message, metadata)
    {
    }
}
