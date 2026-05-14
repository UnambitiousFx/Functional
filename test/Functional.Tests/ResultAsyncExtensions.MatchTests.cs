using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    [Fact]
    public async Task Match_ValueTask_WithSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
                         v => $"success:{v}",
                         _ => "failure");

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_ValueTask_WithFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
                         v => $"success:{v}",
                         e => $"failure:{e.Message}");

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_ValueTask_WithAsyncFunctions_AndSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
                         v => ValueTask.FromResult($"success:{v}"),
                         _ => ValueTask.FromResult("failure"));

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_ValueTask_WithAsyncFunctions_AndFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
                         v => ValueTask.FromResult($"success:{v}"),
                         e => ValueTask.FromResult($"failure:{e.Message}"));

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_Task_WithSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
                         v => $"success:{v}",
                         _ => "failure");

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_Task_WithFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
                         v => $"success:{v}",
                         e => $"failure:{e.Message}");

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_Task_WithAsyncFunctions_AndSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
                         v => ValueTask.FromResult($"success:{v}"),
                         _ => ValueTask.FromResult("failure"));

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_Task_WithAsyncFunctions_AndFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
                         v => ValueTask.FromResult($"success:{v}"),
                         e => ValueTask.FromResult($"failure:{e.Message}"));

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_NonGenericFunc_WithSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var output = await resultTask.Match(
                         () => "success",
                         _ => "failure");

        // Assert (Then)
        Assert.Equal("success", output);
    }

    [Fact]
    public async Task Match_NonGenericFunc_WithFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var output = await resultTask.Match(
                         () => "success",
                         e => $"failure:{e.Message}");

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_NonGenericAction_WithSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask    = ValueTask.FromResult(Result.Success());
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        await resultTask.Match(
            () => { successCalled = true; },
            _ => { failureCalled  = true; });

        // Assert (Then)
        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Match_NonGenericAction_WithFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error         = new Failure("Test error");
        var resultTask    = ValueTask.FromResult(Result.Failure(error));
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        await resultTask.Match(
            () => { successCalled = true; },
            _ => { failureCalled  = true; });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.True(failureCalled);
    }

    [Fact]
    public async Task Match_NonGenericAsyncFunc_WithSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var output = await resultTask.Match(
                         () => ValueTask.FromResult("success"),
                         _ => ValueTask.FromResult("failure"));

        // Assert (Then)
        Assert.Equal("success", output);
    }

    [Fact]
    public async Task Match_NonGenericAsyncFunc_WithFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var output = await resultTask.Match(
                         () => ValueTask.FromResult("success"),
                         e => ValueTask.FromResult($"failure:{e.Message}"));

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_NonGenericAsyncAction_WithSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask    = ValueTask.FromResult(Result.Success());
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        await resultTask.Match(
            () =>
            {
                successCalled = true;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                failureCalled = true;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Match_NonGenericAsyncAction_WithFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error         = new Failure("Test error");
        var resultTask    = ValueTask.FromResult(Result.Failure(error));
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        await resultTask.Match(
            () =>
            {
                successCalled = true;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                failureCalled = true;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.True(failureCalled);
    }
}
