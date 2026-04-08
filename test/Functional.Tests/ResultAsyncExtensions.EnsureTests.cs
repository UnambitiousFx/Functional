using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Ensure Tests

    [Fact]
    public async Task Ensure_ValueTask_WithSyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => x > 0,
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Ensure_ValueTask_WithSyncPredicate_AndPredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => x < 0,
            x => new Failure("Value must be negative"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value must be negative");
    }

    [Fact]
    public async Task Ensure_ValueTask_WithAsyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => ValueTask.FromResult(x > 0),
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Ensure_ValueTask_WithAsyncPredicate_AndPredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => ValueTask.FromResult(x < 0),
            x => new Failure("Value must be negative"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value must be negative");
    }

    [Fact]
    public async Task Ensure_ValueTask_WithFailure_DoesNotEvaluatePredicate()
    {
        // Arrange (Given)
        var error      = new Failure("Original error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var predicateCalled = false;

        // Act (When)
        var result = await resultTask.Ensure(
            x =>
            {
                predicateCalled = true;
                return ValueTask.FromResult(true);
            },
            x => new Failure("Should not be called"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.False(predicateCalled);
    }

    [Fact]
    public async Task Ensure_Task_WithSyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => x > 0,
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Ensure_Task_WithAsyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => ValueTask.FromResult(x > 0),
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    #endregion
}
