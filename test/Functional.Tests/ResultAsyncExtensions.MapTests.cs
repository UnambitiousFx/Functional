using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Map Tests

    [Fact]
    public async Task Map_ValueTask_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_ValueTask_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_ValueTask_WithAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_ValueTask_WithAsyncMapper_AndFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Map(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_Task_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_Task_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_Task_WithAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    #endregion

    #region Map on Non-Generic Result Tests

    [Fact]
    public async Task Map_NonGenericResult_WithSuccess_ProducesValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Map(() => 42);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Map_NonGenericResult_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var result = await resultTask.Map(() => 42);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_NonGenericResultAsync_WithSuccess_ProducesValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Map(() => ValueTask.FromResult(42));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Map_NonGenericResultAsync_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var result = await resultTask.Map(() => ValueTask.FromResult(42));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    #endregion
}
