using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void PrependError_WithSuccess_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var prepended = result.PrependError("Error: ");

        // Assert (Then)
        prepended.ShouldBe().Success();
    }

    [Fact]
    public void PrependError_WithFailure_PrependsToMessage()
    {
        // Arrange (Given)
        var error = new Error("connection refused");
        var result = Result.Failure(error);

        // Act (When)
        var prepended = result.PrependError("Database error: ");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Database error: connection refused");
    }

    [Fact]
    public void PrependError_WithEmptyPrefix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);

        // Act (When)
        var prepended = result.PrependError("");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Test error");
    }

    [Fact]
    public void PrependError_WithNullPrefix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);

        // Act (When)
        var prepended = result.PrependError(null!);

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Test error");
    }

    [Fact]
    public void PrependError_GenericResult_WithFailure_PrependsToMessage()
    {
        // Arrange (Given)
        var error = new Error("not found");
        var result = Result.Failure<string>(error);

        // Act (When)
        var prepended = result.PrependError("User ");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("User not found");
    }
}
