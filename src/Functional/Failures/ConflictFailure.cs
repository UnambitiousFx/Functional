namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents a conflict error, typically used when a resource already exists or a unique constraint is violated.
/// </summary>
/// <param name="Message">The error message describing the conflict.</param>
/// <param name="Extra">Optional additional metadata about the conflict.</param>
public sealed record ConflictFailure(
    string Message,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Failure(ErrorCodes.Conflict, Message, Merge(Extra, []));
