using System.Diagnostics.CodeAnalysis;

namespace UnambitiousFx.Functional;

/// <summary>
/// Represents a read-only collection of metadata key-value pairs.
/// </summary>
public interface IReadOnlyMetadata : IReadOnlyCollection<KeyValuePair<string, object?>>
{
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    object? this[string key] { get; }

    /// <summary>
    /// Determines whether the metadata contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the metadata.</param>
    /// <returns>True if the metadata contains an element with the specified key; otherwise, false.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Attempts to retrieve the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <param name="value">When this method returns true, contains the value associated with the key; otherwise, null.</param>
    /// <returns>True if the key was found; otherwise, false.</returns>
    bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value);

    /// <summary>
    /// Converts the metadata to a string representation, limiting the number of entries displayed.
    /// </summary>
    /// <param name="take">The maximum number of entries to include in the string representation.</param>
    /// <returns>A string that represents the metadata.</returns>
    string ToString(int take);
}
