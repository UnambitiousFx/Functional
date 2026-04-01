using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;
using AggregateFailure = UnambitiousFx.Functional.Failures.AggregateFailure;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Compensate Tests

    [Fact]
    public async Task Compensate_ValueTask_WithSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_ValueTask_WithFailure_AndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_ValueTask_WithFailure_AndFailedRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var rollbackError = new Failure("Rollback error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));

        // Act (When)
        var result = await resultTask.Compensate(_ => Result.Failure(rollbackError));

        // Assert (Then)
        result.ShouldBe()
              .Failure();
        Assert.True(result.TryGetFailure(out var error));
        Assert.IsType<AggregateFailure>(error);
    }

    [Fact]
    public async Task Compensate_ValueTask_WithAsyncRollback_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Compensate(_ => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Compensate_ValueTask_WithAsyncRollback_AndFailure_ExecutesRollback()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_Task_WithSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_Task_WithFailure_AndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_Task_WithAsyncRollback_AndFailure_ExecutesRollback()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    #endregion
}
