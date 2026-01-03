using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Map extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Map - Success Cases

    [Fact]
    public void Map_WithSuccessResult_TransformsValue()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var mapped = result.Map(x => x * 2);

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Map_WithSuccessResult_ChangesType()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var mapped = result.Map(x => x.ToString());

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("42", value));
    }

    [Fact]
    public void Map_WithSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(10).WithMetadata("key", "value");

        // Act (When)
        var mapped = result.Map(x => x * 3);

        // Assert (Then)
        mapped.ShouldBe().Success();
        Assert.Equal("value", mapped.Metadata["key"]);
    }

    #endregion

    #region Map - Failure Cases

    [Fact]
    public void Map_WithFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var mapped = result.Map(x => x * 2);

        // Assert (Then)
        mapped.ShouldBe()
            .Failure()
            .AndMessage("Test error");
    }

    [Fact]
    public void Map_WithFailureResult_DoesNotExecuteMapper()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);
        var executed = false;

        // Act (When)
        var mapped = result.Map(x =>
        {
            executed = true;
            return x * 2;
        });

        // Assert (Then)
        Assert.False(executed);
        mapped.ShouldBe().Failure();
    }

    [Fact]
    public void Map_WithFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error).WithMetadata("key", "value");

        // Act (When)
        var mapped = result.Map(x => x.ToString());

        // Assert (Then)
        mapped.ShouldBe().Failure();
        Assert.Equal("value", mapped.Metadata["key"]);
    }

    #endregion

    #region Map - Edge Cases

    [Fact]
    public void Map_CanChainMultipleMaps()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var mapped = result
            .Map(x => x * 2) // 10
            .Map(x => x + 5) // 15
            .Map(x => x.ToString()); // "15"

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("15", value));
    }

    [Fact]
    public void Map_WithComplexObject_TransformsCorrectly()
    {
        // Arrange (Given)
        var result = Result.Success(new { Name = "John", Age = 30 });

        // Act (When)
        var mapped = result.Map(x => $"{x.Name} is {x.Age} years old");

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("John is 30 years old", value));
    }

    #endregion
}
