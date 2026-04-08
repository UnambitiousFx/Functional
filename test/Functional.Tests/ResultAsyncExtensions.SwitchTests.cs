using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    [Fact]
    public async Task TaskResult_Switch_UsesDirectAsyncExtensions()
    {
        // Arrange (Given)
        var start  = ValueTask.FromResult(Result.Ok(3));
        var output = "";

        // Act (When)
        await start.Switch(
            v => output = $"ok:{v}",
            _ => output = "fail");

        // Assert (Then)
        Assert.Equal("ok:3", output);
    }

    #region Switch Tests

    [Fact]
    public async Task Switch_ValueTask_WithSyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v => successValue = v,
            _ => failureCalled = true);

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_ValueTask_WithSyncActions_AndFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var successCalled = false;
        Failure? capturedError = null;

        // Act (When)
        await resultTask.Switch(
            _ => successCalled = true,
            e => capturedError = e);

        // Assert (Then)
        Assert.False(successCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task Switch_ValueTask_WithAsyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v =>
            {
                successValue = v;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                failureCalled = true;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_ValueTask_WithAsyncActions_AndFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var successCalled = false;
        Failure? capturedError = null;

        // Act (When)
        await resultTask.Switch(
            _ =>
            {
                successCalled = true;
                return ValueTask.CompletedTask;
            },
            e =>
            {
                capturedError = e;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task Switch_Task_WithSyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v => successValue = v,
            _ => failureCalled = true);

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_Task_WithAsyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v =>
            {
                successValue = v;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                failureCalled = true;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_Task_WithSyncActions_AndFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var successCalled = false;
        Failure? capturedError = null;

        // Act (When)
        await resultTask.Switch(
            _ => successCalled = true,
            e => capturedError = e);

        // Assert (Then)
        Assert.False(successCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task Switch_Task_WithAsyncActions_AndFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var successCalled = false;
        Failure? capturedError = null;

        // Act (When)
        await resultTask.Switch(
            _ =>
            {
                successCalled = true;
                return ValueTask.CompletedTask;
            },
            e =>
            {
                capturedError = e;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    #endregion
}
