using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsOrElseTests {
    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithSome_ReturnsOriginal() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithNone_ReturnsFallback() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithFactory_ComputesFallback() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(() => Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithAsyncFactory_ComputesAsyncFallback() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(async () => {
            await Task.Delay(1);
            return Maybe.Some(99);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithSyncValue_ReturnsOriginal() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithSyncFactory_ReturnsAlternativeOnNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(() => Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithAsyncFactory_ReturnsAlternativeOnNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(async () => {
            await Task.Delay(1);
            return Maybe.Some(99);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithSyncValue_ReturnsOriginal() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }
}
