using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

public class ResultExtensionsCompensateTests
{
    [Fact]
    public async Task CompensateAsync_WithSuccessResult_SyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.CompensateAsync(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe().Success();
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task CompensateAsync_WithFailureAndSuccessfulRollback_SyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var result = ValueTask.FromResult(Result.Failure(originalError));
        Error? receivedError = null;

        // Act (When)
        var compensated = await result.CompensateAsync(error =>
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
    public async Task CompensateAsync_WithFailureAndFailedRollback_SyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var rollbackError = new Error("Rollback error");
        var result = ValueTask.FromResult(Result.Failure(originalError));

        // Act (When)
        var compensated = await result.CompensateAsync(_ => Result.Failure(rollbackError));

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateError>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });
    }

    [Fact]
    public async Task CompensateAsync_WithSuccessResult_AsyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.CompensateAsync(_ =>
        {
            rollbackCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        compensated.ShouldBe().Success();
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task CompensateAsync_WithFailureAndSuccessfulRollback_AsyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var result = ValueTask.FromResult(Result.Failure(originalError));
        Error? receivedError = null;

        // Act (When)
        var compensated = await result.CompensateAsync(async error =>
        {
            receivedError = error;
            await Task.Delay(1);
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error => Assert.Equal(originalError, error));
        Assert.Equal(originalError, receivedError);
    }

    [Fact]
    public async Task CompensateAsync_WithFailureAndFailedRollback_AsyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var rollbackError = new Error("Rollback error");
        var result = ValueTask.FromResult(Result.Failure(originalError));

        // Act (When)
        var compensated = await result.CompensateAsync(async _ =>
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateError>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });
    }

    [Fact]
    public async Task CompensateAsync_Generic_WithSuccessResult_SyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.CompensateAsync(_ =>
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
    public async Task CompensateAsync_Generic_WithFailureAndSuccessfulRollback_SyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var result = ValueTask.FromResult(Result.Failure<int>(originalError));
        Error? receivedError = null;

        // Act (When)
        var compensated = await result.CompensateAsync(error =>
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
    public async Task CompensateAsync_Generic_WithFailureAndFailedRollback_SyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var rollbackError = new Error("Rollback error");
        var result = ValueTask.FromResult(Result.Failure<string>(originalError));

        // Act (When)
        var compensated = await result.CompensateAsync(_ => Result.Failure(rollbackError));

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateError>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });
    }

    [Fact]
    public async Task CompensateAsync_Generic_WithSuccessResult_AsyncRollback_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));
        var rollbackCalled = false;

        // Act (When)
        var compensated = await result.CompensateAsync(_ =>
        {
            rollbackCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public async Task CompensateAsync_Generic_WithFailureAndSuccessfulRollback_AsyncRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var result = ValueTask.FromResult(Result.Failure<int>(originalError));
        Error? receivedError = null;

        // Act (When)
        var compensated = await result.CompensateAsync(async error =>
        {
            receivedError = error;
            await Task.Delay(1);
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error => Assert.Equal(originalError, error));
        Assert.Equal(originalError, receivedError);
    }

    [Fact]
    public async Task CompensateAsync_Generic_WithFailureAndFailedRollback_AsyncRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var rollbackError = new Error("Rollback error");
        var result = ValueTask.FromResult(Result.Failure<string>(originalError));

        // Act (When)
        var compensated = await result.CompensateAsync(async _ =>
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateError>(error);
                Assert.Equal(2, aggregateError.Errors.Count());
                Assert.Equal(originalError, aggregateError.Errors.ElementAt(0));
                Assert.Equal(rollbackError, aggregateError.Errors.ElementAt(1));
            });
    }

    [Theory]
    [InlineData("Error A", "Error B")]
    [InlineData("Database failure", "Rollback failure")]
    [InlineData("Network error", "Compensation error")]
    public async Task CompensateAsync_WithMultipleErrorScenarios_AsyncRollback_CreatesCorrectAggregateError(string originalMsg, string rollbackMsg)
    {
        // Arrange (Given)
        var originalError = new Error(originalMsg);
        var rollbackError = new Error(rollbackMsg);
        var result = ValueTask.FromResult(Result.Failure(originalError));

        // Act (When)
        var compensated = await result.CompensateAsync(async _ =>
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateError>(error);
                Assert.Equal(originalMsg, aggregateError.Errors.ElementAt(0).Message);
                Assert.Equal(rollbackMsg, aggregateError.Errors.ElementAt(1).Message);
            });
    }

    [Theory]
    [InlineData("Error A", "Error B")]
    [InlineData("Database failure", "Rollback failure")]
    [InlineData("Network error", "Compensation error")]
    public async Task CompensateAsync_Generic_WithMultipleErrorScenarios_AsyncRollback_CreatesCorrectAggregateError(string originalMsg, string rollbackMsg)
    {
        // Arrange (Given)
        var originalError = new Error(originalMsg);
        var rollbackError = new Error(rollbackMsg);
        var result = ValueTask.FromResult(Result.Failure<int>(originalError));

        // Act (When)
        var compensated = await result.CompensateAsync(async _ =>
        {
            await Task.Delay(1);
            return Result.Failure(rollbackError);
        });

        // Assert (Then)
        compensated.ShouldBe()
            .Failure()
            .And(error =>
            {
                var aggregateError = Assert.IsType<AggregateError>(error);
                Assert.Equal(originalMsg, aggregateError.Errors.ElementAt(0).Message);
                Assert.Equal(rollbackMsg, aggregateError.Errors.ElementAt(1).Message);
            });
    }
}