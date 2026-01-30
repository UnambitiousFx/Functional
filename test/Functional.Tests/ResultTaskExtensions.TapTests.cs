using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for ResultTask Tap extension methods.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Tap_WithSuccess_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success(10).AsAsync();
        var executed = false;

        // Act (When)
        var tapped = await result.Tap(value =>
        {
            executed = true;
            Assert.Equal(10, value);
        });

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(10, value));
        Assert.True(executed);
    }

    [Fact]
    public async Task Tap_WithFailure_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error").AsAsync();
        var executed = false;

        // Act (When)
        var tapped = await result.Tap(_ => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Failure().AndMessage("Error");
        Assert.False(executed);
    }

    [Fact]
    public async Task Tap_WithAsyncSideEffect_Executes()
    {
        // Arrange (Given)
        var result = Result.Success().AsAsync();
        var executed = false;

        // Act (When)
        var tapped = await result.Tap(async () =>
        {
            await ValueTask.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.True(executed);
    }
}