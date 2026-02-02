namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeDebugViewTests
{
    [Fact]
    public void Constructor_WithSome_InitializesCorrectly()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var debugView = new MaybeDebugView<int>(maybe);

        // Assert (Then)
        Assert.NotNull(debugView);
    }

    [Fact]
    public void Constructor_WithNone_InitializesCorrectly()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.None();

        // Act (When)
        var debugView = new MaybeDebugView<int>(maybe);

        // Assert (Then)
        Assert.NotNull(debugView);
    }

    [Fact]
    public void IsSome_WithSome_ReturnsTrue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var isSome = debugView.IsSome;

        // Assert (Then)
        Assert.True(isSome);
    }

    [Fact]
    public void IsSome_WithNone_ReturnsFalse()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.None();
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var isSome = debugView.IsSome;

        // Assert (Then)
        Assert.False(isSome);
    }

    [Fact]
    public void IsNone_WithNone_ReturnsTrue()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.None();
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var isNone = debugView.IsNone;

        // Assert (Then)
        Assert.True(isNone);
    }

    [Fact]
    public void IsNone_WithSome_ReturnsFalse()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var isNone = debugView.IsNone;

        // Assert (Then)
        Assert.False(isNone);
    }

    [Fact]
    public void HasValue_WithSome_ReturnsTrue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var hasValue = debugView.HasValue;

        // Assert (Then)
        Assert.True(hasValue);
    }

    [Fact]
    public void HasValue_WithNone_ReturnsFalse()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.None();
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var hasValue = debugView.HasValue;

        // Assert (Then)
        Assert.False(hasValue);
    }

    [Fact]
    public void Value_WithSome_ReturnsValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var value = debugView.Value;

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void Value_WithNone_ReturnsDefault()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.None();
        var debugView = new MaybeDebugView<int>(maybe);

        // Act (When)
        var value = debugView.Value;

        // Assert (Then)
        Assert.Equal(default, value);
    }

    [Fact]
    public void Value_WithReferenceType_ReturnsCorrectValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some("test");
        var debugView = new MaybeDebugView<string>(maybe);

        // Act (When)
        var value = debugView.Value;

        // Assert (Then)
        Assert.Equal("test", value);
    }

    [Fact]
    public void Value_WithNoneReferenceType_ReturnsNull()
    {
        // Arrange (Given)
        var maybe = Maybe<string>.None();
        var debugView = new MaybeDebugView<string>(maybe);

        // Act (When)
        var value = debugView.Value;

        // Assert (Then)
        Assert.Null(value);
    }
}
