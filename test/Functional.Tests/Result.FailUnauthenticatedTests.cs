using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.FailUnauthenticated extension methods.
/// </summary>
public sealed class ResultFailUnauthenticatedTests
{
    #region FailUnauthenticated - Non-Generic

    [Fact]
    public void FailUnauthenticated_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated("Invalid token");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailUnauthenticated_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated("Invalid token");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthenticated);
    }

    [Fact]
    public void FailUnauthenticated_WithReason_SetsCustomMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated("Session expired");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Session expired");
    }

    [Fact]
    public void FailUnauthenticated_WithNullReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated(null);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Unauthenticated");
    }

    [Fact]
    public void FailUnauthenticated_ErrorIsUnauthenticatedError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated("Invalid credentials");

        // Assert (Then)
        result.TryGet(out Error? error);
        Assert.IsType<UnauthenticatedError>(error);
    }

    [Fact]
    public void FailUnauthenticated_WithEmptyReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated("");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Unauthenticated");
    }

    #endregion

    #region FailUnauthenticated - Generic

    [Fact]
    public void FailUnauthenticated_Generic_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<string>("Invalid token");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailUnauthenticated_Generic_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<int>("Invalid token");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthenticated);
    }

    [Fact]
    public void FailUnauthenticated_Generic_WithReason_SetsCustomMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<string>("Session expired");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Session expired");
    }

    [Fact]
    public void FailUnauthenticated_Generic_WithNullReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<string>(null);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Unauthenticated");
    }

    [Fact]
    public void FailUnauthenticated_Generic_ErrorIsUnauthenticatedError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<string>("Invalid credentials");

        // Assert (Then)
        result.TryGet(out Error? error);
        Assert.IsType<UnauthenticatedError>(error);
    }

    [Fact]
    public void FailUnauthenticated_Generic_CanBeUsedWithComplexType()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<Dictionary<string, object>>("Missing auth header");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailUnauthenticated_Generic_CanBeChainedWithOtherOperations()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated<int>("Invalid token")
            .Recover(_ => 42);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    #endregion

    #region FailUnauthenticated - Edge Cases

    [Fact]
    public void FailUnauthenticated_WithDetailedReason_PreservesMessage()
    {
        // Arrange (Given)
        var detailedReason = "JWT token signature verification failed. Token may have been tampered with.";

        // Act (When)
        var result = Result.FailUnauthenticated(detailedReason);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(detailedReason);
    }

    [Fact]
    public void FailUnauthenticated_CanBeUsedInBindChain()
    {
        // Arrange (Given)
        var successResult = Result.Success("token123");

        // Act (When)
        var result = successResult.Bind(token =>
            string.IsNullOrEmpty(token)
                ? Result.FailUnauthenticated<string>("No token provided")
                : Result.Success(token));

        // Assert (Then)
        result.ShouldBe().Success();
    }

    [Fact]
    public void FailUnauthenticated_WithSpecialCharacters_PreservesReason()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthenticated("Token contains invalid characters: <>&\"'");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Token contains invalid characters: <>&\"'");
    }

    #endregion
}
