using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Maybe LINQ query operators.
/// </summary>
public sealed class MaybeExtensionsLinqTests
{
    [Fact]
    public void Linq_QuerySyntax_WithSomeValues_ReturnsProjectedSome()
    {
        // Arrange (Given)
        var left  = Maybe.Some(10);
        var right = Maybe.Some(2);

        // Act (When)
        var maybe =
            from l in left
            from r in right
            where l > r
            select l + r;

        // Assert (Then)
        maybe.ShouldBe()
             .Some()
             .And(v => Assert.Equal(12, v));
    }

    [Fact]
    public void Linq_QuerySyntax_WithNoneSource_ReturnsNone()
    {
        // Arrange (Given)
        var left  = Maybe.None<int>();
        var right = Maybe.Some(2);

        // Act (When)
        var maybe =
            from l in left
            from r in right
            select l + r;

        // Assert (Then)
        maybe.ShouldBe()
             .None();
    }

    [Fact]
    public void Linq_QuerySyntax_WithFalseWhere_ReturnsNone()
    {
        // Arrange (Given)
        var left  = Maybe.Some(1);
        var right = Maybe.Some(2);

        // Act (When)
        var maybe =
            from l in left
            from r in right
            where l > r
            select l + r;

        // Assert (Then)
        maybe.ShouldBe()
             .None();
    }

    [Fact]
    public void Linq_QuerySyntax_WithLetClause_UsesProjector()
    {
        // Arrange (Given)
        var value = Maybe.Some(10);

        // Act (When)
        var maybe =
            from v in value
            let doubled = v * 2
            let tripled = v * 3
            select doubled + tripled;

        // Assert (Then)
        maybe.ShouldBe()
             .Some()
             .And(result => Assert.Equal(50, result)); // (10*2) + (10*3) = 20 + 30 = 50
    }

    [Fact]
    public void Linq_QuerySyntax_WithLetClause_NoneReturnsNone()
    {
        // Arrange (Given)
        var value = Maybe.None<int>();

        // Act (When)
        var maybe =
            from v in value
            let doubled = v * 2
            select doubled;

        // Assert (Then)
        maybe.ShouldBe()
             .None();
    }
}
