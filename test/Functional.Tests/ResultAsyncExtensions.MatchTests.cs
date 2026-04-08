using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Match Tests

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

    #endregion
}
