namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Represents a conflict error, typically used when a resource already exists or a unique constraint is violated.
/// </summary>
/// <param name="Message">The error message describing the conflict.</param>
/// <param name="Extra">Optional additional metadata about the conflict.</param>
public sealed record ConflictError(
    string Message,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Error(ErrorCodes.Conflict, Message, Merge(Extra, []));
