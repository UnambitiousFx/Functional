using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for async Combine extension methods on Result using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task CombineAsync_WithAllTasksSucceeding_ReturnsSuccess()
    {
        // Arrange (Given)
        var tasks = new[]
        {
            Task.FromResult(Result.Success()), Task.FromResult(Result.Success()),
            Task.FromResult(Result.Success())
        };

        // Act (When)
        var combined = await tasks.CombineAsync();

        // Assert (Then)
        combined.ShouldBe().Success();
    }

    [Fact]
    public async Task CombineAsync_WithOneTaskFailing_ReturnsFailure()
    {
        // Arrange (Given)
        var tasks = new[]
        {
            Task.FromResult(Result.Success()), Task.FromResult(Result.Failure("Error 1")),
            Task.FromResult(Result.Success())
        };

        // Act (When)
        var combined = await tasks.CombineAsync();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public async Task CombineAsync_WithMultipleTasksFailing_AggregatesErrors()
    {
        // Arrange (Given)
        var tasks = new[]
        {
            Task.FromResult(Result.Failure("Error 1")), Task.FromResult(Result.Success()),
            Task.FromResult(Result.Failure("Error 2"))
        };

        // Act (When)
        var combined = await tasks.CombineAsync();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public async Task CombineAsync_WithAwaitableEnumerable_CombinesResults()
    {
        // Arrange (Given)
        var awaitableResults = Task.FromResult<IEnumerable<Result>>(new[]
        {
            Result.Success(), Result.Success(), Result.Success()
        });

        // Act (When)
        var combined = await awaitableResults.CombineAsync();

        // Assert (Then)
        combined.ShouldBe().Success();
    }

    [Fact]
    public async Task CombineAsync_WithAwaitableEnumerableContainingFailures_ReturnsFailure()
    {
        // Arrange (Given)
        var awaitableResults = Task.FromResult<IEnumerable<Result>>(new[]
        {
            Result.Success(), Result.Failure("Error"), Result.Success()
        });

        // Act (When)
        var combined = await awaitableResults.CombineAsync();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public async Task CombineAsync_WithEmptyCollection_ReturnsSuccess()
    {
        // Arrange (Given)
        var tasks = Array.Empty<Task<Result>>();

        // Act (When)
        var combined = await tasks.CombineAsync();

        // Assert (Then)
        combined.ShouldBe().Success();
    }
}
