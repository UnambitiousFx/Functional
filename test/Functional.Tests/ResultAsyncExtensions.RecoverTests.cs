using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Recover Tests

    [Fact]
    public async Task Recover_ValueTask_WithSyncRecover_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Recover(e => 99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Recover_ValueTask_WithSyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => 99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_ValueTask_WithAsyncRecover_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Recover(e => ValueTask.FromResult(99));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Recover_ValueTask_WithAsyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => ValueTask.FromResult(99));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_Task_WithSyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => 99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_Task_WithAsyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => ValueTask.FromResult(99));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    #endregion

    #region Recover With Fallback Value Tests

    [Fact]
    public async Task Recover_WithFallbackValue_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Recover(99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Recover_WithFallbackValue_AndFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    #endregion
}
