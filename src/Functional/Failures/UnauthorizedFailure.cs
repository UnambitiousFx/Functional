namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents an authorization failure error, typically used when access to a resource is denied.
/// </summary>
public sealed record UnauthorizedFailure : Failure
{
    /// <summary>
    ///     Represents an authorization failure error, typically used when access to a resource is denied.
    /// </summary>
    /// <param name="reason">Optional reason for the authorization failure.</param>
    /// <param name="extra">Optional additional metadata about the error.</param>
    public UnauthorizedFailure(string? reason = null,
        IReadOnlyDictionary<string, object?>? extra = null)
        : base(ErrorCodes.Unauthorized, !string.IsNullOrWhiteSpace(reason) ? reason : "Unauthorized", Merge(extra, []))
    {
        Extra = extra;
    }
    
    /// <summary>Optional additional metadata about the error.</summary>
    public IReadOnlyDictionary<string, object?>? Extra { get; init; }
}