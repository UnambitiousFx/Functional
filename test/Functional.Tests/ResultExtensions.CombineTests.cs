using UnambitiousFx.Functional.Errors;
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
        Assert.False(combined.TryGet(out var error));
        Assert.IsType<AggregateError>(error);
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
}
