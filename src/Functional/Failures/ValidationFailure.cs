namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents a validation error containing one or more validation failure messages.
/// </summary>
public sealed record ValidationFailure : Failure
{
    /// <summary>
    ///     Represents a validation error containing one or more validation failure messages.
    /// </summary>
    /// <param name="failures">The collection of validation failure messages.</param>
    /// <param name="extra">Optional additional metadata about the validation failures.</param>
    public ValidationFailure(IReadOnlyList<string> failures,
        IReadOnlyDictionary<string, object?>? extra = null)
        : base(ErrorCodes.Validation, failures.Count == 0
                ? "Validation failed."
                : string.Join("; ", failures),
            Merge(extra, [new KeyValuePair<string, object?>("failures", failures)]))
    {
        Failures = failures;
        Extra = extra;
    }

    /// <summary>
    ///     Represents a validation error containing one or more validation failure messages.
    /// </summary>
    /// <param name="validationMessage">The validation failure message.</param>
    /// <param name="extra">Optional additional metadata about the validation failures.</param>
    public ValidationFailure(string validationMessage, IReadOnlyDictionary<string, object?>? extra = null)
        : this([validationMessage], extra)
    {
    }

    /// <summary>The collection of validation failure messages.</summary>
    public IReadOnlyList<string> Failures { get; init; }

    /// <summary>Optional additional metadata about the validation failures.</summary>
    public IReadOnlyDictionary<string, object?>? Extra { get; init; }
}