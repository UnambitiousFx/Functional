namespace UnambitiousFx.Functional;

/// <summary>
/// Represents a mutable collection of metadata key-value pairs.
/// </summary>
public interface IMetadata : IReadOnlyCollection<KeyValuePair<string, object?>>
{
    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    object? this[string key] { get; set; }

    /// <summary>
    /// Determines whether the metadata contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the metadata.</param>
    /// <returns>True if the metadata contains an element with the specified key; otherwise, false.</returns>
    bool ContainsKey(string key);
}
