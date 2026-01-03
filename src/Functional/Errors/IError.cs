namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Represents an error reason attached to a Result.
/// </summary>
public interface IError
{
    /// <summary>
    ///     A stable machine readable code (e.g. VALIDATION, NOT_FOUND, etc.).
    /// </summary>
    string Code { get; }

    /// <summary>
    ///     A descriptive textual message providing more details about the error.
    /// </summary>
    string Message { get; }

    /// <summary>
    ///     A collection of key-value pairs that provides additional descriptive information
    ///     related to an error, allowing extended context and metadata for debugging or processing purposes.
    /// </summary>
    Metadata Metadata { get; }
}
