using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.FailNotFound extension methods.
/// </summary>
public sealed class ResultFailNotFoundTests
{
    #region FailNotFound - Non-Generic

    [Fact]
    public void FailNotFound_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User", "123");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailNotFound_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User", "123");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.NotFound);
    }

    [Fact]
    public void FailNotFound_GeneratesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User", "123");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Resource 'User' with id '123' was not found.");
    }

    [Fact]
    public void FailNotFound_WithMessageOverride_UsesCustomMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User", "123", "Custom not found message");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Custom not found message");
    }

    [Fact]
    public void FailNotFound_ErrorContainsResourceAndIdentifier()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("Product", "abc-123");

        // Assert (Then)
        result.TryGetError(out Failure? error);
        Assert.IsType<NotFoundFailure>(error);
        var notFoundError = (NotFoundFailure)error;
        Assert.Equal("Product", notFoundError.Resource);
        Assert.Equal("abc-123", notFoundError.Identifier);
    }

    [Fact]
    public void FailNotFound_WithEmptyResource_CreatesFailure()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("", "123");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .And(error => Assert.Contains("'' with id '123'", error.Message));
    }

    [Fact]
    public void FailNotFound_WithEmptyIdentifier_CreatesFailure()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User", "");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Resource 'User' with id '' was not found.");
    }

    #endregion

    #region FailNotFound - Generic

    [Fact]
    public void FailNotFound_Generic_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<string>("User", "123");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailNotFound_Generic_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<int>("User", "123");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.NotFound);
    }

    [Fact]
    public void FailNotFound_Generic_GeneratesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<string>("User", "123");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Resource 'User' with id '123' was not found.");
    }

    [Fact]
    public void FailNotFound_Generic_WithMessageOverride_UsesCustomMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<string>("User", "123", "Custom not found message");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Custom not found message");
    }

    [Fact]
    public void FailNotFound_Generic_ErrorContainsResourceAndIdentifier()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<string>("Product", "abc-123");

        // Assert (Then)
        result.TryGetError(out Failure? error);
        Assert.IsType<NotFoundFailure>(error);
        var notFoundError = (NotFoundFailure)error;
        Assert.Equal("Product", notFoundError.Resource);
        Assert.Equal("abc-123", notFoundError.Identifier);
    }

    [Fact]
    public void FailNotFound_Generic_CanBeUsedWithComplexType()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<Dictionary<string, object>>("User", "123");

        // Assert (Then)
        result.ShouldBe().Failure();
    }

    [Fact]
    public void FailNotFound_Generic_CanBeChainedWithOtherOperations()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound<int>("User", "123")
            .Recover(_ => 42);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    #endregion

    #region FailNotFound - Edge Cases

    [Fact]
    public void FailNotFound_WithSpecialCharactersInResource_CreatesFailure()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User@Email", "test@example.com");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .And(error => Assert.Contains("User@Email", error.Message))
            .And(error => Assert.Contains("test@example.com", error.Message));
    }

    [Fact]
    public void FailNotFound_WithNullMessageOverride_UsesDefaultMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailNotFound("User", "123");

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage("Resource 'User' with id '123' was not found.");
    }

    #endregion
}
