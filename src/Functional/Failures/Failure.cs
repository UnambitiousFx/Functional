namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Default concrete error implementation used in tests and simple failures.
/// </summary>
public record Failure : FailureBase
{
    /// <summary>
    ///     Creates an error with default code and provided message.
    /// </summary>
    public Failure(string message) : base(ErrorCodes.Error, message)
    {
    }

    /// <summary>
    ///     Creates an error with explicit code and message.
    /// </summary>
    public Failure(string code,
        string message,
        IReadOnlyDictionary<string, object?>? metadata = null)
        : base(code, message, metadata)
    {
    }
}
