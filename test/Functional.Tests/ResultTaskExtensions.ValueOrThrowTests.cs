namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task ValueOrThrow_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var value = await awaitableResult.ValueOrThrow();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrThrow_WithAwaitableFailure_ThrowsException()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act & Assert (When/Then)
        await Assert.ThrowsAsync<Exception>(async () => await awaitableResult.ValueOrThrow());
    }

    [Fact]
    public async Task ValueOrThrow_WithFactory_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var value = await awaitableResult.ValueOrThrow(error => new ArgumentException(error.Message));

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrThrow_WithFactory_WithAwaitableFailure_ThrowsCustomException()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act & Assert (When/Then)
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await awaitableResult.ValueOrThrow(error => new ArgumentException(error.Message)));
        Assert.Equal("Error", exception.Message);
    }
}