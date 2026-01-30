using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for ResultTask Map extension methods.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Map_WithResultTaskAndSyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = Result.Success(10).AsAsync();

        // Act (When)
        var mapped = await result.Map(x => x + 5);

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(15, value));
    }

    [Fact]
    public async Task Map_WithResultTaskAndAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = Result.Success(10).AsAsync();

        // Act (When)
        var mapped = await result.Map(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(20, value));
    }

    [Fact]
    public async Task Map_WithFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var mapped = await result.Map(x => x + 5);

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task Map_WithNonGenericResult_TransformsToValue()
    {
        // Arrange (Given)
        var result = Result.Success().AsAsync();

        // Act (When)
        var mapped = await result.Map(() => 42);

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task Map_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(5)
            .WithMetadata("key", "value")
            .AsAsync();

        // Act (When)
        var mapped = await result.Map(x => x + 1);

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(6, value));
        Assert.Equal("value", mapped.Metadata["key"]);
    }
}