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
}
