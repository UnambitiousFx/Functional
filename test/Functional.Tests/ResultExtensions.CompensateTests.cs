using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public class ResultExtensionsCompensateTests
{
    [Fact]
    public void Compensate_WithSuccessResult_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = Result.Success();
        var rollbackCalled = false;

        // Act (When)
        var compensated = result.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe().Success();
        Assert.False(rollbackCalled, "Rollback should not be called for successful result");
    }

    [Fact]
    public void Compensate_WithFailureAndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var result = Result.Failure(originalError);
        Error? receivedError = null;

        // Act (When)
        var compensated = result.Compensate(error =>
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
    public void Compensate_WithFailureAndFailedRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var rollbackError = new Error("Rollback error");
        var result = Result.Failure(originalError);

        // Act (When)
        var compensated = result.Compensate(_ => Result.Failure(rollbackError));

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
    public void Compensate_Generic_WithSuccessResult_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var rollbackCalled = false;

        // Act (When)
        var compensated = result.Compensate(_ =>
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
    public void Compensate_Generic_WithFailureAndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var result = Result.Failure<int>(originalError);
        Error? receivedError = null;

        // Act (When)
        var compensated = result.Compensate(error =>
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
    public void Compensate_Generic_WithFailureAndFailedRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Error("Original error");
        var rollbackError = new Error("Rollback error");
        var result = Result.Failure<string>(originalError);

        // Act (When)
        var compensated = result.Compensate(_ => Result.Failure(rollbackError));

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
    public void Compensate_WithMultipleErrorScenarios_CreatesCorrectAggregateError(string originalMsg,
        string rollbackMsg)
    {
        // Arrange (Given)
        var originalError = new Error(originalMsg);
        var rollbackError = new Error(rollbackMsg);
        var result = Result.Failure(originalError);

        // Act (When)
        var compensated = result.Compensate(_ => Result.Failure(rollbackError));

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
    public void Compensate_Generic_WithMultipleErrorScenarios_CreatesCorrectAggregateError(string originalMsg,
        string rollbackMsg)
    {
        // Arrange (Given)
        var originalError = new Error(originalMsg);
        var rollbackError = new Error(rollbackMsg);
        var result = Result.Failure<int>(originalError);

        // Act (When)
        var compensated = result.Compensate(_ => Result.Failure(rollbackError));

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