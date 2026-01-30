using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsMapTests
{
    [Fact]
    public void Map_WithSome_ReturnsMappedSome()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(5);

        // Act (When)
        var mapped = maybe.Map(value => value * 2);

        // Assert (Then)
        mapped.ShouldBe()
            .Some()
            .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public void Map_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var mapped = maybe.Map(value => value * 2);

        // Assert (Then)
        mapped.ShouldBe()
            .None();
    }
}
