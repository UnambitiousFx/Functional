using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultTaskTests
{
    [Fact]
    public async Task ImplicitConversion_FromResult_CreatesResultTask()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        ResultTask resultTask = result;
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Success();
    }

    [Fact]
    public async Task ImplicitConversion_FromFailure_CreatesFailedResultTask()
    {
        // Arrange (Given)
        var failure = new Failure("Test error");

        // Act (When)
        ResultTask resultTask = failure;
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Failure().AndMessage("Test error");
    }

    [Fact]
    public async Task ImplicitConversion_ToValueTask_ReturnsUnderlyingTask()
    {
        // Arrange (Given)
        var result = Result.Success();
        var resultTask = new ResultTask(ValueTask.FromResult(result));

        // Act (When)
        ValueTask<Result> valueTask = resultTask;
        var awaited = await valueTask;

        // Assert (Then)
        awaited.ShouldBe().Success();
    }

    [Fact]
    public async Task AsValueTask_ReturnsUnderlyingTask()
    {
        // Arrange (Given)
        var result = Result.Success();
        var resultTask = new ResultTask(ValueTask.FromResult(result));

        // Act (When)
        var valueTask = resultTask.AsValueTask();
        var awaited = await valueTask;

        // Assert (Then)
        awaited.ShouldBe().Success();
    }

    [Fact]
    public async Task GetAwaiter_CanBeAwaited()
    {
        // Arrange (Given)
        var result = Result.Success();
        var resultTask = new ResultTask(ValueTask.FromResult(result));

        // Act (When)
        var awaiter = resultTask.GetAwaiter();
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Success();
    }

    [Fact]
    public async Task Await_WithSuccess_ReturnsSuccessResult()
    {
        // Arrange (Given)
        var resultTask = new ResultTask(ValueTask.FromResult(Result.Success()));

        // Act (When)
        var result = await resultTask;

        // Assert (Then)
        result.ShouldBe().Success();
    }

    [Fact]
    public async Task Await_WithFailure_ReturnsFailureResult()
    {
        // Arrange (Given)
        var resultTask = new ResultTask(ValueTask.FromResult(Result.Failure("Error")));

        // Act (When)
        var result = await resultTask;

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage("Error");
    }
}

public sealed class ResultTaskGenericTests
{
    [Fact]
    public async Task ImplicitConversion_FromResult_CreatesResultTask()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        ResultTask<int> resultTask = result;
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task ImplicitConversion_FromValue_CreatesSuccessResultTask()
    {
        // Arrange (Given)
        int value = 42;

        // Act (When)
        ResultTask<int> resultTask = value;
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ImplicitConversion_FromFailure_CreatesFailedResultTask()
    {
        // Arrange (Given)
        var failure = new Failure("Test error");

        // Act (When)
        ResultTask<int> resultTask = failure;
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Failure().AndMessage("Test error");
    }

    [Fact]
    public async Task ImplicitConversion_ToValueTask_ReturnsUnderlyingTask()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var resultTask = new ResultTask<int>(ValueTask.FromResult(result));

        // Act (When)
        ValueTask<Result<int>> valueTask = resultTask;
        var awaited = await valueTask;

        // Assert (Then)
        awaited.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task AsValueTask_ReturnsUnderlyingTask()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var resultTask = new ResultTask<int>(ValueTask.FromResult(result));

        // Act (When)
        var valueTask = resultTask.AsValueTask();
        var awaited = await valueTask;

        // Assert (Then)
        awaited.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task GetAwaiter_CanBeAwaited()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var resultTask = new ResultTask<int>(ValueTask.FromResult(result));

        // Act (When)
        var awaiter = resultTask.GetAwaiter();
        var awaited = await resultTask;

        // Assert (Then)
        awaited.ShouldBe().Success();
    }

    [Fact]
    public async Task Await_WithSuccess_ReturnsSuccessResult()
    {
        // Arrange (Given)
        var resultTask = new ResultTask<int>(ValueTask.FromResult(Result.Success(42)));

        // Act (When)
        var result = await resultTask;

        // Assert (Then)
        result.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task Await_WithFailure_ReturnsFailureResult()
    {
        // Arrange (Given)
        var resultTask = new ResultTask<int>(ValueTask.FromResult(Result.Failure<int>("Error")));

        // Act (When)
        var result = await resultTask;

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task Constructor_WithValueTask_InitializesCorrectly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var resultTask = new ResultTask<int>(valueTask);
        var result = await resultTask;

        // Assert (Then)
        result.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }
}
