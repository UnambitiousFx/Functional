using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.FailValidation extension methods.
/// </summary>
public sealed class ResultFailValidationTests
{
    #region FailValidation - Non-Generic

    [Fact]
    public void FailValidation_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation("Invalid input");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailValidation_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation("Invalid input");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Validation);
    }

    [Fact]
    public void FailValidation_SetsProvidedMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation("Email must be valid");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Email must be valid");
    }

    [Fact]
    public void FailValidation_ErrorContainsFailuresList()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation("Invalid input");

        // Assert (Then)
        result.TryGetError(out Failure? error);
        Assert.IsType<ValidationFailure>(error);
        var validationError = (ValidationFailure)error;
        Assert.Single(validationError.Failures);
        Assert.Equal("Invalid input", validationError.Failures[0]);
    }

    [Fact]
    public void FailValidation_WithEmptyMessage_CreatesFailure()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation("");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .WhichIsValidationError()
            .WithFailureCount(1)
            .WithFailureContaining("");
    }

    [Fact]
    public void FailValidation_WithLongMessage_PreservesEntireMessage()
    {
        // Arrange (Given)
        var longMessage = "This is a very long validation error message that contains detailed information about what went wrong with the user's input and suggestions for how to fix it.";

        // Act (When)
        var result = Result.FailValidation(longMessage);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(longMessage);
    }

    #endregion

    #region FailValidation - Generic

    [Fact]
    public void FailValidation_Generic_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation<string>("Invalid input");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailValidation_Generic_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation<int>("Invalid input");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Validation);
    }

    [Fact]
    public void FailValidation_Generic_SetsProvidedMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation<string>("Email must be valid");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Email must be valid");
    }

    [Fact]
    public void FailValidation_Generic_ErrorContainsFailuresList()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation<string>("Invalid input");

        // Assert (Then)
        result.TryGetError(out Failure? error);
        Assert.IsType<ValidationFailure>(error);
        var validationError = (ValidationFailure)error;
        Assert.Single(validationError.Failures);
        Assert.Equal("Invalid input", validationError.Failures[0]);
    }

    [Fact]
    public void FailValidation_Generic_CanBeUsedWithComplexType()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation<Dictionary<string, object>>("Validation failed");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailValidation_Generic_CanBeChainedWithOtherOperations()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation<int>("Invalid number")
            .Recover(_ => 42);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    #endregion

    #region FailValidation - Edge Cases

    [Fact]
    public void FailValidation_WithSpecialCharacters_PreservesMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailValidation("Invalid: value contains <>&\"'");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Invalid: value contains <>&\"'");
    }

    [Fact]
    public void FailValidation_WithNewlines_PreservesFormatting()
    {
        // Arrange (Given)
        var message = "Validation failed:\n- Field 1 is required\n- Field 2 is invalid";

        // Act (When)
        var result = Result.FailValidation(message);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(message);
    }

    [Fact]
    public void FailValidation_CanBeUsedInBindChain()
    {
        // Arrange (Given)
        var successResult = Result.Success(42);

        // Act (When)
        var result = successResult.Bind(value =>
            value > 50
                ? Result.Success(value)
                : Result.FailValidation<int>("Value must be greater than 50"));

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Validation);
    }

    #endregion
}
