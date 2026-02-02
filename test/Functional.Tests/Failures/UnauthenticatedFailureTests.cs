using UnambitiousFx.Functional.Failures;
using UnauthenticatedFailure = UnambitiousFx.Functional.Failures.UnauthenticatedFailure;

namespace UnambitiousFx.Functional.Tests.Failures;

/// <summary>
///     Tests for UnauthenticatedError type.
/// </summary>
public class UnauthenticatedFailureTests
{
    [Fact]
    public void UnauthenticatedError_DefaultConstructor_SetsDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedFailure();

        // Assert (Then)
        Assert.Equal((string?)"Unauthenticated", (string?)error.Message);
        Assert.Equal((string?)ErrorCodes.Unauthenticated, (string?)error.Code);
    }

    [Fact]
    public void UnauthenticatedError_WithReason_SetsCustomMessage()
    {
        // Arrange (Given)
        var reason = "Invalid authentication token";

        // Act (When)
        var error = new UnauthenticatedFailure(reason);

        // Assert (Then)
        Assert.Equal((string?)"Invalid authentication token", (string?)error.Message);
        Assert.Equal((string?)ErrorCodes.Unauthenticated, (string?)error.Code);
    }

    [Fact]
    public void UnauthenticatedError_Code_IsUnauthenticatedErrorCode()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedFailure();

        // Assert (Then)
        Assert.Equal((string?)ErrorCodes.Unauthenticated, (string?)error.Code);
    }

    [Fact]
    public void UnauthenticatedError_WithNullReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedFailure();

        // Assert (Then)
        Assert.Equal((string?)"Unauthenticated", (string?)error.Message);
    }

    [Fact]
    public void UnauthenticatedError_WithExtra_PreservesExtraData()
    {
        // Arrange (Given)
        var extra = new Dictionary<string, object?>
        {
            ["userId"] = "user123",
            ["attemptedResource"] = "/api/secure"
        };

        // Act (When)
        var error = new UnauthenticatedFailure("Session expired", extra);

        // Assert (Then)
        Assert.Equal((string?)"Session expired", (string?)error.Message);
        Assert.NotNull(error.Metadata);
        Assert.Equal<object>("user123", error.Metadata["userId"]);
        Assert.Equal<object>("/api/secure", error.Metadata["attemptedResource"]);
    }

    [Fact]
    public void UnauthenticatedError_RecordEquality_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new UnauthenticatedFailure("Token expired");
        var error2 = new UnauthenticatedFailure("Token expired");

        // Act & Assert (When/Then)
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void UnauthenticatedError_RecordInequality_WithDifferentReason_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new UnauthenticatedFailure("Token expired");
        var error2 = new UnauthenticatedFailure("Invalid credentials");

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void UnauthenticatedError_RecordInequality_WithDifferentExtra_WorksCorrectly()
    {
        // Arrange (Given)
        var extra1 = new Dictionary<string, object?> { ["key"] = "value1" };
        var extra2 = new Dictionary<string, object?> { ["key"] = "value2" };
        var error1 = new UnauthenticatedFailure("Token expired", extra1);
        var error2 = new UnauthenticatedFailure("Token expired", extra2);

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void UnauthenticatedError_CanBeUsedInResult()
    {
        // Arrange (Given)
        var unauthenticatedError = new UnauthenticatedFailure("Missing authentication header");

        // Act (When)
        var result = Result.Failure<string>(unauthenticatedError);

        // Assert (Then)
        Assert.True(result.IsFailure);
        result.TryGetError(out var error);
        Assert.IsType<UnauthenticatedFailure>(error);
        var typedError = (UnauthenticatedFailure)error;
        Assert.Equal((string?)"Missing authentication header", (string?)typedError.Message);
        Assert.Equal((string?)ErrorCodes.Unauthenticated, (string?)typedError.Code);
    }

    [Fact]
    public void UnauthenticatedError_WithEmptyReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedFailure("");

        // Assert (Then)
        Assert.Equal((string?)"Unauthenticated", (string?)error.Message);
    }

    [Fact]
    public void UnauthenticatedError_InheritsFromError()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedFailure();

        // Assert (Then)
        Assert.IsAssignableFrom<Failure>(error);
    }

    [Fact]
    public void UnauthenticatedError_WithComplexReasonMessage_PreservesMessage()
    {
        // Arrange (Given)
        var complexReason =
            "Authentication failed: JWT token signature verification failed. Token may have been tampered with.";

        // Act (When)
        var error = new UnauthenticatedFailure(complexReason);

        // Assert (Then)
        Assert.Equal(complexReason, (string?)error.Message);
    }
}