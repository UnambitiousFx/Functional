using UnambitiousFx.Functional.Failures;
using Xunit.Sdk;

namespace UnambitiousFx.Functional.xunit.Tests;

public class FluentAssertionExtensionsTests
{
    [Fact]
    public void WhichIsValidationError_WhenValidationError_ReturnsValidationFailureAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Field is required"]));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When)
        var validationAssertion = failureAssertion.WhichIsValidationError();

        // Assert (Then)
        Assert.NotNull(validationAssertion.Failure);
        Assert.IsType<ValidationFailure>(validationAssertion.Failure);
    }

    [Fact]
    public void WhichIsValidationError_WhenNotValidationError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundFailure("User", "123"));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<FailException>(() => failureAssertion.WhichIsValidationError());
        Assert.Contains("Expected ValidationError but was NotFoundFailure", exception.Message);
    }

    [Fact]
    public void WhichIsNotFoundError_WhenNotFoundError_ReturnsNotFoundFailureAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundFailure("User", "123"));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When)
        var notFoundAssertion = failureAssertion.WhichIsNotFoundError();

        // Assert (Then)
        Assert.NotNull(notFoundAssertion.Failure);
        Assert.IsType<NotFoundFailure>(notFoundAssertion.Failure);
    }

    [Fact]
    public void WhichIsNotFoundError_WhenNotNotFoundError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid"]));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<FailException>(() => failureAssertion.WhichIsNotFoundError());
        Assert.Contains("Expected NotFoundError but was ValidationFailure", exception.Message);
    }

    [Fact]
    public void WhichIsConflictError_WhenConflictError_ReturnsConflictFailureAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new ConflictFailure("Resource already exists"));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When)
        var conflictAssertion = failureAssertion.WhichIsConflictError();

        // Assert (Then)
        Assert.NotNull(conflictAssertion.Failure);
        Assert.IsType<ConflictFailure>(conflictAssertion.Failure);
    }

    [Fact]
    public void WhichIsConflictError_WhenNotConflictError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid"]));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<FailException>(() => failureAssertion.WhichIsConflictError());
        Assert.Contains("Expected ConflictError but was ValidationFailure", exception.Message);
    }

    [Fact]
    public void WhichIs_WhenErrorMatchesType_ReturnsTypedFailureAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new UnauthorizedFailure());
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIs<UnauthorizedFailure>();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Failure);
        Assert.IsType<UnauthorizedFailure>(typedAssertion.Failure);
    }

    [Fact]
    public void WhichIs_WhenErrorDoesNotMatchType_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid"]));
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<FailException>(() => failureAssertion.WhichIs<UnauthorizedFailure>());
        Assert.Contains("Expected UnauthorizedFailure but was ValidationFailure", exception.Message);
    }

    [Fact]
    public void WhichIs_WithCustomErrorType_WorksCorrectly()
    {
        // Arrange (Given)
        var customError = new Failure("CUSTOM", "Custom error");
        var result      = Result.Failure(customError);
        var failureAssertion = result.ShouldBe()
                                     .Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIs<Failure>();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Failure);
        Assert.Equal("CUSTOM", typedAssertion.Failure.Code);
    }

    [Fact]
    public void WhichIsTimeoutError_WhenTimeoutFailure_ReturnsTypedAssertion()
    {
        // Arrange (Given)
        var failure          = new TimeoutFailure(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(6));
        var result           = Result.Failure(failure);
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIsTimeoutError();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Failure);
        Assert.IsType<TimeoutFailure>(typedAssertion.Failure);
    }

    [Fact]
    public void WhichIsTimeoutError_WhenNotTimeoutFailure_ThrowsFailException()
    {
        // Arrange (Given)
        var result           = Result.Failure(new Failure("ERR", "not timeout"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsTimeoutError());
    }

    [Fact]
    public void WhichIsUnauthenticatedError_WhenUnauthenticatedFailure_ReturnsTypedAssertion()
    {
        // Arrange (Given)
        var failure          = new UnauthenticatedFailure();
        var result           = Result.Failure(failure);
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIsUnauthenticatedError();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Failure);
        Assert.IsType<UnauthenticatedFailure>(typedAssertion.Failure);
    }

    [Fact]
    public void WhichIsUnauthenticatedError_WhenNotUnauthenticatedFailure_ThrowsFailException()
    {
        // Arrange (Given)
        var result           = Result.Failure(new Failure("ERR", "not unauthenticated"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsUnauthenticatedError());
    }

    [Fact]
    public void WhichIsUnauthorizedError_WhenUnauthorizedFailure_ReturnsTypedAssertion()
    {
        // Arrange (Given)
        var failure          = new UnauthorizedFailure();
        var result           = Result.Failure(failure);
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIsUnauthorizedError();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Failure);
        Assert.IsType<UnauthorizedFailure>(typedAssertion.Failure);
    }

    [Fact]
    public void WhichIsUnauthorizedError_WhenNotUnauthorizedFailure_ThrowsFailException()
    {
        // Arrange (Given)
        var result           = Result.Failure(new Failure("ERR", "not unauthorized"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsUnauthorizedError());
    }
}
