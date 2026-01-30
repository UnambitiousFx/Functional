using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsFilterTests
{
    [Fact]
    public void Filter_WithSomeAndPredicateTrue_ReturnsSome()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(10);

        // Act (When)
        var filtered = maybe.Filter(value => value > 5);

        // Assert (Then)
        filtered.ShouldBe()
            .Some()
            .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public void Filter_WithSomeAndPredicateFalse_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(3);

        // Act (When)
        var filtered = maybe.Filter(value => value > 5);

        // Assert (Then)
        filtered.ShouldBe()
            .None();
    }

    [Fact]
    public void Filter_WithNone_DoesNotExecutePredicate()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        var filtered = maybe.Filter(_ =>
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
