namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents an error indicating that a requested resource was not found.
/// </summary>
public sealed record NotFoundFailure : Failure
{
    /// <summary>
    /// Represents an error that occurs when a requested resource cannot be found.
    /// </summary>
    /// <param name="resource">The type or name of the missing resource.</param>
    /// <param name="identifier">The unique identifier associated with the missing resource.</param>
    /// <param name="messageOverride">An optional custom message to describe the error. Defaults to a standard message if not provided.</param>
    /// <param name="Extra">Optional additional key-value metadata to provide more context about the error.</param>
    public NotFoundFailure(string resource,
        string identifier,
        string? messageOverride = null,
        IReadOnlyDictionary<string, object?>? Extra = null)
        : base(ErrorCodes.NotFound, messageOverride ?? $"Resource '{resource}' with id '{identifier}' was not found.",
            Merge(Extra,
            [
                new KeyValuePair<string, object?>("resource", resource),
            new KeyValuePair<string, object?>("identifier", identifier)
        ]))
    {
        Resource = resource;
        Identifier = identifier;
        this.Extra = Extra;
    }

    /// <summary>The type or name of the resource that was not found.</summary>
    public string Resource { get;  }

    /// <summary>The identifier of the resource that was not found.</summary>
    public string Identifier { get; }

    /// <summary>Optional additional metadata about the error.</summary>
    public IReadOnlyDictionary<string, object?>? Extra { get; init; }
}
