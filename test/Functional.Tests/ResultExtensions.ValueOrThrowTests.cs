using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Try and ValueOrThrow extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region ValueOrThrow - Default

    [Fact]
    public void ValueOrThrow_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var value = result.ValueOrThrow();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void ValueOrThrow_WithFailure_ThrowsException()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act & Assert
        var exception = Assert.Throws<FunctionalException>(() => result.ValueOrThrow());
        Assert.Contains("Test error", exception.Message);
    }

    [Fact]
    public void ValueOrThrow_WithStringType_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var value = result.ValueOrThrow();

        // Assert (Then)
        Assert.Equal("hello", value);
    }

    #endregion

    #region ValueOrThrow - Custom Exception

    [Fact]
    public void ValueOrThrow_WithCustomFactory_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var value = result.ValueOrThrow(err => new InvalidOperationException(err.Message));

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void ValueOrThrow_WithCustomFactory_ThrowsCustomException()
    {
        // Arrange (Given)
        var error = new Error("Custom error");
        var result = Result.Failure<int>(error);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            result.ValueOrThrow(err => new ArgumentException(err.Message)));

        Assert.Equal("Custom error", exception.Message);
    }

    [Fact]
    public void ValueOrThrow_CanThrowSpecificExceptionType()
    {
        // Arrange (Given)
        var error = new Error("Not found");
        var result = Result.Failure<string>(error);

        // Act & Assert
        var exception = Assert.Throws<KeyNotFoundException>(() =>
            result.ValueOrThrow(err => new KeyNotFoundException($"Item not found: {err.Message}")));

        Assert.Contains("Item not found", exception.Message);
    }

    [Fact]
    public void ValueOrThrow_CustomFactory_ReceivesError()
    {
        // Arrange (Given)
        var error = new Error("ERR_404", "Resource not found");
        var result = Result.Failure<int>(error);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() =>
            result.ValueOrThrow(err => new Exception($"[{err.Code}] {err.Message}")));

        Assert.Equal("[ERR_404] Resource not found", exception.Message);
    }

    #endregion

    #region Try and ValueOrThrow Combined

    [Fact]
    public void Try_ThenValueOrThrow_WorksTogether()
    {
        // Arrange (Given)
        var result = Result.Success("42");

        // Act (When)
        var value = result
            .Try(x => int.Parse(x))
            .Try(x => x * 2)
            .ValueOrThrow();

        // Assert (Then)
        Assert.Equal(84, value);
    }

    [Fact]
    public void Try_WithException_ThenValueOrThrow_PropagatesAsCustomException()
    {
        // Arrange (Given)
        var result = Result.Success("not a number");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            result
                .Try(x => int.Parse(x))
                .ValueOrThrow(err => new InvalidOperationException($"Parse failed: {err.Message}")));

        Assert.Contains("Parse failed", exception.Message);
    }

    #endregion
}
