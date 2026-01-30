using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Recover_WithResultTaskSuccess_DoesNotRecover()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var recovered = await awaitableResult.Recover(async error =>
        {
            await ValueTask.CompletedTask;
            return 99;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Recover_WithResultTaskFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var recovered = await awaitableResult.Recover(async error =>
        {
            await ValueTask.CompletedTask;
            return 99;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_WithResultTaskFailure_CanAccessErrorInRecovery()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Specific error").AsAsync();

        // Act (When)
        var recovered = await awaitableResult.Recover(async error =>
        {
            await ValueTask.CompletedTask;
            return error.Message == "Specific error" ? 100 : 0;
        });

        // Assert (Then)
        recovered.ShouldBe().Success().And(v => Assert.Equal(100, v));
    }
}