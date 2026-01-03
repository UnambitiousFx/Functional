using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result Flatten extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Flatten - Failure Cases

    [Fact]
    public void Flatten_WithFailure_PropagatesOuterError()
    {
        // Arrange (Given)
        var error = new Error("Outer error");
        var outerResult = Result.Failure<Result<int>>(error);

        // Act (When)
        var flattened = outerResult.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Failure().AndMessage("Outer error");
    }

    #endregion

    #region Flatten - Success Cases

    [Fact]
    public void Flatten_WithSuccessOfSuccess_ReturnsInnerSuccess()
    {
        // Arrange (Given)
        var innerResult = Result.Success(42);
        var outerResult = Result.Success(innerResult);

        // Act (When)
        var flattened = outerResult.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public void Flatten_WithSuccessOfFailure_ReturnsInnerFailure()
    {
        // Arrange (Given)
        var error = new Error("Inner error");
        var innerResult = Result.Failure<int>(error);
        var outerResult = Result.Success(innerResult);

        // Act (When)
        var flattened = outerResult.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Failure().AndMessage("Inner error");
    }

    #endregion

    #region Flatten - Use Cases

    [Fact]
    public void Flatten_WithNestedBindOperations_SimplifiesResult()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        // Bind returns Result<Result<int>>, Flatten simplifies to Result<int>
        var nested = result.Bind(x => Result.Success(Result.Success(x * 2)));
        var flattened = nested.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void Flatten_CanChainWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(Result.Success(10));

        // Act (When)
        var final = result
            .Flatten()
            .Map(x => x * 2)
            .Bind(x => Result.Success(x + 5));

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(25, value));
    }

    #endregion

    #region Flatten - Type Scenarios

    [Fact]
    public void Flatten_WithStringType_WorksCorrectly()
    {
        // Arrange (Given)
        var innerResult = Result.Success("hello");
        var outerResult = Result.Success(innerResult);

        // Act (When)
        var flattened = outerResult.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Success().And(value => Assert.Equal("hello", value));
    }

    [Fact]
    public void Flatten_WithComplexType_WorksCorrectly()
    {
        // Arrange (Given)
        var person = new { Name = "John", Age = 30 };
        var innerResult = Result.Success(person);
        var outerResult = Result.Success(innerResult);

        // Act (When)
        var flattened = outerResult.Flatten();

        // Assert (Then)
        flattened.ShouldBe().Success();
        Assert.True(flattened.TryGet(out var value, out _));
        Assert.Equal("John", value.Name);
        Assert.Equal(30, value.Age);
    }

    #endregion
}
