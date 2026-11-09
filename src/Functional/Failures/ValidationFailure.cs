namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents a validation error containing one or more validation errors.
/// </summary>
public sealed record ValidationFailure : Failure
{
    /// <summary>
    ///     Represents a validation error built from structured errors (field, message, code).
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    /// <param name="extra">Optional additional metadata about the validation failures.</param>
    public ValidationFailure(IReadOnlyList<ValidationError>        errors,
                             IReadOnlyDictionary<string, object?>? extra = null)
        : this(errors, errors.Select(e => e.Message).ToArray(), extra)
    {
    }

    /// <summary>
    ///     Represents a validation error containing one or more validation failure messages not tied to a field.
    /// </summary>
    /// <param name="failures">The collection of validation failure messages.</param>
    /// <param name="extra">Optional additional metadata about the validation failures.</param>
    public ValidationFailure(IReadOnlyList<string>                 failures,
                             IReadOnlyDictionary<string, object?>? extra = null)
        : this(failures.Select(f => new ValidationError(null, f)).ToArray(), failures, extra)
    {
    }

    /// <summary>
    ///     Represents a validation error containing one or more validation failure messages.
    /// </summary>
    /// <param name="validationMessage">The validation failure message.</param>
    /// <param name="extra">Optional additional metadata about the validation failures.</param>
    public ValidationFailure(string                                validationMessage,
                             IReadOnlyDictionary<string, object?>? extra = null)
        : this([validationMessage], extra)
    {
    }

    private ValidationFailure(IReadOnlyList<ValidationError>        errors,
                              IReadOnlyList<string>                 failures,
                              IReadOnlyDictionary<string, object?>? extra)
        : base(FailureCodes.Validation, failures.Count == 0
                                          ? "Validation failed."
                                          : string.Join("; ", failures),
               Merge(extra, [new KeyValuePair<string, object?>("failures", failures)]))
    {
        Errors   = errors;
        Failures = failures;
        Extra    = extra;
    }

    /// <summary>The structured validation errors.</summary>
    public IReadOnlyList<ValidationError> Errors { get; }

    /// <summary>The validation failure messages, one per entry in <see cref="Errors" />.</summary>
    public IReadOnlyList<string> Failures { get; }

    /// <summary>Optional additional metadata about the validation failures.</summary>
    public IReadOnlyDictionary<string, object?>? Extra { get; init; }
}
