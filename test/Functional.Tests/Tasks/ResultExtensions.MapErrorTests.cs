using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task MapErrorAsync_WithSyncResult_Failure_MapsError()
    {
        // Arrange (Given)
        var result = Result.Failure("Original");

        // Act (When)
        var mapped = await result.MapErrorAsync(async error =>
        {
            await Task.CompletedTask;
            return error with { Message = "Mapped: " + error.Message };
        });

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Mapped: Original");
    }

    [Fact]
    public async Task MapErrorAsync_WithSyncResult_Success_DoesNotMap()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var mapped = await result.MapErrorAsync(async error =>
        {
            await Task.CompletedTask;
            return error with { Message = "Should not appear" };
        });

        // Assert (Then)
        mapped.ShouldBe().Success();
    }

    [Fact]
    public async Task MapErrorAsync_WithAwaitableResult_Failure_MapsError()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure("Original"));

        // Act (When)
        var mapped = await awaitableResult.MapErrorAsync(async error =>
        {
            await Task.CompletedTask;
            return error with { Message = "Mapped: " + error.Message };
        });

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Mapped: Original");
    }

    [Fact]
    public async Task MapErrorAsync_Generic_WithSyncResult_Failure_MapsError()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original");

        // Act (When)
        var mapped = await result.MapErrorAsync(async error =>
        {
            await Task.CompletedTask;
            return error with { Message = "Mapped: " + error.Message };
        });

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Mapped: Original");
    }

    [Fact]
    public async Task MapErrorAsync_Generic_WithAwaitableResult_Success_DoesNotMap()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        var mapped = await awaitableResult.MapErrorAsync(async error =>
        {
            await Task.CompletedTask;
            return error with { Message = "Should not appear" };
        });

        // Assert (Then)
        mapped.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }
}
