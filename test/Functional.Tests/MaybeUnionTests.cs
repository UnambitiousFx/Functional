#if NET11_0_OR_GREATER
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeUnionTests {
    [Fact]
    public void Switch_WithSomeReferenceType_MatchesValueArm() {
        // Arrange (Given)
        var maybe = Maybe.Some("hello");

        // Act (When) — exhaustive switch, no discard arm: compiling is the union recognition proof
        var output = maybe switch {
            string value => value,
            null         => "none",
        };

        // Assert (Then)
        Assert.Equal("hello", output);
    }

    [Fact]
    public void Switch_WithSomeStructValue_MatchesValueArm() {
        // Arrange (Given)
        var maybe = Maybe.Some(21);

        // Act (When) — struct case goes through TryGetValue (non-boxing path)
        var output = maybe switch {
            int value => value * 2,
            null      => 0,
        };

        // Assert (Then)
        Assert.Equal(42, output);
    }

    [Fact]
    public void Switch_WithNone_MatchesNullArm() {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var output = maybe switch {
            int value => value.ToString(),
            null      => "none",
        };

        // Assert (Then)
        Assert.Equal("none", output);
    }

    [Fact]
    public void IsPattern_WithSome_UnwrapsValue() {
        // Arrange (Given)
        var maybe = Maybe.Some("payload");

        // Act (When)
        var matched = maybe is string value
                          ? value
                          : null;

        // Assert (Then)
        Assert.Equal("payload", matched);
    }

    [Fact]
    public void Value_WithSomeReferenceType_ReturnsSameInstance() {
        // Arrange (Given)
        var payload = new object();
        var maybe   = Maybe.Some(payload);

        // Act (When)
        var value = maybe.Value;

        // Assert (Then)
        Assert.Same(payload, value);
    }

    [Fact]
    public void Value_WithNone_ReturnsNull() {
        // Arrange (Given)
        var maybe = Maybe.None<string>();

        // Act (When)
        var value = maybe.Value;

        // Assert (Then)
        Assert.Null(value);
    }

    [Fact]
    public void HasValue_WithDefaultMaybe_ReturnsFalse() {
        // Arrange (Given)
        var maybe = default(Maybe<int>);

        // Act (When)
        var hasValue = maybe.HasValue;
        var gotValue = maybe.TryGetValue(out _);

        // Assert (Then)
        Assert.False(hasValue);
        Assert.False(gotValue);
        Assert.Null(maybe.Value);
    }

    [Fact]
    public void TryGetValue_WithSome_ReturnsValue() {
        // Arrange (Given)
        var maybe = Maybe.Some(9);

        // Act (When)
        var got = maybe.TryGetValue(out var value);

        // Assert (Then)
        Assert.True(got);
        Assert.Equal(9, value);
    }

    [Fact]
    public void UnionCreate_WithValue_CreatesSome() {
        // Arrange (Given)
        const string value = "made";

        // Act (When)
        var maybe = Maybe<string>.IUnionMembers.Create(value);

        // Assert (Then)
        maybe.ShouldBe()
             .Some()
             .And(v => Assert.Equal("made", v));
    }

    [Fact]
    public void ImplicitConversion_OnNet11_StillWorks() {
        // Arrange (Given)
        Maybe<string> some = "text";
        Maybe<string> none = (string?)null;

        // Act (When) & Assert (Then)
        some.ShouldBe()
            .Some()
            .And(v => Assert.Equal("text", v));
        none.ShouldBe()
            .None();
    }

    [Fact]
    public void Match_OnNet11_StillWorks() {
        // Arrange (Given)
        var maybe = Maybe.Some(5);

        // Act (When)
        var output = maybe.Match(value => value + 1,
                                 () => -1);

        // Assert (Then)
        Assert.Equal(6, output);
    }
}
#endif
