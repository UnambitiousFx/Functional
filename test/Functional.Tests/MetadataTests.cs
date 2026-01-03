namespace UnambitiousFx.Functional.Tests;

public sealed class MetadataTests
{
    #region Collection Expression Tests

    [Fact]
    public void Metadata_CollectionExpression_Empty()
    {
        Metadata meta = [];

        Assert.Empty(meta);
    }

    [Fact]
    public void Metadata_CollectionExpression_SingleItem()
    {
        Metadata meta = [new KeyValuePair<string, object?>("key", "value")];

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void Metadata_CollectionExpression_MultipleItems()
    {
        Metadata meta =
        [
            new KeyValuePair<string, object?>("traceId", "abc-123"),
            new KeyValuePair<string, object?>("userId", 42),
            new KeyValuePair<string, object?>("timestamp", DateTime.UtcNow)
        ];

        Assert.Equal(3, meta.Count);
        Assert.Equal("abc-123", meta["traceId"]);
        Assert.Equal(42, meta["userId"]);
        Assert.True(meta.ContainsKey("timestamp"));
    }

    [Fact]
    public void Metadata_CollectionExpression_CaseInsensitive()
    {
        Metadata meta =
        [
            new KeyValuePair<string, object?>("KEY", "value1")
        ];

        Assert.Equal("value1", meta["key"]);
        Assert.Equal("value1", meta["KEY"]);
    }

    #endregion

    #region From Method Tests

    [Fact]
    public void Metadata_From_Empty()
    {
        var meta = Metadata.From();

        Assert.Empty(meta);
    }

    [Fact]
    public void Metadata_From_SingleTuple()
    {
        var meta = Metadata.From(("key", "value"));

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void Metadata_From_MultipleTuples()
    {
        var meta = Metadata.From(
            ("traceId", "abc-123"),
            ("userId", 42),
            ("isActive", true)
        );

        Assert.Equal(3, meta.Count);
        Assert.Equal("abc-123", meta["traceId"]);
        Assert.Equal(42, meta["userId"]);
        Assert.True((bool)meta["isActive"]!);
    }

    [Fact]
    public void Metadata_From_DuplicateKeys_LastWins()
    {
        var meta = Metadata.From(
            ("key", "first"),
            ("key", "second")
        );

        Assert.Single(meta);
        Assert.Equal("second", meta["key"]);
    }

    #endregion

    #region Merge Method Tests

    [Fact]
    public void Metadata_Merge_Empty()
    {
        var meta = Metadata.Merge();

        Assert.Empty(meta);
    }

    [Fact]
    public void Metadata_Merge_SingleSource()
    {
        var source = Metadata.From(("key", "value"));
        var meta = Metadata.Merge(source);

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void Metadata_Merge_MultipleSources()
    {
        var meta1 = Metadata.From(("key1", "value1"));
        var meta2 = Metadata.From(("key2", "value2"));
        var meta3 = Metadata.From(("key3", "value3"));

        var merged = Metadata.Merge(meta1, meta2, meta3);

        Assert.Equal(3, merged.Count);
        Assert.Equal("value1", merged["key1"]);
        Assert.Equal("value2", merged["key2"]);
        Assert.Equal("value3", merged["key3"]);
    }

    [Fact]
    public void Metadata_Merge_OverlappingKeys_LastWins()
    {
        var meta1 = Metadata.From(("key", "first"), ("unique1", "a"));
        var meta2 = Metadata.From(("key", "second"), ("unique2", "b"));
        var meta3 = Metadata.From(("key", "third"), ("unique3", "c"));

        var merged = Metadata.Merge(meta1, meta2, meta3);

        Assert.Equal(4, merged.Count);
        Assert.Equal("third", merged["key"]);
        Assert.Equal("a", merged["unique1"]);
        Assert.Equal("b", merged["unique2"]);
        Assert.Equal("c", merged["unique3"]);
    }

    #endregion

    #region MetadataBuilder Tests

    [Fact]
    public void MetadataBuilder_Empty()
    {
        var builder = new MetadataBuilder();
        var meta = builder.Build();

        Assert.Empty(meta);
    }

    [Fact]
    public void MetadataBuilder_Add_SingleItem()
    {
        var meta = new MetadataBuilder()
            .Add("key", "value")
            .Build();

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void MetadataBuilder_Add_ChainedCalls()
    {
        var meta = new MetadataBuilder()
            .Add("key1", "value1")
            .Add("key2", 42)
            .Add("key3", true)
            .Build();

        Assert.Equal(3, meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42, meta["key2"]);
        Assert.True((bool)meta["key3"]!);
    }

    [Fact]
    public void MetadataBuilder_AddIf_ConditionTrue()
    {
        var meta = new MetadataBuilder()
            .AddIf(true, "key", "value")
            .Build();

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void MetadataBuilder_AddIf_ConditionFalse()
    {
        var meta = new MetadataBuilder()
            .AddIf(false, "key", "value")
            .Build();

        Assert.Empty(meta);
    }

    [Fact]
    public void MetadataBuilder_AddIf_FuncConditionTrue()
    {
        var meta = new MetadataBuilder()
            .AddIf(() => 1 + 1 == 2, "key", "value")
            .Build();

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void MetadataBuilder_AddIf_FuncConditionFalse()
    {
        var meta = new MetadataBuilder()
            .AddIf(() => false, "key", "value")
            .Build();

        Assert.Empty(meta);
    }

    [Fact]
    public void MetadataBuilder_AddRange_KeyValuePairs()
    {
        var items = new Dictionary<string, object?> { ["key1"] = "value1", ["key2"] = 42 };

        var meta = new MetadataBuilder()
            .AddRange(items)
            .Build();

        Assert.Equal(2, meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42, meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_AddRange_Metadata()
    {
        var existing = Metadata.From(("key1", "value1"), ("key2", 42));

        var meta = new MetadataBuilder()
            .AddRange(existing)
            .Build();

        Assert.Equal(2, meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42, meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_AddRange_Tuples()
    {
        var meta = new MetadataBuilder()
            .AddRange(("key1", "value1"), ("key2", 42))
            .Build();

        Assert.Equal(2, meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42, meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_WithInitialMetadata()
    {
        var initial = Metadata.From(("existing", "value"));

        var meta = new MetadataBuilder(initial)
            .Add("new", "data")
            .Build();

        Assert.Equal(2, meta.Count);
        Assert.Equal("value", meta["existing"]);
        Assert.Equal("data", meta["new"]);
    }

    [Fact]
    public void MetadataBuilder_ImplicitConversion()
    {
        Metadata meta = new MetadataBuilder()
            .Add("key", "value");

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    #endregion

    #region Result WithMetadata Tests

    [Fact]
    public void Result_WithMetadata_Tuples()
    {
        var result = Result.Success()
            .WithMetadata(("traceId", "abc-123"), ("userId", 42));

        Assert.Equal(2, result.Metadata.Count);
        Assert.Equal("abc-123", result.Metadata["traceId"]);
        Assert.Equal(42, result.Metadata["userId"]);
    }

    [Fact]
    public void Result_WithMetadata_Builder()
    {
        var result = Result.Success()
            .WithMetadata(builder => builder
                .Add("key1", "value1")
                .AddIf(true, "key2", "value2")
                .AddIf(false, "key3", "value3"));

        Assert.Equal(2, result.Metadata.Count);
        Assert.Equal("value1", result.Metadata["key1"]);
        Assert.Equal("value2", result.Metadata["key2"]);
        Assert.False(result.Metadata.ContainsKey("key3"));
    }

    [Fact]
    public void Result_WithMetadata_Builder_PreservesExisting()
    {
        var result = Result.Success()
            .WithMetadata("existing", "value")
            .WithMetadata(builder => builder
                .Add("new", "data"));

        Assert.Equal(2, result.Metadata.Count);
        Assert.Equal("value", result.Metadata["existing"]);
        Assert.Equal("data", result.Metadata["new"]);
    }

    [Fact]
    public void ResultT_WithMetadata_Tuples()
    {
        var result = Result.Success(42)
            .WithMetadata(("traceId", "abc-123"), ("userId", 99));

        Assert.True(result.TryGet(out int value));
        Assert.Equal(42, value);
        Assert.Equal(2, result.Metadata.Count);
        Assert.Equal("abc-123", result.Metadata["traceId"]);
        Assert.Equal(99, result.Metadata["userId"]);
    }

    [Fact]
    public void ResultT_WithMetadata_Builder()
    {
        var result = Result.Success("test")
            .WithMetadata(builder => builder
                .Add("key1", "value1")
                .AddRange(("key2", 42), ("key3", true)));

        Assert.True(result.TryGet(out string? value));
        Assert.Equal("test", value);
        Assert.Equal(3, result.Metadata.Count);
        Assert.Equal("value1", result.Metadata["key1"]);
        Assert.Equal(42, result.Metadata["key2"]);
        Assert.True((bool)result.Metadata["key3"]!);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Integration_CollectionExpression_WithResult()
    {
        Metadata meta =
        [
            new KeyValuePair<string, object?>("traceId", "abc-123"),
            new KeyValuePair<string, object?>("userId", 42)
        ];

        var result = Result.Success()
            .WithMetadata(meta);

        Assert.Equal(2, result.Metadata.Count);
        Assert.Equal("abc-123", result.Metadata["traceId"]);
        Assert.Equal(42, result.Metadata["userId"]);
    }

    [Fact]
    public void Integration_MergeAndBuilder()
    {
        var meta1 = Metadata.From(("key1", "value1"));
        var meta2 = Metadata.From(("key2", "value2"));
        var merged = Metadata.Merge(meta1, meta2);

        var result = Result.Success()
            .WithMetadata(merged)
            .WithMetadata(builder => builder
                .Add("key3", "value3")
                .AddIf(true, "key4", "value4"));

        Assert.Equal(4, result.Metadata.Count);
        Assert.Equal("value1", result.Metadata["key1"]);
        Assert.Equal("value2", result.Metadata["key2"]);
        Assert.Equal("value3", result.Metadata["key3"]);
        Assert.Equal("value4", result.Metadata["key4"]);
    }

    [Fact]
    public void Integration_FluentChaining()
    {
        var result = Result.Success(42)
            .WithMetadata("step1", "init")
            .WithMetadata(("step2", "processing"), ("step3", "validation"))
            .WithMetadata(builder => builder
                .Add("step4", "completion")
                .AddIf(true, "status", "success"));

        Assert.True(result.TryGet(out int value));
        Assert.Equal(42, value);
        Assert.Equal(5, result.Metadata.Count);
        Assert.Equal("init", result.Metadata["step1"]);
        Assert.Equal("processing", result.Metadata["step2"]);
        Assert.Equal("validation", result.Metadata["step3"]);
        Assert.Equal("completion", result.Metadata["step4"]);
        Assert.Equal("success", result.Metadata["status"]);
    }

    #endregion
}
