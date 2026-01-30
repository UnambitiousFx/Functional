using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for ResultTask TapError extension methods.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task TapError_WithSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success().AsAsync();
        var executed = false;

        // Act (When)
        var tapped = await result.TapError(_ => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.False(executed);
    }

    [Fact]
    public async Task TapError_WithFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Failure("Error occurred").AsAsync();
        Failure? capturedError = null;

        // Act (When)
        var tapped = await result.TapError(error => capturedError = error);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.NotNull(capturedError);
        Assert.Equal("Error occurred", capturedError.Message);
    }

    [Fact]
    public async Task TapError_WithAsyncSideEffect_ExecutesOnFailure()
    {
        // Arrange (Given)
        var result = Result.Failure("Error occurred").AsAsync();
        var executed = false;

        // Act (When)
        var tapped = await result.TapError(async _ =>
        {
            await ValueTask.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.True(executed);
    }
}