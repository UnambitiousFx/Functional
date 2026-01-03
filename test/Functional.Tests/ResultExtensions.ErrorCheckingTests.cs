using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Combined Operations

    [Fact]
    public void PrependError_AndAppendError_CanBeCombined()
    {
        // Arrange (Given)
        var error = new Error("failed");
        var result = Result.Failure(error);

        // Act (When)
        var modified = result
            .PrependError("Operation ")
            .AppendError(" - retry later");

        // Assert (Then)
        modified.ShouldBe().Failure().AndMessage("Operation failed - retry later");
    }

    [Fact]
    public void ErrorModification_CanBeChainedWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var final = result
            .Bind(x => x > 10 ? Result.Success(x) : Result.Failure<int>("Too small"))
            .PrependError("Validation error: ")
            .AppendError(" (minimum value is 10)")
            .Recover(0);

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(0, value));
    }

    #endregion


    #region ExceptionWrappingExtensions

    [Fact]
    public void Wrap_WithException_CreatesExceptionalError()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Something went wrong");

        // Act (When)
        var error = exception.Wrap();

        // Assert (Then)
        Assert.IsType<ExceptionalError>(error);
        Assert.Same(exception, error.Exception);
        Assert.Equal("Something went wrong", error.Message);
    }

    [Fact]
    public void Wrap_WithMessageOverride_UsesOverriddenMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Original message");

        // Act (When)
        var error = exception.Wrap("Custom error message");

        // Assert (Then)
        Assert.IsType<ExceptionalError>(error);
        Assert.Same(exception, error.Exception);
        Assert.Equal("Custom error message", error.Message);
    }

    [Fact]
    public void Wrap_WithExtraMetadata_AttachesMetadata()
    {
        // Arrange (Given)
        var exception = new ArgumentException("Bad argument");
        var extra = new Dictionary<string, object?> { ["UserId"] = 123, ["Action"] = "Login" };

        // Act (When)
        var error = exception.Wrap(extra: extra);

        // Assert (Then)
        Assert.IsType<ExceptionalError>(error);
        Assert.Equal(123, error.Extra!["UserId"]);
        Assert.Equal("Login", error.Extra!["Action"]);
    }

    [Fact]
    public void AsError_WithException_CreatesExceptionalError()
    {
        // Arrange (Given)
        var exception = new ArgumentNullException("param", "Parameter cannot be null");

        // Act (When)
        var error = exception.AsError();

        // Assert (Then)
        Assert.IsType<ExceptionalError>(error);
        Assert.Same(exception, error.Exception);
        Assert.Contains("Parameter cannot be null", error.Message);
    }

    [Fact]
    public void AsError_WithMessageOverride_UsesOverriddenMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Original");

        // Act (When)
        var error = exception.AsError("Overridden");

        // Assert (Then)
        Assert.Equal("Overridden", error.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithContext_PrependsContextToMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Failed to process");

        // Act (When)
        var error = exception.WrapAndPrepend("Database error: ");

        // Assert (Then)
        Assert.IsType<ExceptionalError>(error);
        Assert.Equal("Database error: Failed to process", error.Message);
        Assert.Same(exception, error.Exception);
    }

    [Fact]
    public void WrapAndPrepend_WithContextAndMessageOverride_UsesOverriddenMessage()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Original message");

        // Act (When)
        var error = exception.WrapAndPrepend("Error: ", "Custom message");

        // Assert (Then)
        Assert.Equal("Error: Custom message", error.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithEmptyContext_BehavesLikeWrap()
    {
        // Arrange (Given)
        var exception = new ArgumentException("Bad arg");

        // Act (When)
        var error = exception.WrapAndPrepend("");

        // Assert (Then)
        Assert.Equal("Bad arg", error.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithNullContext_BehavesLikeWrap()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Test");

        // Act (When)
        var error = exception.WrapAndPrepend(null!);

        // Assert (Then)
        Assert.Equal("Test", error.Message);
    }

    [Fact]
    public void WrapAndPrepend_WithExtraMetadata_AttachesMetadata()
    {
        // Arrange (Given)
        var exception = new Exception("Error");
        var extra = new Dictionary<string, object?> { ["RequestId"] = "req-123" };

        // Act (When)
        var error = exception.WrapAndPrepend("API Error: ", extra: extra);

        // Assert (Then)
        Assert.Equal("req-123", error.Extra!["RequestId"]);
    }

    [Fact]
    public void ExceptionWrapping_CanBeUsedInResultCreation()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Service unavailable");

        // Act (When)
        var result = Result.Failure(exception.Wrap());

        // Assert (Then)
        result.ShouldBe().Failure();
        Assert.True(result.HasException<InvalidOperationException>());
    }

    [Fact]
    public void ExceptionWrapping_CanBeUsedWithResultBind()
    {
        // Arrange (Given)
        var exception = new ArgumentException("Invalid input");
        var result = Result.Success(10);

        // Act (When)
        var final = result.Bind(x => Result.Failure<int>(exception.AsError("Validation failed")));

        // Assert (Then)
        final.ShouldBe().Failure().AndMessage("Validation failed");
    }

    #endregion
}
