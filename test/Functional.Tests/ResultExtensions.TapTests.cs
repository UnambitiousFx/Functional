using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Tap, TapFailure, and TapBoth extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Tap - Success Cases

    [Fact]
    public void Tap_WithSuccess_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result        = Result.Success(5);
        var capturedValue = 0;

        // Act (When)
        var tapped = result.Tap(x => capturedValue = x);

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
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
        tapped.ShouldBe()
              .Success()
              .And(value => Assert.Equal(42, value));
    }

    [Fact]
    public void Tap_NonGeneric_WithSuccess_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result   = Result.Success();
        var executed = false;

        // Act (When)
        var tapped = result.Tap(() => executed = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.True(executed);
    }

    #endregion

    #region Tap - Failure Cases

    [Fact]
    public void Tap_WithFailure_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var error    = new Failure("Test error");
        var result   = Result.Failure<int>(error);
        var executed = false;

        // Act (When)
        var tapped = result.Tap(x => executed = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Failure();
        Assert.False(executed);
    }

    [Fact]
    public void Tap_WithFailure_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error  = new Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var tapped = result.Tap(x => { });

        // Assert (Then)
        tapped.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    #endregion

    #region Tap - Chaining

    [Fact]
    public void Tap_CanChainMultipleTaps()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var log    = new List<int>();

        // Act (When)
        var tapped = result
                    .Tap(x => log.Add(x))
                    .Tap(x => log.Add(x * 2))
                    .Tap(x => log.Add(x * 3));

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.Equal(new[] { 5, 10, 15 }, log);
    }

    [Fact]
    public void Tap_CanCombineWithOtherOperations()
    {
        // Arrange (Given)
        var result        = Result.Success(5);
        var capturedValue = 0;

        // Act (When)
        var final = result
                   .Tap(x => capturedValue = x)
                   .Map(x => x * 2)
                   .Tap(x => capturedValue = x);

        // Assert (Then)
        final.ShouldBe()
             .Success()
             .And(value => Assert.Equal(10, value));
        Assert.Equal(10, capturedValue);
    }

    #endregion

    #region TapIf Tests

    [Fact]
    public void TapIf_NonGeneric_WithSuccess_AndPredicateTrue_ExecutesAction()
    {
        // Arrange (Given)
        var result      = Result.Success();
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(() => true, () => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.True(tapExecuted);
    }

    [Fact]
    public void TapIf_NonGeneric_WithSuccess_AndPredicateFalse_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result      = Result.Success();
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(() => false, () => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.False(tapExecuted);
    }

    [Fact]
    public void TapIf_NonGeneric_WithFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error       = new Failure("Test error");
        var result      = Result.Failure(error);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(() => true, () => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(tapExecuted);
    }

    [Fact]
    public void TapIf_Generic_WithSuccess_AndPredicateTrue_ExecutesValueAction()
    {
        // Arrange (Given)
        var result      = Result.Success(42);
        var capturedVal = 0;

        // Act (When)
        var tapped = result.TapIf(v => v > 0, v => capturedVal = v);

        // Assert (Then)
        tapped.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, capturedVal);
    }

    [Fact]
    public void TapIf_Generic_WithSuccess_AndPredicateFalse_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result      = Result.Success(42);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(v => v < 0, _ => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(tapExecuted);
    }

    [Fact]
    public void TapIf_Generic_WithFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error       = new Failure("Test error");
        var result      = Result.Failure<int>(error);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(v => v > 0, _ => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(tapExecuted);
    }

    [Fact]
    public void TapIf_Generic_WithNoArgAction_AndPredicateTrue_ExecutesAction()
    {
        // Arrange (Given)
        var result      = Result.Success(42);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(v => v > 0, () => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.True(tapExecuted);
    }

    [Fact]
    public void TapIf_Generic_WithNoArgAction_AndPredicateFalse_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var result      = Result.Success(42);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.TapIf(v => v < 0, () => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(tapExecuted);
    }

    [Fact]
    public void Tap_Generic_WithNoArgAction_OnSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var result      = Result.Success(42);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.Tap(() => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.True(tapExecuted);
    }

    [Fact]
    public void Tap_Generic_WithNoArgAction_OnFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error       = new Failure("Test error");
        var result      = Result.Failure<int>(error);
        var tapExecuted = false;

        // Act (When)
        var tapped = result.Tap(() => tapExecuted = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(tapExecuted);
    }

    #endregion
}
