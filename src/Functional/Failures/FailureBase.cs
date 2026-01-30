using System.Diagnostics;

namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Standard base implementation for an error. Immutable and value-based.
/// </summary>
[DebuggerDisplay("{GetType().Name,nq} [{Code}]: {Message}")]
public abstract record FailureBase : IFailure
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FailureBase" /> class.
    /// </summary>
    /// <param name="code">The machine-readable error code.</param>
    /// <param name="message">The human-readable error message.</param>
    /// <param name="metadata">Optional metadata associated with the error.</param>
    protected FailureBase(string code,
        string message,
        IReadOnlyDictionary<string, object?>? metadata = null)
    {
        Code = code;
        Message = message;
        Metadata = metadata is null || metadata.Count == 0
            ? Metadata.Empty
            : new Metadata(metadata);
    }

    /// <summary>
    ///     Gets the machine-readable error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    ///     Gets or inits the human-readable error message.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    ///     Gets the metadata associated with this error.
    /// </summary>
    public Metadata Metadata { get; }

    /// <summary>
    ///     Merges two metadata dictionaries, with the right dictionary taking precedence for duplicate keys.
    /// </summary>
    /// <param name="left">The left metadata dictionary, may be null.</param>
    /// <param name="right">The right metadata collection to merge.</param>
    /// <returns>A merged dictionary containing all entries.</returns>
    protected static IReadOnlyDictionary<string, object?> Merge(IReadOnlyDictionary<string, object?>? left,
        IEnumerable<KeyValuePair<string, object?>> right)
    {
        var d = left is null
            ? new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, object?>(left, StringComparer.OrdinalIgnoreCase);
        foreach (var kv in right)
        {
            d[kv.Key] = kv.Value;
        }

        return d;
    }
}
