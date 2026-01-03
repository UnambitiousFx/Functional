using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsBindTests
{
    [Fact]
    public void Bind_WithSomeAndReturningSome_ReturnsSome()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var result = maybe.Bind(v => Maybe.Some(v.ToString()));

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal("42", v));
    }

    [Fact]
    public void Bind_WithSomeAndReturningNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var result = maybe.Bind(_ => Maybe.None<string>());

        // Assert (Then)
        result.ShouldBe()
            .None();
    }

    [Fact]
    public void Bind_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        var result = maybe.Bind(v =>
        {
            executed = true;
            return Maybe.Some(v.ToString());
        });

        // Assert (Then)
        result.ShouldBe()
            .None();
        Assert.False(executed);
    }
}
