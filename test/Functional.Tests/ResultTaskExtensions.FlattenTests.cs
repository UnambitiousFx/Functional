using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for async Flatten extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Flatten_WithNestedSuccess_ReturnsInnerValue()
    {
        // Arrange (Given)
        var nested = Result.Success(Result.Success(42)).AsAsync();

        // Act (When)
        var flattened = await nested.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task Flatten_WithOuterFailure_ReturnsOuterFailure()
    {
        // Arrange (Given)
        var nested = Result.Failure<Result<int>>("Outer error").AsAsync();

        // Act (When)
        var flattened = await nested.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Failure().AndMessage("Outer error");
    }

    [Fact]
    public async Task Flatten_WithInnerFailure_ReturnsInnerFailure()
    {
        // Arrange (Given)
        var nested = Result.Success(Result.Failure<int>("Inner error")).AsAsync();

        // Act (When)
        var flattened = await nested.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Failure().AndMessage("Inner error");
    }
}