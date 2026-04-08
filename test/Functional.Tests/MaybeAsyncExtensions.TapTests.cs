using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsTapTests {
    [Fact]
    public async Task TaskMaybe_TapSome_WithSyncAction_ExecutesAction() {
        // Arrange (Given)
        var maybe     = ValueTask.FromResult(Maybe.Some(42));
        var seenValue = 0;

        // Act (When)
        var result = await maybe.TapSome(x => seenValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, seenValue);
    }

    [Fact]
    public async Task TaskMaybe_TapSome_WithAsyncAction_ExecutesAction() {
        // Arrange (Given)
        var maybe     = ValueTask.FromResult(Maybe.Some(42));
        var seenValue = 0;

        // Act (When)
        var result = await maybe.TapSome(async x => {
            await Task.Delay(1);
            seenValue = x;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, seenValue);
    }

    [Fact]
    public async Task TaskMaybe_TapNone_WithSyncAction_ExecutesAction() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(() => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.True(actionExecuted);
    }

    [Fact]
    public async Task TaskMaybe_TapNone_WithAsyncAction_ExecutesAction() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(async () => {
            await Task.Delay(1);
            actionExecuted = true;
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.True(actionExecuted);
    }

    [Fact]
    public async Task ValueTaskMaybe_TapSome_WithNone_DoesNotExecuteAction() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapSome(x => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.False(actionExecuted);
    }

    [Fact]
    public async Task ValueTaskMaybe_TapNone_WithSome_DoesNotExecuteAction() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.Some(42));
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(() => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionExecuted);
    }

    [Fact]
    public async Task ValueTaskMaybe_TapSome_WithAsyncAction_OnNone_DoesNotExecuteAction() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapSome(async _ => {
            await Task.Delay(1);
            actionExecuted = true;
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.False(actionExecuted);
    }

    [Fact]
    public async Task ValueTaskMaybe_TapNone_WithAsyncAction_OnSome_DoesNotExecuteAction() {
        // Arrange (Given)
        var maybe          = ValueTask.FromResult(Maybe.Some(42));
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(async () => {
            await Task.Delay(1);
            actionExecuted = true;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionExecuted);
    }

    [Fact]
    public void Maybe_TapSome_WithSome_ExecutesAction() {
        // Arrange (Given)
        var maybe     = Maybe.Some(42);
        var seenValue = 0;

        // Act (When)
        var result = maybe.TapSome(x => seenValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, seenValue);
    }

    [Fact]
    public void Maybe_TapSome_WithNone_DoesNotExecuteAction() {
        // Arrange (Given)
        var maybe          = Maybe.None<int>();
        var actionExecuted = false;

        // Act (When)
        var result = maybe.TapSome(_ => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.False(actionExecuted);
    }

    [Fact]
    public void Maybe_TapNone_WithNone_ExecutesAction() {
        // Arrange (Given)
        var maybe          = Maybe.None<int>();
        var actionExecuted = false;

        // Act (When)
        var result = maybe.TapNone(() => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .None();
        Assert.True(actionExecuted);
    }

    [Fact]
    public void Maybe_TapNone_WithSome_DoesNotExecuteAction() {
        // Arrange (Given)
        var maybe          = Maybe.Some(42);
        var actionExecuted = false;

        // Act (When)
        var result = maybe.TapNone(() => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionExecuted);
    }
}
