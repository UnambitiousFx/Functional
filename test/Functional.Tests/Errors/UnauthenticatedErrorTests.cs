using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests.Errors;

/// <summary>
/// Tests for UnauthenticatedError type.
/// </summary>
public class UnauthenticatedErrorTests
{
    [Fact]
    public void UnauthenticatedError_DefaultConstructor_SetsDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedError();

        // Assert (Then)
        Assert.Equal("Unauthenticated.", error.Message);
        Assert.Equal(ErrorCodes.Unauthenticated, error.Code);
    }

    [Fact]
    public void UnauthenticatedError_WithReason_SetsCustomMessage()
    {
        // Arrange (Given)
        var reason = "Invalid authentication token";

        // Act (When)
        var error = new UnauthenticatedError(reason);

        // Assert (Then)
        Assert.Equal("Invalid authentication token", error.Message);
        Assert.Equal(ErrorCodes.Unauthenticated, error.Code);
    }

    [Fact]
    public void UnauthenticatedError_Code_IsUnauthenticatedErrorCode()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedError();

        // Assert (Then)
        Assert.Equal(ErrorCodes.Unauthenticated, error.Code);
    }

    [Fact]
    public void UnauthenticatedError_WithNullReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedError(null);

        // Assert (Then)
        Assert.Equal("Unauthenticated.", error.Message);
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
        var error = new UnauthenticatedError("Session expired", extra);

        // Assert (Then)
        Assert.Equal("Session expired", error.Message);
        Assert.NotNull(error.Extra);
        Assert.Equal("user123", error.Extra["userId"]);
        Assert.Equal("/api/secure", error.Extra["attemptedResource"]);
    }

    [Fact]
    public void UnauthenticatedError_RecordEquality_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new UnauthenticatedError("Token expired");
        var error2 = new UnauthenticatedError("Token expired");

        // Act & Assert (When/Then)
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void UnauthenticatedError_RecordInequality_WithDifferentReason_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new UnauthenticatedError("Token expired");
        var error2 = new UnauthenticatedError("Invalid credentials");

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void UnauthenticatedError_RecordInequality_WithDifferentExtra_WorksCorrectly()
    {
        // Arrange (Given)
        var extra1 = new Dictionary<string, object?> { ["key"] = "value1" };
        var extra2 = new Dictionary<string, object?> { ["key"] = "value2" };
        var error1 = new UnauthenticatedError("Token expired", extra1);
        var error2 = new UnauthenticatedError("Token expired", extra2);

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void UnauthenticatedError_CanBeUsedInResult()
    {
        // Arrange (Given)
        var unauthenticatedError = new UnauthenticatedError("Missing authentication header");

        // Act (When)
        var result = Result.Failure<string>(unauthenticatedError);

        // Assert (Then)
        Assert.True(result.IsFaulted);
        result.TryGet(out Error? error);
        Assert.IsType<UnauthenticatedError>(error);
        var typedError = (UnauthenticatedError)error;
        Assert.Equal("Missing authentication header", typedError.Message);
        Assert.Equal(ErrorCodes.Unauthenticated, typedError.Code);
    }

    [Fact]
    public void UnauthenticatedError_WithEmptyReason_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedError("");

        // Assert (Then)
        Assert.Equal("Unauthenticated.", error.Message);
    }

    [Fact]
    public void UnauthenticatedError_InheritsFromError()
    {
        // Arrange & Act (Given/When)
        var error = new UnauthenticatedError();

        // Assert (Then)
        Assert.IsAssignableFrom<Error>(error);
    }

    [Fact]
    public void UnauthenticatedError_WithComplexReasonMessage_PreservesMessage()
    {
        // Arrange (Given)
        var complexReason = "Authentication failed: JWT token signature verification failed. Token may have been tampered with.";

        // Act (When)
        var error = new UnauthenticatedError(complexReason);

        // Assert (Then)
        Assert.Equal(complexReason, error.Message);
    }
}
