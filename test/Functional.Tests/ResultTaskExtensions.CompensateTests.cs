using UnambitiousFx.Functional.xunit;
using AggregateFailure = UnambitiousFx.Functional.Failures.AggregateFailure;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Compensate_WithSuccessResult_SyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = Result.Success().AsAsync();
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe().Success();
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task Compensate_WithFailureAndSuccessfulRollback_SyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var result = Result.Failure(originalError).AsAsync();
        Functional.Failures.Failure? receivedError = null;

        // Act (When)
        var compensated = await result.Compensate(error =>
        {
            receivedError = error;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error => Assert.Equal(originalError, error));
        Assert.Equal(originalError, receivedError);
    }

    [Fact]
    public async Task Compensate_WithFailureAndFailedRollback_SyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var rollbackError = new Functional.Failures.Failure("Rollback error");
        var result = Result.Failure(originalError).AsAsync();

        // Act (When)
        var compensated = await result.Compensate(_ => Result.Failure(rollbackError));

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateFailure>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });
    }

    [Fact]
    public async Task Compensate_WithSuccessResult_AsyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = Result.Success().AsAsync();
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe().Success();
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task Compensate_WithFailureAndSuccessfulRollback_AsyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var result = Result.Failure(originalError).AsAsync();
        Functional.Failures.Failure? receivedError = null;

        // Act (When)
        var compensated = await result.Compensate(RollbackAsync);

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error => Assert.Equal(originalError, error));
        Assert.Equal(originalError, receivedError);

        ResultTask RollbackAsync(Functional.Failures.Failure failure)
        {
            receivedError = failure;
            return RollbackCoreAsync().AsAsync();
        }

        async Task<Result> RollbackCoreAsync()
        {
            await Task.Delay(1);
            return Result.Success();
        }
    }

    [Fact]
    public async Task Compensate_WithFailureAndFailedRollback_AsyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var rollbackError = new Functional.Failures.Failure("Rollback error");
        var result = Result.Failure(originalError).AsAsync();

        // Act (When)
        var compensated = await result.Compensate(RollbackAsync);

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateFailure>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });

        ResultTask RollbackAsync(Functional.Failures.Failure failure)
        {
            return RollbackCoreAsync().AsAsync();
        }

        async Task<Result> RollbackCoreAsync()
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        }
    }

    [Fact]
    public async Task Compensate_Generic_WithSuccessResult_SyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42).AsAsync();
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task Compensate_Generic_WithFailureAndSuccessfulRollback_SyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var result = Result.Failure<int>(originalError).AsAsync();
        Functional.Failures.Failure? receivedError = null;

        // Act (When)
        var compensated = await result.Compensate(error =>
        {
            receivedError = error;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error => Assert.Equal(originalError, error));
        Assert.Equal(originalError, receivedError);
    }

    [Fact]
    public async Task Compensate_Generic_WithFailureAndFailedRollback_SyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var rollbackError = new Functional.Failures.Failure("Rollback error");
        var result = Result.Failure<string>(originalError).AsAsync();

        // Act (When)
        var compensated = await result.Compensate(_ => Result.Failure(rollbackError));

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateFailure>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });
    }

    [Fact]
    public async Task Compensate_Generic_WithSuccessResult_AsyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42).AsAsync();
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task Compensate_Generic_WithFailureAndSuccessfulRollback_AsyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var result = Result.Failure<int>(originalError).AsAsync();
        Functional.Failures.Failure? receivedError = null;

        // Act (When)
        var compensated = await result.Compensate(RollbackAsync);

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error => Assert.Equal(originalError, error));
        Assert.Equal(originalError, receivedError);

        ResultTask RollbackAsync(Functional.Failures.Failure failure)
        {
            receivedError = failure;
            return RollbackCoreAsync().AsAsync();
        }

        async Task<Result> RollbackCoreAsync()
        {
            await Task.Delay(1);
            return Result.Success();
        }
    }

    [Fact]
    public async Task Compensate_Generic_WithFailureAndFailedRollback_AsyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure("Original error");
        var rollbackError = new Functional.Failures.Failure("Rollback error");
        var result = Result.Failure<string>(originalError).AsAsync();

        // Act (When)
        var compensated = await result.Compensate(RollbackAsync);

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateFailure>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });

        ResultTask RollbackAsync(Functional.Failures.Failure failure)
        {
            return RollbackCoreAsync().AsAsync();
        }

        async Task<Result> RollbackCoreAsync()
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        }
    }

    [Theory]
    [InlineData("Error A", "Error B")]
    [InlineData("Database failure", "Rollback failure")]
    [InlineData("Network error", "Compensation error")]
    public async Task Compensate_WithMultipleErrorScenarios_AsyncRollback_CreatesCorrectAggregateError(
        string originalMsg, string rollbackMsg)
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure(originalMsg);
        var rollbackError = new Functional.Failures.Failure(rollbackMsg);
        var result = Result.Failure(originalError).AsAsync();

        // Act (When)
        var compensated = await result.Compensate(RollbackAsync);

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateFailure>(error);
                Assert.Equal(originalMsg, aggregateError.Errors.ElementAt(0).Message);
                Assert.Equal(rollbackMsg, aggregateError.Errors.ElementAt(1).Message);
            });

        ResultTask RollbackAsync(Functional.Failures.Failure failure)
        {
            return RollbackCoreAsync().AsAsync();
        }

        async Task<Result> RollbackCoreAsync()
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        }
    }

    [Theory]
    [InlineData("Error A", "Error B")]
    [InlineData("Database failure", "Rollback failure")]
    [InlineData("Network error", "Compensation error")]
    public async Task Compensate_Generic_WithMultipleErrorScenarios_AsyncRollback_CreatesCorrectAggregateError(
        string originalMsg, string rollbackMsg)
    {
        // Arrange (Given)
        var originalError = new Functional.Failures.Failure(originalMsg);
        var rollbackError = new Functional.Failures.Failure(rollbackMsg);
        var result = Result.Failure<int>(originalError).AsAsync();

        // Act (When)
        var compensated = await result.Compensate(RollbackAsync);

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateFailure>(error);
                Assert.Equal(originalMsg, aggregateError.Errors.ElementAt(0).Message);
                Assert.Equal(rollbackMsg, aggregateError.Errors.ElementAt(1).Message);
            });

        ResultTask RollbackAsync(Functional.Failures.Failure failure)
        {
            return RollbackCoreAsync().AsAsync();
        }

        async Task<Result> RollbackCoreAsync()
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        }
    }
}