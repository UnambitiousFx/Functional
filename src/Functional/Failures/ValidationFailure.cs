namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents a validation error containing one or more validation failure messages.
/// </summary>
/// <param name="Failures">The collection of validation failure messages.</param>
/// <param name="Extra">Optional additional metadata about the validation failures.</param>
public sealed record ValidationFailure(
    IReadOnlyList<string> Failures,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Failure(ErrorCodes.Validation, Failures.Count == 0
            ? "Validation failed."
            : string.Join("; ", Failures),
        Merge(Extra, [new KeyValuePair<string, object?>("failures", Failures)]));
