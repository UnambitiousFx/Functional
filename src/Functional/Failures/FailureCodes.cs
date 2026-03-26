namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Standard failure codes used throughout the library.
///     These constants ensure consistency and prevent typos in failure handling.
/// </summary>
public static class FailureCodes
{
    /// <summary>
    ///     Generic failure code for general failures.
    /// </summary>
    public const string Failure = "ERROR";

    /// <summary>
    ///     Failure code for wrapped exceptions.
    /// </summary>
    public const string Exception = "EXCEPTION";

    /// <summary>
    ///     Failure code for aggregate failures containing multiple sub-failures.
    /// </summary>
    public const string AggregateFailure = "AGGREGATE_ERROR";

    /// <summary>
    ///     Failure code for validation failures.
    /// </summary>
    public const string Validation = "VALIDATION";

    /// <summary>
    ///     Failure code for resource not found failures.
    /// </summary>
    public const string NotFound = "NOT_FOUND";

    /// <summary>
    ///     Failure code for conflicting operations (e.g., duplicate keys).
    /// </summary>
    public const string Conflict = "CONFLICT";

    /// <summary>
    ///     Failure code for unauthorized access attempts.
    /// </summary>
    public const string Unauthorized = "UNAUTHORIZED";

    /// <summary>
    ///     Failure code for operations that exceed timeout limits.
    /// </summary>
    public const string Timeout = "TIMEOUT";

    /// <summary>
    ///     Failure code indicating that the user is not authenticated.
    /// </summary>
    public const string Unauthenticated = "UNAUTHENTICATED";
}
