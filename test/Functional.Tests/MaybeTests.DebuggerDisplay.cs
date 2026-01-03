using System.Reflection;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeDebuggerDisplayTests
{
    [Fact]
    public void DebuggerDisplay_WithSome_ReturnsCorrectString()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var property =
            typeof(Maybe<int>).GetProperty("DebuggerDisplay", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act (When)
        var display = property?.GetValue(maybe) as string;

        // Assert (Then)
        Assert.Equal("Some(42)", display);
    }

    [Fact]
    public void DebuggerDisplay_WithNone_ReturnsCorrectString()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var property =
            typeof(Maybe<int>).GetProperty("DebuggerDisplay", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act (When)
        var display = property?.GetValue(maybe) as string;

        // Assert (Then)
        Assert.Equal("None", display);
    }
}
