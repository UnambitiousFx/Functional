using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsOrElseTests
{
    [Fact]
    public void OrElse_WithSome_ReturnsOriginal()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(7);
        var fallback = Maybe.Some(42);

        // Act (When)
        var result = maybe.OrElse(fallback);

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal(7, v));
    }

    [Fact]
    public void OrElse_WithNone_ReturnsFallback()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var fallback = Maybe.Some(42);

        // Act (When)
        var result = maybe.OrElse(fallback);

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void OrElse_WithFactory_WithSome_DoesNotExecuteFactory()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(7);
        var executed = false;

        // Act (When)
        var result = maybe.OrElse(() =>
        {
            executed = true;
            return Maybe.Some(42);
        });

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal(7, v));
        Assert.False(executed);
    }

    [Fact]
    public void OrElse_WithFactory_WithNone_ExecutesFactory()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        var result = maybe.OrElse(() =>
        {
            executed = true;
            return Maybe.Some(42);
        });

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal(42, v));
        Assert.True(executed);
    }
}
