using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.xunit.Tests;

public class FluentAssertionExtensionsTests
{
    [Fact(DisplayName = "WhichIsValidationError returns ValidationErrorAssertion when error is ValidationError")]
    public void WhichIsValidationError_WhenValidationError_ReturnsValidationErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Field is required"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var validationAssertion = failureAssertion.WhichIsValidationError();

        // Assert (Then)
        Assert.NotNull(validationAssertion.Failure);
        Assert.IsType<ValidationFailure>(validationAssertion.Failure);
    }

    [Fact(DisplayName = "WhichIsValidationError throws when error is not ValidationError")]
    public void WhichIsValidationError_WhenNotValidationError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundFailure("User", "123"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsValidationError());
        Assert.Contains("Expected ValidationError but was NotFoundError", exception.Message);
    }

    [Fact(DisplayName = "WhichIsNotFoundError returns NotFoundErrorAssertion when error is NotFoundError")]
    public void WhichIsNotFoundError_WhenNotFoundError_ReturnsNotFoundErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundFailure("User", "123"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var notFoundAssertion = failureAssertion.WhichIsNotFoundError();

        // Assert (Then)
        Assert.NotNull(notFoundAssertion.Failure);
        Assert.IsType<NotFoundFailure>(notFoundAssertion.Failure);
    }

    [Fact(DisplayName = "WhichIsNotFoundError throws when error is not NotFoundError")]
    public void WhichIsNotFoundError_WhenNotNotFoundError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsNotFoundError());
        Assert.Contains("Expected NotFoundError but was ValidationError", exception.Message);
    }

    [Fact(DisplayName = "WhichIsConflictError returns ConflictErrorAssertion when error is ConflictError")]
    public void WhichIsConflictError_WhenConflictError_ReturnsConflictErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new ConflictFailure("Resource already exists"));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var conflictAssertion = failureAssertion.WhichIsConflictError();

        // Assert (Then)
        Assert.NotNull(conflictAssertion.Failure);
        Assert.IsType<ConflictFailure>(conflictAssertion.Failure);
    }

    [Fact(DisplayName = "WhichIsConflictError throws when error is not ConflictError")]
    public void WhichIsConflictError_WhenNotConflictError_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIsConflictError());
        Assert.Contains("Expected ConflictError but was ValidationError", exception.Message);
    }

    [Fact(DisplayName = "WhichIs<TError> returns TypedErrorAssertion when error matches type")]
    public void WhichIs_WhenErrorMatchesType_ReturnsTypedErrorAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new UnauthorizedFailure());
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIs<UnauthorizedFailure>();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Error);
        Assert.IsType<UnauthorizedFailure>(typedAssertion.Error);
    }

    [Fact(DisplayName = "WhichIs<TError> throws when error does not match type")]
    public void WhichIs_WhenErrorDoesNotMatchType_Throws()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid"]));
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When) & Assert (Then)
        var exception = Assert.Throws<Xunit.Sdk.FailException>(() => failureAssertion.WhichIs<UnauthorizedFailure>());
        Assert.Contains("Expected UnauthorizedError but was ValidationError", exception.Message);
    }

    [Fact(DisplayName = "WhichIs<TError> works with custom error types")]
    public void WhichIs_WithCustomErrorType_WorksCorrectly()
    {
        // Arrange (Given)
        var customError = new Failure("CUSTOM", "Custom error");
        var result = Result.Failure(customError);
        var failureAssertion = result.ShouldBe().Failure();

        // Act (When)
        var typedAssertion = failureAssertion.WhichIs<Failure>();

        // Assert (Then)
        Assert.NotNull(typedAssertion.Error);
        Assert.Equal("CUSTOM", typedAssertion.Error.Code);
    }
}
