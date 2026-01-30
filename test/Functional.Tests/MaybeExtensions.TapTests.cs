using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeExtensionsTapTests
{
    [Fact]
    public void Tap_WithSome_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var executed = false;
        var captured = 0;

        // Act (When)
        var tapped = maybe.Tap(value =>
        {
            executed = true;
            captured = value;
        });

        // Assert (Then)
        tapped.ShouldBe()
            .Some()
            .And(v => Assert.Equal(42, v));
        Assert.True(executed);
        Assert.Equal(42, captured);
    }

    [Fact]
    public void Tap_WithNone_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        var tapped = maybe.Tap(_ => executed = true);

        // Assert (Then)
        tapped.ShouldBe()
            .None();
        Assert.False(executed);
    }
}
