using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task ValueOrThrowAsync_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await awaitableResult.ValueOrThrowAsync();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrThrowAsync_WithAwaitableFailure_ThrowsException()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act & Assert (When/Then)
        await Assert.ThrowsAsync<Exception>(async () => await awaitableResult.ValueOrThrowAsync());
    }

    [Fact]
    public async Task ValueOrThrowAsync_WithFactory_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await awaitableResult.ValueOrThrowAsync(error => new ArgumentException(error.Message));

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrThrowAsync_WithFactory_WithAwaitableFailure_ThrowsCustomException()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act & Assert (When/Then)
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await awaitableResult.ValueOrThrowAsync(error => new ArgumentException(error.Message)));
        Assert.Equal("Error", exception.Message);
    }
}
