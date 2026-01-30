namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task ValueOr_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var value = await awaitableResult.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_WithAwaitableFailure_ReturnsDefaultValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var value = await awaitableResult.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_WithFactory_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var value = await awaitableResult.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_WithFactory_WithAwaitableFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var value = await awaitableResult.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, value);
    }
}