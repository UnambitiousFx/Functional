namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Represents an authorization failure error, typically used when access to a resource is denied.
/// </summary>
/// <param name="Reason">Optional reason for the authorization failure.</param>
/// <param name="Extra">Optional additional metadata about the error.</param>
public sealed record UnauthorizedError(
    string? Reason = null,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Error(ErrorCodes.Unauthorized, Reason ?? "Unauthorized.", Merge(Extra, []));