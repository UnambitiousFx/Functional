using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

public sealed class MaybeExtensionsMatchTests
{
    [Fact]
    public async Task MatchAsync_WithSome_ReturnsSomeResult()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybeTask.MatchAsync(
            some: v => v * 2,
            none: () => 0);

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task MatchAsync_WithNone_ReturnsNoneResult()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybeTask.MatchAsync(
            some: v => v * 2,
            none: () => -1);

        // Assert (Then)
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task MatchAsync_WithAsyncHandlers_WithSome_ReturnsSomeResult()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybeTask.MatchAsync(
            some: v => ValueTask.FromResult(v * 2),
            none: () => ValueTask.FromResult(0));

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task MatchAsync_WithAsyncHandlers_WithNone_ReturnsNoneResult()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybeTask.MatchAsync(
            some: v => ValueTask.FromResult(v * 2),
            none: () => ValueTask.FromResult(-1));

        // Assert (Then)
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task MatchAsync_WithAsyncHandlers_WithSome_ExecutesAsyncOperation()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some("test"));
        var executed = false;

        // Act (When)
        var result = await maybeTask.MatchAsync(
            some: async v =>
            {
                await Task.Delay(1);
                executed = true;
                return v.ToUpper();
            },
            none: () => ValueTask.FromResult(string.Empty));

        // Assert (Then)
        Assert.Equal("TEST", result);
        Assert.True(executed);
    }

    [Fact]
    public async Task MatchAsync_WithAsyncHandlers_WithNone_ExecutesAsyncOperation()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.None<string>());
        var executed = false;

        // Act (When)
        var result = await maybeTask.MatchAsync(
            some: v => ValueTask.FromResult(v.ToUpper()),
            none: async () =>
            {
                await Task.Delay(1);
                executed = true;
                return "default";
            });

        // Assert (Then)
        Assert.Equal("default", result);
        Assert.True(executed);
    }
}
