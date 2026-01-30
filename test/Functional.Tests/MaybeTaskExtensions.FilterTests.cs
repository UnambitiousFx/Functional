using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class MaybeTaskExtensionsTests
{
    [Fact]
    public async Task Filter_WithSomeAndPredicateTrue_ReturnsSome()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(10)));

        // Act (When)
        var filtered = await maybeTask.Filter(value => value > 5);

        // Assert (Then)
        filtered.ShouldBe()
            .Some()
            .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Filter_WithSomeAndPredicateFalse_ReturnsNone()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(3)));

        // Act (When)
        var filtered = await maybeTask.Filter(value => value > 5);

        // Assert (Then)
        filtered.ShouldBe()
            .None();
    }

    [Fact]
    public async Task Filter_WithNone_DoesNotExecutePredicate()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.None<int>()));
        var executed = false;

        // Act (When)
        var filtered = await maybeTask.Filter(_ =>
        {
            executed = true;
            return true;
        });

        // Assert (Then)
        filtered.ShouldBe()
            .None();
        Assert.False(executed);
    }
}
