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
            new KeyValuePair<string, object?>("traceId",   "abc-123"),
            new KeyValuePair<string, object?>("userId",    42),
            new KeyValuePair<string, object?>("timestamp", DateTime.UtcNow)
        ];

        Assert.Equal(3,         meta.Count);
        Assert.Equal("abc-123", meta["traceId"]);
        Assert.Equal(42,        meta["userId"]);
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

        Assert.Equal(3,         meta.Count);
        Assert.Equal("abc-123", meta["traceId"]);
        Assert.Equal(42,        meta["userId"]);
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
        var meta   = Metadata.Merge(source);

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

        Assert.Equal(3,        merged.Count);
        Assert.Equal("value1", merged["key1"]);
        Assert.Equal("value2", merged["key2"]);
        Assert.Equal("value3", merged["key3"]);
    }

    [Fact]
    public void Metadata_Merge_OverlappingKeys_LastWins()
    {
        var meta1 = Metadata.From(("key", "first"),  ("unique1", "a"));
        var meta2 = Metadata.From(("key", "second"), ("unique2", "b"));
        var meta3 = Metadata.From(("key", "third"),  ("unique3", "c"));

        var merged = Metadata.Merge(meta1, meta2, meta3);

        Assert.Equal(4,       merged.Count);
        Assert.Equal("third", merged["key"]);
        Assert.Equal("a",     merged["unique1"]);
        Assert.Equal("b",     merged["unique2"]);
        Assert.Equal("c",     merged["unique3"]);
    }

    #endregion

    #region MetadataBuilder Tests

    [Fact]
    public void MetadataBuilder_Empty()
    {
        var builder = new MetadataBuilder();
        var meta    = builder.Build();

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

        Assert.Equal(3,        meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42,       meta["key2"]);
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

        Assert.Equal(2,        meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42,       meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_AddRange_Metadata()
    {
        var existing = Metadata.From(("key1", "value1"), ("key2", 42));

        var meta = new MetadataBuilder()
                  .AddRange(existing)
                  .Build();

        Assert.Equal(2,        meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42,       meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_AddRange_Tuples()
    {
        var meta = new MetadataBuilder()
                  .AddRange(("key1", "value1"), ("key2", 42))
                  .Build();

        Assert.Equal(2,        meta.Count);
        Assert.Equal("value1", meta["key1"]);
        Assert.Equal(42,       meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_WithInitialMetadata()
    {
        var initial = Metadata.From(("existing", "value"));

        var meta = new MetadataBuilder(initial)
                  .Add("new", "data")
                  .Build();

        Assert.Equal(2,       meta.Count);
        Assert.Equal("value", meta["existing"]);
        Assert.Equal("data",  meta["new"]);
    }

    [Fact]
    public void MetadataBuilder_ImplicitConversion()
    {
        Metadata meta = new MetadataBuilder()
           .Add("key", "value");

        Assert.Single(meta);
        Assert.Equal("value", meta["key"]);
    }

    [Fact]
    public void MetadataBuilder_Remove_RemovesKey()
    {
        // Arrange (Given)
        var builder = new MetadataBuilder()
                     .Add("key1", "value1")
                     .Add("key2", "value2");

        // Act (When)
        var result = builder.Remove("key1");
        var meta   = result.Build();

        // Assert (Then)
        Assert.Same(builder, result);
        Assert.Single(meta);
        Assert.False(meta.ContainsKey("key1"));
        Assert.Equal("value2", meta["key2"]);
    }

    [Fact]
    public void MetadataBuilder_Remove_NonExistentKey_DoesNotThrow()
    {
        // Arrange (Given)
        var builder = new MetadataBuilder()
                     .Add("key1", "value1");

        // Act (When)
        var result = builder.Remove("nonexistent");
        var meta   = result.Build();

        // Assert (Then)
        Assert.Same(builder, result);
        Assert.Single(meta);
        Assert.Equal("value1", meta["key1"]);
    }

    [Fact]
    public void MetadataBuilder_Clear_RemovesAllKeys()
    {
        // Arrange (Given)
        var builder = new MetadataBuilder()
                     .Add("key1", "value1")
                     .Add("key2", "value2")
                     .Add("key3", "value3");

        // Act (When)
        var result = builder.Clear();
        var meta   = result.Build();

        // Assert (Then)
        Assert.Same(builder, result);
        Assert.Empty(meta);
    }

    [Fact]
    public void MetadataBuilder_Clear_OnEmptyBuilder_DoesNotThrow()
    {
        // Arrange (Given)
        var builder = new MetadataBuilder();

        // Act (When)
        var result = builder.Clear();
        var meta   = result.Build();

        // Assert (Then)
        Assert.Same(builder, result);
        Assert.Empty(meta);
    }

    #endregion

    #region Result WithMetadata Tests

    [Fact]
    public void Result_WithMetadata_Tuples()
    {
        var result = Result.Success()
                           .WithMetadata(("traceId", "abc-123"), ("userId", 42));

        Assert.Equal(2,         result.Metadata.Count);
        Assert.Equal("abc-123", result.Metadata["traceId"]);
        Assert.Equal(42,        result.Metadata["userId"]);
    }

    [Fact]
    public void Result_WithMetadata_Builder()
    {
        var result = Result.Success()
                           .WithMetadata(builder => builder
                                                   .Add("key1", "value1")
                                                   .AddIf(true,  "key2", "value2")
                                                   .AddIf(false, "key3", "value3"));

        Assert.Equal(2,        result.Metadata.Count);
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

        Assert.Equal(2,       result.Metadata.Count);
        Assert.Equal("value", result.Metadata["existing"]);
        Assert.Equal("data",  result.Metadata["new"]);
    }

    [Fact]
    public void ResultT_WithMetadata_Tuples()
    {
        var result = Result.Success(42)
                           .WithMetadata(("traceId", "abc-123"), ("userId", 99));

        Assert.True(result.TryGetValue(out var value));
        Assert.Equal(42,        value);
        Assert.Equal(2,         result.Metadata.Count);
        Assert.Equal("abc-123", result.Metadata["traceId"]);
        Assert.Equal(99,        result.Metadata["userId"]);
    }

    [Fact]
    public void ResultT_WithMetadata_Builder()
    {
        var result = Result.Success("test")
                           .WithMetadata(builder => builder
                                                   .Add("key1", "value1")
                                                   .AddRange(("key2", 42), ("key3", true)));

        Assert.True(result.TryGetValue(out var value));
        Assert.Equal("test",   value);
        Assert.Equal(3,        result.Metadata.Count);
        Assert.Equal("value1", result.Metadata["key1"]);
        Assert.Equal(42,       result.Metadata["key2"]);
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
            new KeyValuePair<string, object?>("userId",  42)
        ];

        var result = Result.Success()
                           .WithMetadata(meta);

        Assert.Equal(2,         result.Metadata.Count);
        Assert.Equal("abc-123", result.Metadata["traceId"]);
        Assert.Equal(42,        result.Metadata["userId"]);
    }

    [Fact]
    public void Integration_MergeAndBuilder()
    {
        var meta1  = Metadata.From(("key1", "value1"));
        var meta2  = Metadata.From(("key2", "value2"));
        var merged = Metadata.Merge(meta1, meta2);

        var result = Result.Success()
                           .WithMetadata(merged)
                           .WithMetadata(builder => builder
                                                   .Add("key3", "value3")
                                                   .AddIf(true, "key4", "value4"));

        Assert.Equal(4,        result.Metadata.Count);
        Assert.Equal("value1", result.Metadata["key1"]);
        Assert.Equal("value2", result.Metadata["key2"]);
        Assert.Equal("value3", result.Metadata["key3"]);
        Assert.Equal("value4", result.Metadata["key4"]);
    }

    [Fact]
    public void Integration_FluentChaining()
    {
        var result = Result.Success(42)
                           .WithMetadata("step1",                 "init")
                           .WithMetadata(("step2", "processing"), ("step3", "validation"))
                           .WithMetadata(builder => builder
                                                   .Add("step4", "completion")
                                                   .AddIf(true, "status", "success"));

        Assert.True(result.TryGetValue(out var value));
        Assert.Equal(42,           value);
        Assert.Equal(5,            result.Metadata.Count);
        Assert.Equal("init",       result.Metadata["step1"]);
        Assert.Equal("processing", result.Metadata["step2"]);
        Assert.Equal("validation", result.Metadata["step3"]);
        Assert.Equal("completion", result.Metadata["step4"]);
        Assert.Equal("success",    result.Metadata["status"]);
    }

    #endregion

    #region Metadata Indexer and ToString Tests

    [Fact]
    public void Metadata_IndexerSetter_UpdatesExistingValue()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key", "initial value"));

        // Act (When)
        meta["key"] = "updated value";

        // Assert (Then)
        Assert.Equal("updated value", meta["key"]);
    }

    [Fact]
    public void Metadata_IndexerSetter_AddsNewKey()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"));

        // Act (When)
        meta["key2"] = "value2";

        // Assert (Then)
        Assert.Equal(2, meta.Count);
        Assert.Equal("value2", meta["key2"]);
    }

    [Fact]
    public void Metadata_ToString_WithZeroTake_ReturnsEmpty()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key", "value"));

        // Act (When)
        var result = meta.ToString(0);

        // Assert (Then)
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Metadata_ToString_WithNegativeTake_ReturnsEmpty()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key", "value"));

        // Act (When)
        var result = meta.ToString(-5);

        // Assert (Then)
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Metadata_ToString_WithEmptyMetadata_ReturnsEmpty()
    {
        // Arrange (Given)
        var meta = Metadata.Empty;

        // Act (When)
        var result = meta.ToString(10);

        // Assert (Then)
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Metadata_ToString_WithTakeGreaterThanCount_ReturnsAll()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"), ("key2", "value2"));

        // Act (When)
        var result = meta.ToString(100);

        // Assert (Then)
        Assert.Contains("key1:value1", result);
        Assert.Contains("key2:value2", result);
    }

    #endregion

    #region Keys and Values Tests

    [Fact]
    public void Metadata_Keys_ReturnsAllKeys()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"), ("key2", 42), ("key3", true));

        // Act (When)
        var keys = meta.Keys.ToList();

        // Assert (Then)
        Assert.Equal(3, keys.Count);
        Assert.Contains("key1", keys);
        Assert.Contains("key2", keys);
        Assert.Contains("key3", keys);
    }

    [Fact]
    public void Metadata_Values_ReturnsAllValues()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"), ("key2", 42), ("key3", true));

        // Act (When)
        var values = meta.Values.ToList();

        // Assert (Then)
        Assert.Equal(3, values.Count);
        Assert.Contains("value1", values);
        Assert.Contains(42, values);
        Assert.Contains(true, values);
    }

    [Fact]
    public void Metadata_Keys_OnEmpty_ReturnsEmpty()
    {
        // Arrange (Given)
        var meta = Metadata.Empty;

        // Act (When)
        var keys = meta.Keys;

        // Assert (Then)
        Assert.Empty(keys);
    }

    [Fact]
    public void Metadata_Values_OnEmpty_ReturnsEmpty()
    {
        // Arrange (Given)
        var meta = Metadata.Empty;

        // Act (When)
        var values = meta.Values;

        // Assert (Then)
        Assert.Empty(values);
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void Metadata_TryGetValue_WithExistingKey_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key", "value"));

        // Act (When)
        var found = meta.TryGetValue("key", out var value);

        // Assert (Then)
        Assert.True(found);
        Assert.Equal("value", value);
    }

    [Fact]
    public void Metadata_TryGetValue_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"));

        // Act (When)
        var found = meta.TryGetValue("key2", out var value);

        // Assert (Then)
        Assert.False(found);
        Assert.Null(value);
    }

    [Fact]
    public void Metadata_TryGetValue_CaseInsensitive()
    {
        // Arrange (Given)
        var meta = Metadata.From(("Key", "value"));

        // Act (When)
        var found = meta.TryGetValue("key", out var value);

        // Assert (Then)
        Assert.True(found);
        Assert.Equal("value", value);
    }

    [Fact]
    public void Metadata_TryGetValue_WithNullValue_ReturnsTrueAndNull()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key", (object?)null));

        // Act (When)
        var found = meta.TryGetValue("key", out var value);

        // Assert (Then)
        Assert.True(found);
        Assert.Null(value);
    }

    #endregion

    #region IEnumerable Tests

    [Fact]
    public void Metadata_GetEnumerator_ReturnsAllKeyValuePairs()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"), ("key2", 42));

        // Act (When)
        var pairs = meta.ToList();

        // Assert (Then)
        Assert.Equal(2, pairs.Count);
        Assert.Contains(pairs, p => p.Key == "key1" && (string?)p.Value == "value1");
        Assert.Contains(pairs, p => p.Key == "key2" && (int?)p.Value == 42);
    }

    [Fact]
    public void Metadata_NonGenericGetEnumerator_ReturnsAllKeyValuePairs()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"), ("key2", 42));
        System.Collections.IEnumerable enumerable = meta;

        // Act (When)
        var enumerator = enumerable.GetEnumerator();
        var count = 0;
        while (enumerator.MoveNext())
        {
            count++;
        }

        // Assert (Then)
        Assert.Equal(2, count);
    }

    [Fact]
    public void Metadata_ContainsKey_WithExistingKey_ReturnsTrue()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"));

        // Act (When)
        var contains = meta.ContainsKey("key1");

        // Assert (Then)
        Assert.True(contains);
    }

    [Fact]
    public void Metadata_ContainsKey_WithNonExistentKey_ReturnsFalse()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key1", "value1"));

        // Act (When)
        var contains = meta.ContainsKey("key2");

        // Assert (Then)
        Assert.False(contains);
    }

    [Fact]
    public void Metadata_ContainsKey_CaseInsensitive()
    {
        // Arrange (Given)
        var meta = Metadata.From(("Key", "value"));

        // Act (When)
        var contains = meta.ContainsKey("key");

        // Assert (Then)
        Assert.True(contains);
    }

    [Fact]
    public void Metadata_ToString_WithEmptyMetadata_ReturnsEmptyString()
    {
        // Arrange (Given)
        var meta = Metadata.Empty;

        // Act (When)
        var result = meta.ToString();

        // Assert (Then)
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Metadata_ToString_WithNullValue_ShowsNull()
    {
        // Arrange (Given)
        var meta = Metadata.From(("key", (object?)null));

        // Act (When)
        var result = meta.ToString();

        // Assert (Then)
        Assert.Contains("key:null", result);
    }

    #endregion
}
