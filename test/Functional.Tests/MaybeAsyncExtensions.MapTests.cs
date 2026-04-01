using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsMapTests {
    [Fact]
    public async Task ValueTaskMaybe_Map_WithAsyncMapper_TransformsValue() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(5));

        // Act (When)
        var result = await maybe.Map(async x => {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_Map_OnNone_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Map(async x => {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
    }

    [Fact]
    public async Task ValueTaskMaybe_Map_WithSyncMapper_OnNone_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .None();
    }

    [Fact]
    public async Task TaskMaybe_Map_WithSyncMapper_TransformsValue() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(10));

        // Act (When)
        var result = await maybe.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task TaskMaybe_Map_WithNone_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .None();
    }
}
