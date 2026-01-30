using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsToResultTests
{
    [Fact]
    public void ToResult_WithError_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var error = new Failure("code", "message");

        // Act (When)
        var result = maybe.ToResult(error);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void ToResult_WithError_WithNone_ReturnsFailure()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var error = new Failure("code", "message");

        // Act (When)
        var result = maybe.ToResult(error);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(error.Code)
            .AndMessage(error.Message);
    }

    [Fact]
    public void ToResult_WithErrorFactory_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var result = maybe.ToResult(() => new Failure("code", "message"));

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void ToResult_WithErrorFactory_WithNone_ReturnsFailure()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var error = new Failure("code", "message");

        // Act (When)
        var result = maybe.ToResult(() => error);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(error.Code)
            .AndMessage(error.Message);
    }

    [Fact]
    public void ToResult_WithMessage_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var message = "error message";

        // Act (When)
        var result = maybe.ToResult(message);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void ToResult_WithMessage_WithNone_ReturnsFailure()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var message = "error message";

        // Act (When)
        var result = maybe.ToResult(message);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(message);
    }
}
