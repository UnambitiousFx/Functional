using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.FailConflict extension methods.
/// </summary>
public sealed class ResultFailConflictTests
{
    #region FailConflict - Non-Generic

    [Fact]
    public void FailConflict_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict("Resource already exists");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailConflict_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict("Duplicate entry");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Conflict);
    }

    [Fact]
    public void FailConflict_SetsProvidedMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict("User with email already exists");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("User with email already exists");
    }

    [Fact]
    public void FailConflict_ErrorIsConflictError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict("Duplicate key");

        // Assert (Then)
        result.TryGet(out Error? error);
        Assert.IsType<ConflictError>(error);
    }

    [Fact]
    public void FailConflict_WithEmptyMessage_CreatesFailure()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict("");

        // Assert (Then)
        result.ShouldBe().Failure();
        result.ShouldBe().Failure().AndMessage("");
    }

    [Fact]
    public void FailConflict_WithLongMessage_PreservesEntireMessage()
    {
        // Arrange (Given)
        var longMessage = "A conflict occurred because the resource you are trying to create already exists with the same unique identifier. Please use a different identifier or update the existing resource instead.";

        // Act (When)
        var result = Result.FailConflict(longMessage);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(longMessage);
    }

    #endregion

    #region FailConflict - Generic

    [Fact]
    public void FailConflict_Generic_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict<string>("Resource already exists");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailConflict_Generic_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict<int>("Duplicate entry");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Conflict);
    }

    [Fact]
    public void FailConflict_Generic_SetsProvidedMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict<string>("User with email already exists");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("User with email already exists");
    }

    [Fact]
    public void FailConflict_Generic_ErrorIsConflictError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict<string>("Duplicate key");

        // Assert (Then)
        result.TryGet(out Error? error);
        Assert.IsType<ConflictError>(error);
    }

    [Fact]
    public void FailConflict_Generic_CanBeUsedWithComplexType()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict<Dictionary<string, object>>("Duplicate resource");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailConflict_Generic_CanBeChainedWithOtherOperations()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict<int>("Duplicate ID")
            .Recover(_ => 42);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    #endregion

    #region FailConflict - Edge Cases

    [Fact]
    public void FailConflict_WithSpecialCharacters_PreservesMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailConflict("Conflict: resource '<id>' already exists");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Conflict: resource '<id>' already exists");
    }

    [Fact]
    public void FailConflict_CanBeUsedInBindChain()
    {
        // Arrange (Given)
        var successResult = Result.Success("user@example.com");
        var existingEmails = new List<string> { "user@example.com", "admin@example.com" };

        // Act (When)
        var result = successResult.Bind(email =>
            existingEmails.Contains(email)
                ? Result.FailConflict<string>("Email already registered")
                : Result.Success(email));

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Conflict);
    }

    [Fact]
    public void FailConflict_WithDatabaseConstraintMessage_PreservesDetails()
    {
        // Arrange (Given)
        var message = "UNIQUE constraint failed: users.email";

        // Act (When)
        var result = Result.FailConflict(message);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(message);
    }

    [Fact]
    public void FailConflict_WithJsonInMessage_PreservesFormatting()
    {
        // Arrange (Given)
        var message = "Conflict with existing resource: {\"id\": 123, \"name\": \"test\"}";

        // Act (When)
        var result = Result.FailConflict(message);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(message);
    }

    #endregion
}
