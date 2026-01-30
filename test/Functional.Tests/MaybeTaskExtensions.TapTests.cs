using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class MaybeTaskExtensionsTests
{
    [Fact]
    public async Task Tap_WithSome_ExecutesAction()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(42)));
        var executed = false;
        var captured = 0;

        // Act (When)
        var result = await maybeTask.Tap(value =>
        {
            executed = true;
            captured = value;
        });

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal(42, v));
        Assert.True(executed);
        Assert.Equal(42, captured);
    }

    [Fact]
    public async Task Tap_WithNone_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.None<int>()));
        var executed = false;

        // Act (When)
        var result = await maybeTask.Tap(_ => executed = true);

        // Assert (Then)
        result.ShouldBe()
            .None();
        Assert.False(executed);
    }

    [Fact]
    public async Task Tap_WithSome_ExecutesAsyncAction()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(42)));
        var executed = false;

        // Act (When)
        var result = await maybeTask.Tap(value =>
        {
            executed = true;
            Assert.Equal(42, value);
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal(42, v));
        Assert.True(executed);
    }
}
