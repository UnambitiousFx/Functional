using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region TapFailure Tests

    [Fact]
    public async Task TapFailure_ValueTask_WithSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_ValueTask_WithFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;
        Failure? capturedError = null;

        // Act (When)
        var result = await resultTask.TapFailure(e =>
        {
            actionCalled   = true;
            capturedError = e;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task TapFailure_ValueTask_WithAsyncAction_AndSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_ValueTask_WithAsyncAction_AndFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithAsyncAction_AndSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithAsyncAction_AndFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
    }

    #endregion

    #region Tap Tests

    [Fact]
    public async Task Tap_ValueTask_WithSyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x => tappedValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    [Fact]
    public async Task Tap_ValueTask_WithSyncAction_AndFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Tap(x => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task Tap_ValueTask_WithAsyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x =>
        {
            tappedValue = x;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    [Fact]
    public async Task Tap_ValueTask_WithAsyncAction_AndFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Tap(x =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task Tap_Task_WithSyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x => tappedValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    [Fact]
    public async Task Tap_Task_WithAsyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x =>
        {
            tappedValue = x;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    #endregion
}
