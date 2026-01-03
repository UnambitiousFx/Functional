namespace UnambitiousFx.Functional;

/// <summary>
///     Provides a fluent API for building Metadata instances.
/// </summary>
public sealed class MetadataBuilder
{
    private readonly Metadata _metadata;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MetadataBuilder" /> class.
    /// </summary>
    public MetadataBuilder()
    {
        _metadata = new Metadata();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MetadataBuilder" /> class with initial metadata.
    /// </summary>
    /// <param name="initial">The initial metadata to start with</param>
    public MetadataBuilder(IReadOnlyMetadata initial)
    {
        _metadata = new Metadata(initial);
    }

    /// <summary>
    ///     Adds a key-value pair to the metadata.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder Add(string key, object? value)
    {
        _metadata[key] = value;
        return this;
    }

    /// <summary>
    ///     Adds a key-value pair to the metadata if the condition is true.
    /// </summary>
    /// <param name="condition">The condition to evaluate</param>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder AddIf(bool condition, string key, object? value)
    {
        if (condition)
        {
            _metadata[key] = value;
        }

        return this;
    }

    /// <summary>
    ///     Adds a key-value pair to the metadata if the condition function returns true.
    /// </summary>
    /// <param name="condition">The condition function to evaluate</param>
    /// <param name="key">The key</param>
    /// <param name="value">The value</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder AddIf(Func<bool> condition, string key, object? value)
    {
        if (condition())
        {
            _metadata[key] = value;
        }

        return this;
    }

    /// <summary>
    ///     Adds multiple key-value pairs to the metadata.
    /// </summary>
    /// <param name="items">The key-value pairs to add</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder AddRange(IEnumerable<KeyValuePair<string, object?>> items)
    {
        foreach (var item in items)
        {
            _metadata[item.Key] = item.Value;
        }

        return this;
    }

    /// <summary>
    ///     Adds multiple key-value pairs from another metadata instance.
    /// </summary>
    /// <param name="metadata">The metadata to merge</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder AddRange(IReadOnlyMetadata metadata)
    {
        foreach (var item in metadata)
        {
            _metadata[item.Key] = item.Value;
        }

        return this;
    }

    /// <summary>
    ///     Adds multiple key-value pairs from tuples.
    /// </summary>
    /// <param name="items">The tuples to add</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder AddRange(params (string Key, object? Value)[] items)
    {
        foreach (var (key, value) in items)
        {
            _metadata[key] = value;
        }

        return this;
    }

    /// <summary>
    ///     Removes a key from the metadata if it exists.
    /// </summary>
    /// <param name="key">The key to remove</param>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder Remove(string key)
    {
        _metadata.TryGetValue(key, out _);
        return this;
    }

    /// <summary>
    ///     Clears all metadata.
    /// </summary>
    /// <returns>The current builder instance for method chaining</returns>
    public MetadataBuilder Clear()
    {
        foreach (var key in _metadata.Keys.ToList())
        {
            _metadata.TryGetValue(key, out _);
        }

        return this;
    }

    /// <summary>
    ///     Builds and returns the final Metadata instance.
    /// </summary>
    /// <returns>A new Metadata instance with all added values</returns>
    public Metadata Build() => new(_metadata);

    /// <summary>
    ///     Implicitly converts a MetadataBuilder to Metadata.
    /// </summary>
    /// <param name="builder">The builder to convert</param>
    public static implicit operator Metadata(MetadataBuilder builder) => builder.Build();
}
