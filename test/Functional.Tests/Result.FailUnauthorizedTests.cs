using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.FailUnauthorized extension methods.
/// </summary>
public sealed class ResultFailUnauthorizedTests
{
    #region FailUnauthorized - Non-Generic

    [Fact]
    public void FailUnauthorized_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized("Insufficient permissions");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailUnauthorized_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized("Insufficient permissions");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthorized);
    }

    [Fact]
    public void FailUnauthorized_WithReason_SetsCustomMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized("Access denied");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Access denied");
    }

    [Fact]
    public void FailUnauthorized_WithNullReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized(null);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Unauthorized.");
    }

    [Fact]
    public void FailUnauthorized_ErrorIsUnauthorizedError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized("Permission denied");

        // Assert (Then)
        result.TryGet(out Error? error);
        Assert.IsType<UnauthorizedError>(error);
    }

    [Fact]
    public void FailUnauthorized_WithEmptyReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized("");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Unauthorized");
    }

    #endregion

    #region FailUnauthorized - Generic

    [Fact]
    public void FailUnauthorized_Generic_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<string>("Insufficient permissions");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailUnauthorized_Generic_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<int>("Insufficient permissions");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthorized);
    }

    [Fact]
    public void FailUnauthorized_Generic_WithReason_SetsCustomMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<string>("Access denied");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Access denied");
    }

    [Fact]
    public void FailUnauthorized_Generic_WithNullReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<string>(null);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Unauthorized.");
    }

    [Fact]
    public void FailUnauthorized_Generic_ErrorIsUnauthorizedError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<string>("Permission denied");

        // Assert (Then)
        result.TryGet(out Error? error);
        Assert.IsType<UnauthorizedError>(error);
    }

    [Fact]
    public void FailUnauthorized_Generic_CanBeUsedWithComplexType()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<Dictionary<string, object>>("Admin access required");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailUnauthorized_Generic_CanBeChainedWithOtherOperations()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized<int>("Permission denied")
            .Recover(_ => 42);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    #endregion

    #region FailUnauthorized - Edge Cases

    [Fact]
    public void FailUnauthorized_WithDetailedReason_PreservesMessage()
    {
        // Arrange (Given)
        var detailedReason = "User does not have the required role 'Administrator' to perform this action.";

        // Act (When)
        var result = Result.FailUnauthorized(detailedReason);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(detailedReason);
    }

    [Fact]
    public void FailUnauthorized_CanBeUsedInBindChain()
    {
        // Arrange (Given)
        var successResult = Result.Success("user");

        // Act (When)
        var result = successResult.Bind(role =>
            role == "admin"
                ? Result.Success(role)
                : Result.FailUnauthorized<string>("Admin role required"));

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthorized);
    }

    [Fact]
    public void FailUnauthorized_WithSpecialCharacters_PreservesReason()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailUnauthorized("Access denied for resource: /api/admin/<id>");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Access denied for resource: /api/admin/<id>");
    }

    [Fact]
    public void FailUnauthorized_DifferentFromUnauthenticated()
    {
        // Arrange & Act (Given/When)
        var unauthorizedResult = Result.FailUnauthorized("No permission");
        var unauthenticatedResult = Result.FailUnauthenticated("No token");

        // Assert (Then)
        unauthorizedResult.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthorized);
        unauthenticatedResult.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Unauthenticated);
    }

    #endregion
}
