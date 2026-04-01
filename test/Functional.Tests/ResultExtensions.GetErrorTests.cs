using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for the result.GetError&lt;TError&gt;() internal method.
/// </summary>
public sealed partial class ResultExtensions
{
    #region GetError Internal Method Tests

    [Fact]
    public void GetError_WithSuccess_ReturnsNone()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var error = result.GetError<ValidationFailure>();

        // Assert (Then)
        error.ShouldBe()
             .None();
    }

    [Fact]
    public void GetError_WithMatchingFailureType_ReturnsSome()
    {
        // Arrange (Given)
        var failure = new ValidationFailure("validation failed");
        var result  = Result.Failure(failure);

        // Act (When)
        var error = result.GetError<ValidationFailure>();

        // Assert (Then)
        error.ShouldBe()
             .Some()
             .And(e => Assert.Equal("validation failed", e.Message));
    }

    [Fact]
    public void GetError_WithNonMatchingFailureType_ReturnsNone()
    {
        // Arrange (Given)
        var failure = new NotFoundFailure("user", "123");
        var result  = Result.Failure(failure);

        // Act (When)
        var error = result.GetError<ValidationFailure>();

        // Assert (Then)
        error.ShouldBe()
             .None();
    }

    [Fact]
    public void GetError_WithAggregateFailureContainingMatchingType_ReturnsSome()
    {
        // Arrange (Given)
        var validation = new ValidationFailure("validation failed");
        var notFound   = new NotFoundFailure("user", "123");
        var aggregate  = new AggregateFailure(validation, notFound);
        var result     = Result.Failure(aggregate);

        // Act (When)
        var error = result.GetError<ValidationFailure>();

        // Assert (Then)
        error.ShouldBe()
             .Some()
             .And(e => Assert.Equal("validation failed", e.Message));
    }

    [Fact]
    public void GetError_WithAggregateFailureWithoutMatchingType_ReturnsNone()
    {
        // Arrange (Given)
        var notFound  = new NotFoundFailure("user", "123");
        var aggregate = new AggregateFailure(notFound);
        var result    = Result.Failure(aggregate);

        // Act (When)
        var error = result.GetError<ValidationFailure>();

        // Assert (Then)
        error.ShouldBe()
             .None();
    }

    #endregion
}
