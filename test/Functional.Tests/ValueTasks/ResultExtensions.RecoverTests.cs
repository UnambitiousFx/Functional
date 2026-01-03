using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task RecoverAsync_WithAwaitableSuccess_DoesNotRecover()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var recovered = await awaitableResult.RecoverAsync(async error =>
        {
            await ValueTask.CompletedTask;
            return 99;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task RecoverAsync_WithAwaitableFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var recovered = await awaitableResult.RecoverAsync(async error =>
        {
            await ValueTask.CompletedTask;
            return 99;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task RecoverAsync_WithAwaitableFailure_CanAccessErrorInRecovery()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Specific error"));

        // Act (When)
        var recovered = await awaitableResult.RecoverAsync(async error =>
        {
            await ValueTask.CompletedTask;
            return error.Message == "Specific error" ? 100 : 0;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(v => Assert.Equal(100, v));
    }
}
