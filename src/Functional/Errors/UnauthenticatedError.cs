namespace UnambitiousFx.Functional.Errors;

/// <summary>
/// Represents an error that occurs when an operation encounters
/// an authentication failure, indicating that the user or system
/// is not authenticated.
/// </summary>
/// <remarks>
/// This error is used to encapsulate authentication-related failures
/// and includes an optional reason describing the error context,
/// as well as additional metadata for further details.
/// </remarks>
/// <param name="Reason">
/// An optional message that provides additional context or a description
/// of why the authentication error occurred. Defaults to "Unauthenticated."
/// if not specified.
/// </param>
/// <param name="Extra">
/// An optional dictionary containing additional information
/// or metadata about the error. It can be used to pass contextual
/// information related to the authentication failure.
/// </param>
public sealed record UnauthenticatedError(string? Reason = null,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Error(ErrorCodes.Unauthenticated, Reason ?? "Unauthenticated.", Merge(Extra, []));