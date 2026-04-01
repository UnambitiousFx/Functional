using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region ValueOr Tests

    [Fact]
    public async Task ValueOr_ValueTask_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithFactory_AndSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithFactory_AndFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithAsyncFactory_AndSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(() => ValueTask.FromResult(99));

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithAsyncFactory_AndFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(() => ValueTask.FromResult(99));

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithFactory_AndSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithAsyncFactory_AndFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(() => ValueTask.FromResult(99));

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOrDefault_ValueTask_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrDefault_ValueTask_WithFailure_ReturnsDefault()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(default, value);
    }

    [Fact]
    public async Task ValueOrDefault_Task_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrDefault_Task_WithFailure_ReturnsDefault()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(default, value);
    }

    #endregion
}
