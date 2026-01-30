namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsValueOrTests
{
    [Fact]
    public void ValueOr_WithSome_ReturnsValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(9);

        // Act (When)
        var value = maybe.ValueOr(42);

        // Assert (Then)
        Assert.Equal(9, value);
    }

    [Fact]
    public void ValueOr_WithNone_ReturnsFallback()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var value = maybe.ValueOr(42);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void ValueOr_WithFactory_WithSome_DoesNotExecuteFactory()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(9);
        var executed = false;

        // Act (When)
        var value = maybe.ValueOr(() =>
        {
            executed = true;
            return 42;
        });

        // Assert (Then)
        Assert.Equal(9, value);
        Assert.False(executed);
    }

    [Fact]
    public void ValueOr_WithFactory_WithNone_ExecutesFactory()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        var value = maybe.ValueOr(() =>
        {
            executed = true;
            return 42;
        });

        // Assert (Then)
        Assert.Equal(42, value);
        Assert.True(executed);
    }
}
