using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for async Flatten extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task FlattenAsync_WithNestedSuccess_ReturnsInnerValue()
    {
        // Arrange (Given)
        var nested = ValueTask.FromResult(Result.Success(Result.Success(42)));

        // Act (When)
        var flattened = await nested.FlattenAsync();

        // Assert (Then)
        flattened.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task FlattenAsync_WithOuterFailure_ReturnsOuterFailure()
    {
        // Arrange (Given)
        var nested = ValueTask.FromResult(Result.Failure<Result<int>>("Outer error"));

        // Act (When)
        var flattened = await nested.FlattenAsync();

        // Assert (Then)
        flattened.ShouldBe().Failure().AndMessage("Outer error");
    }

    [Fact]
    public async Task FlattenAsync_WithInnerFailure_ReturnsInnerFailure()
    {
        // Arrange (Given)
        var nested = ValueTask.FromResult(Result.Success(Result.Failure<int>("Inner error")));

        // Act (When)
        var flattened = await nested.FlattenAsync();

        // Assert (Then)
        flattened.ShouldBe().Failure().AndMessage("Inner error");
    }
}
