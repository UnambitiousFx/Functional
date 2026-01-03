using UnambitiousFx.Functional.Tasks;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task ValueOrAsync_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        var value = await awaitableResult.ValueOrAsync(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrAsync_WithAwaitableFailure_ReturnsDefaultValue()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var value = await awaitableResult.ValueOrAsync(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOrAsync_WithFactory_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        var value = await awaitableResult.ValueOrAsync(() => 99);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrAsync_WithFactory_WithAwaitableFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var value = await awaitableResult.ValueOrAsync(() => 99);

        // Assert (Then)
        Assert.Equal(99, value);
    }
}
