using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void HasError_WithSuccess_ReturnsFalse()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var hasError = result.HasError<ValidationError>();

        // Assert (Then)
        Assert.False(hasError);
    }

    [Fact]
    public void HasError_WithMatchingErrorType_ReturnsTrue()
    {
        // Arrange (Given)
        var error = new ValidationError(new[] { "Field is required" });
        var result = Result.Failure(error);

        // Act (When)
        var hasError = result.HasError<ValidationError>();

        // Assert (Then)
        Assert.True(hasError);
    }

    [Fact]
    public void HasError_WithDifferentErrorType_ReturnsFalse()
    {
        // Arrange (Given)
        var error = new Error("Simple error");
        var result = Result.Failure(error);

        // Act (When)
        var hasError = result.HasError<ValidationError>();

        // Assert (Then)
        Assert.False(hasError);
    }

    [Fact]
    public void HasError_WithBaseErrorType_ReturnsTrue()
    {
        // Arrange (Given)
        var error = new Error("Simple error");
        var result = Result.Failure(error);

        // Act (When)
        var hasError = result.HasError<Error>();

        // Assert (Then)
        Assert.True(hasError);
    }
}
