using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void HasException_WithSuccess_ReturnsFalse()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var hasException = result.HasException<InvalidOperationException>();

        // Assert (Then)
        Assert.False(hasException);
    }

    [Fact]
    public void HasException_WithExceptionalError_ReturnsTrue()
    {
        // Arrange (Given)
        var exception = new InvalidOperationException("Invalid operation");
        var result = Result.Failure(exception);

        // Act (When)
        var hasException = result.HasException<InvalidOperationException>();

        // Assert (Then)
        Assert.True(hasException);
    }

    [Fact]
    public void HasException_WithDifferentExceptionType_ReturnsFalse()
    {
        // Arrange (Given)
        var exception = new ArgumentException("Bad argument");
        var result = Result.Failure(exception);

        // Act (When)
        var hasException = result.HasException<InvalidOperationException>();

        // Assert (Then)
        Assert.False(hasException);
    }

    [Fact]
    public void HasException_WithNonExceptionalError_ReturnsFalse()
    {
        // Arrange (Given)
        var error = new Error("Simple error");
        var result = Result.Failure(error);

        // Act (When)
        var hasException = result.HasException<Exception>();

        // Assert (Then)
        Assert.False(hasException);
    }

    [Fact]
    public void HasException_WithBaseExceptionType_ReturnsTrue()
    {
        // Arrange (Given)
        var exception = new ArgumentNullException("param");
        var result = Result.Failure(exception);

        // Act (When)
        var hasException = result.HasException<Exception>();

        // Assert (Then)
        Assert.True(hasException);
    }
}
