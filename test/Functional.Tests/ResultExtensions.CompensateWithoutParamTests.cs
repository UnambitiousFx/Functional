using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;
using AggregateFailure = UnambitiousFx.Functional.Failures.AggregateFailure;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result&lt;T&gt;.Compensate overloads that take no failure parameter.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Compensate Without Failure Param Tests

    [Fact]
    public void Compensate_Generic_WithoutFailureParam_WithSuccess_DoesNotCallRollback()
    {
        // Arrange (Given)
        var result         = Result.Success(42);
        var rollbackCalled = false;

        // Act (When)
        var compensated = result.Compensate(() => {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
                   .Success()
                   .And(v => Assert.Equal(42, v));
        Assert.False(rollbackCalled);
    }

    [Fact]
    public void Compensate_Generic_WithoutFailureParam_WithFailureAndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError  = new Failure("Original error");
        var result         = Result.Failure<int>(originalError);
        var rollbackCalled = false;

        // Act (When)
        var compensated = result.Compensate(() => {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        compensated.ShouldBe()
                   .Failure()
                   .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public void Compensate_Generic_WithoutFailureParam_WithFailureAndFailedRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var rollbackError = new Failure("Rollback error");
        var result        = Result.Failure<int>(originalError);

        // Act (When)
        var compensated = result.Compensate(() => Result.Failure(rollbackError));

        // Assert (Then)
        compensated.ShouldBe()
                   .Failure();
        Assert.True(compensated.TryGetFailure(out var error));
        Assert.IsType<AggregateFailure>(error);
    }

    #endregion
}
