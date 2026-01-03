using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.xunit.Tests;

public class FluentAssertionExtensionsTests
{
    [Fact(DisplayName = "WhichIsValidationError returns ValidationErrorAssertion when error is ValidationError")]
    public void WhichIsValidationError_WhenValidationError_ReturnsValidationErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationError(["Field is required"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var validationAssertion = failureAssertion.WhichIsValidationError();

        // Assert (Then)
        Assert.NotNull(validationAssertion.Error);
        Assert.IsType<ValidationError>(validationAssertion.Error);
    }

    [Fact(DisplayName = "WhichIsValidationError throws when error is not ValidationError")]
    public void WhichIsValidationError_WhenNotValidationError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundError("User", "123"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsValidationError());
        Assert.Contains("Expected ValidationError but was NotFoundError", exception.Message);
    }

    [Fact(DisplayName = "WhichIsNotFoundError returns NotFoundErrorAssertion when error is NotFoundError")]
    public void WhichIsNotFoundError_WhenNotFoundError_ReturnsNotFoundErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundError("User", "123"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var notFoundAssertion = failureAssertion.WhichIsNotFoundError();

        // Assert (Then)
        Assert.NotNull(notFoundAssertion.Error);
        Assert.IsType<NotFoundError>(notFoundAssertion.Error);
    }

    [Fact(DisplayName = "WhichIsNotFoundError throws when error is not NotFoundError")]
    public void WhichIsNotFoundError_WhenNotNotFoundError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationError(["Invalid"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsNotFoundError());
        Assert.Contains("Expected NotFoundError but was ValidationError", exception.Message);
    }

    [Fact(DisplayName = "WhichIsConflictError returns ConflictErrorAssertion when error is ConflictError")]
    public void WhichIsConflictError_WhenConflictError_ReturnsConflictErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new ConflictError("Resource already exists"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var conflictAssertion = failureAssertion.WhichIsConflictError();

        // Assert (Then)
        Assert.NotNull(conflictAssertion.Error);
        Assert.IsType<ConflictError>(conflictAssertion.Error);
    }

    [Fact(DisplayName = "WhichIsConflictError throws when error is not ConflictError")]
    public void WhichIsConflictError_WhenNotConflictError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationError(["Invalid"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsConflictError());
        Assert.Contains("Expected ConflictError but was ValidationError", exception.Message);
    }

    [Fact(DisplayName = "WhichIs<TError> returns TypedErrorAssertion when error matches type")]
    public void WhichIs_WhenErrorMatchesType_ReturnsTypedErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new UnauthorizedError());
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIs<UnauthorizedError>();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Error);
        Assert.IsType<UnauthorizedError>(typedAssertion.Error);
    }

    [Fact(DisplayName = "WhichIs<TError> throws when error does not match type")]
    public void WhichIs_WhenErrorDoesNotMatchType_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationError(["Invalid"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIs<UnauthorizedError>());
        Assert.Contains("Expected UnauthorizedError but was ValidationError", exception.Message);
    }

    [Fact(DisplayName = "WhichIs<TError> works with custom error types")]
    public void WhichIs_WithCustomErrorType_WorksCorrectly()
    {
        // Arrange (Given)
        var customError = new Error("CUSTOM", "Custom error");
        var result = Result.Failure(customError);
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIs<Error>();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Error);
        Assert.Equal("CUSTOM", typedAssertion.Error.Code);
    }
}
