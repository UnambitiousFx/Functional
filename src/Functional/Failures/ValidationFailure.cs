namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents a validation error containing one or more validation failure messages.
/// </summary>
public sealed record ValidationFailure : Failure
{
    /// <summary>
    ///     Represents a validation error containing one or more validation failure messages.
    /// </summary>
    /// <param name="Failures">The collection of validation failure messages.</param>
    /// <param name="Extra">Optional additional metadata about the validation failures.</param>
    public ValidationFailure(IReadOnlyList<string> Failures,
        IReadOnlyDictionary<string, object?>? Extra = null)
        : base(ErrorCodes.Validation, Failures.Count == 0
                ? "Validation failed."
                : string.Join("; ", Failures),
            Merge(Extra, [new KeyValuePair<string, object?>("failures", Failures)]))
    {
        this.Failures = Failures;
        this.Extra = Extra;
    }

    public ValidationFailure(string validationMessage, IReadOnlyDictionary<string, object?>? extra = null)
        : this([validationMessage], extra)
    {
    }

    /// <summary>The collection of validation failure messages.</summary>
    public IReadOnlyList<string> Failures { get; init; }

    /// <summary>Optional additional metadata about the validation failures.</summary>
    public IReadOnlyDictionary<string, object?>? Extra { get; init; }
}