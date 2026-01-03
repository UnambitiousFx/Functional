namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Standard error codes used throughout the library.
///     These constants ensure consistency and prevent typos in error handling.
/// </summary>
public static class ErrorCodes
{
    /// <summary>
    ///     Generic error code for general failures.
    /// </summary>
    public const string Error = "ERROR";

    /// <summary>
    ///     Error code for wrapped exceptions.
    /// </summary>
    public const string Exception = "EXCEPTION";

    /// <summary>
    ///     Error code for aggregate errors containing multiple sub-errors.
    /// </summary>
    public const string AggregateError = "AGGREGATE_ERROR";

    /// <summary>
    ///     Error code for validation failures.
    /// </summary>
    public const string Validation = "VALIDATION";

    /// <summary>
    ///     Error code for resource not found errors.
    /// </summary>
    public const string NotFound = "NOT_FOUND";

    /// <summary>
    ///     Error code for conflicting operations (e.g., duplicate keys).
    /// </summary>
    public const string Conflict = "CONFLICT";

    /// <summary>
    ///     Error code for unauthorized access attempts.
    /// </summary>
    public const string Unauthorized = "UNAUTHORIZED";

    /// <summary>
    ///     Error code for operations that exceed timeout limits.
    /// </summary>
    public const string Timeout = "TIMEOUT";
}
