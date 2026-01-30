using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Tap, TapError, and TapBoth extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Tap - Success Cases

    [Fact]
    public void Tap_WithSuccess_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var capturedValue = 0;

        // Act (When)
        var tapped = result.Tap(x => capturedValue = x);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.Equal(5, capturedValue);
    }

    [Fact]
    public void Tap_WithSuccess_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var tapped = result.Tap(x => { });

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public void Tap_NonGeneric_WithSuccess_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success();
        var executed = false;

        // Act (When)
        var tapped = result.Tap(() => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.True(executed);
    }

    #endregion

    #region Tap - Failure Cases

    [Fact]
    public void Tap_WithFailure_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<int>(error);
        var executed = false;

        // Act (When)
        var tapped = result.Tap(x => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(executed);
    }

    [Fact]
    public void Tap_WithFailure_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error = new Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var tapped = result.Tap(x => { });

        // Assert (Then)
        tapped.ShouldBe().Failure().AndMessage("Test error");
    }

    #endregion

    #region Tap - Chaining

    [Fact]
    public void Tap_CanChainMultipleTaps()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var log = new List<int>();

        // Act (When)
        var tapped = result
            .Tap(x => log.Add(x))
            .Tap(x => log.Add(x * 2))
            .Tap(x => log.Add(x * 3));

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.Equal(new[] { 5, 10, 15 }, log);
    }

    [Fact]
    public void Tap_CanCombineWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var capturedValue = 0;

        // Act (When)
        var final = result
            .Tap(x => capturedValue = x)
            .Map(x => x * 2)
            .Tap(x => capturedValue = x);

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(10, value));
        Assert.Equal(10, capturedValue);
    }

    #endregion
}
