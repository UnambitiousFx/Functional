using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsValueOrTests {
    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithSome_ReturnsValue() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithNone_ReturnsFallback() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithFactory_ComputesFallback() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithAsyncFactory_ComputesAsyncFallback() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(async () => {
            await Task.Delay(1);
            return 99;
        });

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithSyncValue_ReturnsSome() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithSyncValue_ReturnsAlternativeOnNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithSyncFactory_ReturnsAlternativeOnNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithAsyncFactory_ReturnsAlternativeOnNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(async () => {
            await Task.Delay(1);
            return 99;
        });

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithSyncValue_ReturnsSome() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(42, result);
    }
}
