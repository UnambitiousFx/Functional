using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.Fail alias factory methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Result.Fail Alias Tests

    [Fact]
    public void Fail_WithException_ReturnsFailure()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("boom");

        // Act (When)
        var result = Result.Fail(exception);

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public void Fail_WithFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var failure = new Failure("Test error");

        // Act (When)
        var result = Result.Fail(failure);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public void Fail_WithMultipleErrors_ReturnsAggregateFailure()
    {
        // Arrange (Given)
        var error1 = new Failure("error 1");
        var error2 = new Failure("error 2");

        // Act (When)
        var result = Result.Fail(error1, error2);

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public void Fail_Generic_WithException_ReturnsFailure()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("boom");

        // Act (When)
        var result = Result.Fail<int>(exception);

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    #endregion
}
