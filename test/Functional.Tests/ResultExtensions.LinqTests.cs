using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result LINQ query operators.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void Linq_QuerySyntax_WithSuccesses_ReturnsProjectedSuccess()
    {
        // Arrange (Given)
        var left = Result.Success(10);
        var right = Result.Success(2);

        // Act (When)
        var result =
            from l in left
            from r in right
            where l > r
            select l + r;

        // Assert (Then)
        result.ShouldBe().Success().And(v => Assert.Equal(12, v));
    }

    [Fact]
    public void Linq_QuerySyntax_WithFailedSource_PropagatesFailure()
    {
        // Arrange (Given)
        var left = Result.Failure<int>("left failed");
        var right = Result.Success(2);

        // Act (When)
        var result =
            from l in left
            from r in right
            select l + r;

        // Assert (Then)
        result.ShouldBe().Failure().AndMessage("left failed");
    }

    [Fact]
    public void Linq_QuerySyntax_WithFalseWhere_ReturnsValidationFailure()
    {
        // Arrange (Given)
        var left = Result.Success(1);
        var right = Result.Success(2);

        // Act (When)
        var result =
            from l in left
            from r in right
            where l > r
            select l + r;

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(ErrorCodes.Validation)
            .AndMessage("Result.Where predicate returned false.");
    }

    [Fact]
    public void Linq_QuerySyntax_PreservesMetadata()
    {
        // Arrange (Given)
        var left = Result.Success(10).WithMetadata("scope", "linq");
        var right = Result.Success(2);

        // Act (When)
        var result =
            from l in left
            from r in right
            select l + r;

        // Assert (Then)
        result.ShouldBe().Success().And(v => Assert.Equal(12, v));
        Assert.Equal("linq", result.Metadata["scope"]);
    }
}
