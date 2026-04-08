using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsBindTests {
    [Fact]
    public async Task ValueTaskMaybe_Pipeline_CanBindMapTapSomeTapNoneWithoutWrapper() {
        // Arrange (Given)
        var start   = ValueTask.FromResult(Maybe.Some(2));
        var seen    = 0;
        var noneHit = false;

        // Act (When)
        var maybe = await start
                         .Bind(v => ValueTask.FromResult(Maybe.Some(v + 1)))
                         .Map(v => v * 10)
                         .TapSome(v => seen = v)
                         .TapNone(() => noneHit = true);

        // Assert (Then)
        maybe.ShouldBe()
             .Some()
             .And(v => Assert.Equal(30, v));
        Assert.Equal(30, seen);
        Assert.False(noneHit);
    }

    [Fact]
    public async Task ValueTaskMaybe_Bind_WithNone_ReturnsNone() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.None<int>());
        var functionCalled = false;

        // Act (When)
        var result = await maybe.Bind(x => {
            functionCalled = true;
            return ValueTask.FromResult(Maybe.Some(x * 2));
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.False(functionCalled);
    }

    [Fact]
    public async Task TaskMaybe_Bind_WithAsyncSelector_ChainsCorrectly() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(10));

        // Act (When)
        var result = await maybe.Bind(async x => {
            await Task.Delay(1);
            return Maybe.Some(x * 2);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task TaskMaybe_Bind_WithNone_ReturnsNone() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.None<int>());
        var functionCalled = false;

        // Act (When)
        var result = await maybe.Bind(x => {
            functionCalled = true;
            return Maybe.Some(x * 2);
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.False(functionCalled);
    }

    [Fact]
    public async Task TaskMaybe_Bind_WithSyncFunction_ReturningSome_ReturnsTransformedValue() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(10));

        // Act (When)
        var result = await maybe.Bind(x => Maybe.Some(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(20, v));
    }
}
