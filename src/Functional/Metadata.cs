using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace UnambitiousFx.Functional;

/// <summary>
///     Represents a case-insensitive collection of metadata key-value pairs.
/// </summary>
[DebuggerTypeProxy(typeof(MetadataDebugView))]
[DebuggerDisplay("Count = {Count}")]
[CollectionBuilder(typeof(Metadata), nameof(Create))]
public sealed record Metadata : IMetadata, IReadOnlyMetadata
{
    /// <summary>
    ///     Gets an empty Metadata instance.
    /// </summary>
    public static readonly Metadata Empty = new();

    private readonly Dictionary<string, object?> _data;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Metadata" /> class.
    /// </summary>
    public Metadata()
    {
        _data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Metadata" /> class with values from a dictionary.
    /// </summary>
    /// <param name="metadata">The dictionary containing initial metadata values</param>
    public Metadata(IReadOnlyDictionary<string, object?> metadata)
    {
        _data = new Dictionary<string, object?>(metadata, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Metadata" /> class with values from another metadata instance.
    /// </summary>
    /// <param name="metadata">The metadata instance containing initial values</param>
    public Metadata(IReadOnlyMetadata metadata)
    {
        _data = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in metadata)
        {
            _data[kv.Key] = kv.Value;
        }
    }

    /// <summary>
    ///     Gets a collection containing all keys in the metadata.
    /// </summary>
    public IEnumerable<string> Keys => _data.Keys;

    /// <summary>
    ///     Gets a collection containing all values in the metadata.
    /// </summary>
    public IEnumerable<object?> Values => _data.Values;

    /// <summary>
    ///     Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set</param>
    /// <returns>The value associated with the specified key</returns>
    public object? this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the metadata key-value pairs.
    /// </summary>
    /// <returns>An enumerator for the metadata collection</returns>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => _data.GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through the metadata key-value pairs.
    /// </summary>
    /// <returns>An enumerator for the metadata collection</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    ///     Gets the number of key-value pairs in the metadata.
    /// </summary>
    public int Count => _data.Count;

    /// <summary>
    ///     Determines whether the metadata contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate</param>
    /// <returns>true if the metadata contains the key; otherwise, false</returns>
    public bool ContainsKey(string key) => _data.ContainsKey(key);

    /// <summary>
    ///     Attempts to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to locate</param>
    /// <param name="value">When this method returns, contains the value associated with the key if found; otherwise, null</param>
    /// <returns>true if the key was found; otherwise, false</returns>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value) => _data.TryGetValue(key, out value);

    /// <summary>
    ///     Returns a string representation of the first specified number of metadata entries.
    /// </summary>
    /// <param name="take">The maximum number of entries to include</param>
    /// <returns>A comma-separated string of key:value pairs</returns>
    public string ToString(int take)
    {
        if (take <= 0 || _data.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(",", _data
            .Take(take)
            .Select(kv => kv.Key + ":" + (kv.Value ?? "null")));
    }

    /// <summary>
    ///     Returns a string representation of all metadata entries.
    /// </summary>
    /// <returns>A comma-separated string of key:value pairs</returns>
    public override string ToString()
    {
        if (_data.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(",", _data
            .Select(kv => kv.Key + ":" + (kv.Value ?? "null")));
    }

    /// <summary>
    ///     Creates a new Metadata instance from a collection of key-value pairs.
    ///     This method supports collection expression syntax in C# 12+.
    /// </summary>
    /// <param name="items">The key-value pairs to include in the metadata</param>
    /// <returns>A new Metadata instance containing the specified items</returns>
    public static Metadata Create(ReadOnlySpan<KeyValuePair<string, object?>> items)
    {
        var metadata = new Metadata();
        foreach (var item in items)
        {
            metadata[item.Key] = item.Value;
        }

        return metadata;
    }

    /// <summary>
    ///     Creates a new Metadata instance from a collection of tuples.
    /// </summary>
    /// <param name="items">The tuples to include in the metadata</param>
    /// <returns>A new Metadata instance containing the specified items</returns>
    public static Metadata From(params (string Key, object? Value)[] items)
    {
        var metadata = new Metadata();
        foreach (var (key, value) in items)
        {
            metadata[key] = value;
        }

        return metadata;
    }

    /// <summary>
    ///     Merges multiple Metadata instances into a new instance.
    ///     Later values overwrite earlier values for duplicate keys.
    /// </summary>
    /// <param name="sources">The metadata instances to merge</param>
    /// <returns>A new Metadata instance containing all merged values</returns>
    public static Metadata Merge(params IReadOnlyMetadata[] sources)
    {
        var metadata = new Metadata();
        foreach (var source in sources)
        foreach (var kv in source)
        {
            metadata[kv.Key] = kv.Value;
        }

        return metadata;
    }
}
