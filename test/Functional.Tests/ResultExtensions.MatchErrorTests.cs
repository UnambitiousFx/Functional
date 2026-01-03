using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void MatchError_WithMatchingErrorType_ExecutesOnMatch()
    {
        // Arrange (Given)
        var error = new ValidationError(new[] { "Field required" });
        var result = Result.Failure(error);

        // Act (When)
        var matched = result.MatchError<ValidationError, string>(
            validationError => $"Validation failed with {validationError.Failures.Count} errors",
            () => "No validation error");

        // Assert (Then)
        Assert.Equal("Validation failed with 1 errors", matched);
    }

    [Fact]
    public void MatchError_WithNonMatchingErrorType_ExecutesOnElse()
    {
        // Arrange (Given)
        var error = new Error("Simple error");
        var result = Result.Failure(error);

        // Act (When)
        var matched = result.MatchError<ValidationError, string>(
            validationError => "Matched validation error",
            () => "No validation error found");

        // Assert (Then)
        Assert.Equal("No validation error found", matched);
    }

    [Fact]
    public void MatchError_WithSuccess_ExecutesOnElse()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var matched = result.MatchError<ValidationError, string>(
            validationError => "Matched validation error",
            () => "Success - no error");

        // Assert (Then)
        Assert.Equal("Success - no error", matched);
    }

    [Fact]
    public void MatchError_GenericResult_WithMatchingError_ExecutesOnMatch()
    {
        // Arrange (Given)
        var error = new ValidationError(new[] { "Invalid" });
        var result = Result.Failure<int>(error);

        // Act (When)
        var matched = result.MatchError<ValidationError, int, int>(
            validationError => -1,
            () => 0);

        // Assert (Then)
        Assert.Equal(-1, matched);
    }
}
