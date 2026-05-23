namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents a bad request error, typically used when the request is malformed or contains invalid input
///     that is not covered by field-level validation (e.g., invalid format, missing required parameters).
/// </summary>
/// <param name="Message">The error message describing why the request is invalid.</param>
/// <param name="Extra">Optional additional metadata about the bad request.</param>
public record BadRequestFailure(string                                Message,
                                IReadOnlyDictionary<string, object?>? Extra = null)
    : Failure(FailureCodes.BadRequest, Message, Merge(Extra, []));
