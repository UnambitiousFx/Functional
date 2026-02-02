namespace UnambitiousFx.Functional.Tests;

public sealed class MetadataDebugViewTests
{
    [Fact]
    public void Constructor_WithMetadata_InitializesCorrectly()
    {
        // Arrange (Given)
        var metadata = Metadata.From(("key", "value"));

        // Act (When)
        var debugView = new MetadataDebugView(metadata);

        // Assert (Then)
        Assert.NotNull(debugView);
    }

    [Fact]
    public void Items_ReturnsMetadataAsArray()
    {
        // Arrange (Given)
        var metadata = Metadata.From(
            ("key1", "value1"),
            ("key2", (object?)42),
            ("key3", (object?)true));
        var debugView = new MetadataDebugView(metadata);

        // Act (When)
        var items = debugView.Items;

        // Assert (Then)
        Assert.Equal(3, items.Length);
        Assert.Contains(items, kvp => kvp.Key == "key1" && kvp.Value as string == "value1");
        Assert.Contains(items, kvp => kvp.Key == "key2" && kvp.Value is int && (int)kvp.Value == 42);
        Assert.Contains(items, kvp => kvp.Key == "key3" && kvp.Value is bool && (bool)kvp.Value == true);
    }

    [Fact]
    public void Items_WithEmptyMetadata_ReturnsEmptyArray()
    {
        // Arrange (Given)
        var metadata = Metadata.Empty;
        var debugView = new MetadataDebugView(metadata);

        // Act (When)
        var items = debugView.Items;

        // Assert (Then)
        Assert.Empty(items);
    }

    [Fact]
    public void Items_PreservesKeyValuePairs()
    {
        // Arrange (Given)
        var metadata = Metadata.From(("testKey", "testValue"));
        var debugView = new MetadataDebugView(metadata);

        // Act (When)
        var items = debugView.Items;

        // Assert (Then)
        Assert.Single(items);
        Assert.Equal("testKey", items[0].Key);
        Assert.Equal("testValue", items[0].Value);
    }
}
