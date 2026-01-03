using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void AppendError_WithSuccess_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var appended = result.AppendError(" - additional info");

        // Assert (Then)
        appended.ShouldBe().Success();
    }

    [Fact]
    public void AppendError_WithFailure_AppendsToMessage()
    {
        // Arrange (Given)
        var error = new Error("Database error");
        var result = Result.Failure(error);

        // Act (When)
        var appended = result.AppendError(" - connection timeout");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Database error - connection timeout");
    }

    [Fact]
    public void AppendError_WithEmptySuffix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);

        // Act (When)
        var appended = result.AppendError("");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Test error");
    }

    [Fact]
    public void AppendError_WithNullSuffix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);

        // Act (When)
        var appended = result.AppendError(null!);

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Test error");
    }

    [Fact]
    public void AppendError_GenericResult_WithFailure_AppendsToMessage()
    {
        // Arrange (Given)
        var error = new Error("Validation failed");
        var result = Result.Failure<int>(error);

        // Act (When)
        var appended = result.AppendError(" - field: age");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Validation failed - field: age");
    }
}
