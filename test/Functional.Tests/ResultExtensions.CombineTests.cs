using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void Combine_WithAllSuccess_ReturnsSuccess()
    {
        // Arrange (Given)
        var results = new[] { Result.Success(), Result.Success(), Result.Success() };

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe().Success();
    }

    [Fact]
    public void Combine_WithSomeFailures_ReturnsAggregateFailure()
    {
        // Arrange (Given)
        var results = new[]
        {
            Result.Success(), Result.Failure("Error 1"), Result.Success(), Result.Failure("Error 2")
        };

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe().Failure();
        Assert.False(combined.TryGetError(out var error));
        Assert.IsType<AggregateFailure>(error);
    }

    [Fact]
    public void Combine_WithAllFailures_ReturnsAggregateFailure()
    {
        // Arrange (Given)
        var results = new[] { Result.Failure("Error 1"), Result.Failure("Error 2"), Result.Failure("Error 3") };

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public void Combine_WithEmptyCollection_ReturnsSuccess()
    {
        // Arrange (Given)
        var results = Array.Empty<Result>();

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe().Success();
    }

    [Fact]
    public void CombineGeneric_WithAllSuccess_ReturnsSuccessWithValues()
    {
        // Arrange (Given)
        var results = new[] { Result.Success(1), Result.Success(2), Result.Success(3) };

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe()
            .Success()
            .And(values => Assert.Equal([1, 2, 3], values));
    }

    [Fact]
    public void CombineGeneric_WithSomeFailures_ReturnsAggregateFailure()
    {
        // Arrange (Given)
        var results = new[]
        {
            Result.Success(1), Result.Failure<int>("Error 1"), Result.Success(3), Result.Failure<int>("Error 2")
        };

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe().Failure();
        Assert.False(combined.TryGet(out _, out var error));
        Assert.IsType<AggregateFailure>(error);
    }

    [Fact]
    public void CombineGeneric_WithAllFailures_ReturnsAggregateFailure()
    {
        // Arrange (Given)
        var results = new[]
        {
            Result.Failure<int>("Error 1"), Result.Failure<int>("Error 2"), Result.Failure<int>("Error 3")
        };

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public void CombineGeneric_WithEmptyCollection_ReturnsSuccessWithEmptyValues()
    {
        // Arrange (Given)
        var results = Array.Empty<Result<int>>();

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe()
            .Success()
            .And(values => Assert.Empty(values));
    }

    [Theory]
    [InlineData("test1", "test2", "test3")]
    [InlineData("a")]
    public void CombineGeneric_WithDifferentValueTypes_WorksCorrectly(params string[] inputValues)
    {
        // Arrange (Given)
        var results = inputValues.Select(Result.Success);

        // Act (When)
        var combined = results.Combine();

        // Assert (Then)
        combined.ShouldBe()
            .Success()
            .And(values => Assert.Equal(inputValues, values));
    }
}
