using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result ToResult extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region ToResult - Success Cases

    [Fact]
    public void ToResult_WithSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe().Success();
    }

    [Fact]
    public void ToResult_WithSuccessResult_DiscardsValue()
    {
        // Arrange (Given)
        var result = Result.Success("some value");

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.IsType<Result>(converted);
    }

    [Fact]
    public void ToResult_WithSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(10).WithMetadata("key", "value");

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region ToResult - Failure Cases

    [Fact]
    public void ToResult_WithFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndMessage("Test error");
    }

    [Fact]
    public void ToResult_WithFailureResult_PreservesErrorCode()
    {
        // Arrange (Given)
        var error = new Error("TEST_CODE", "Test error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndCode("TEST_CODE");
    }

    [Fact]
    public void ToResult_WithFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error).WithMetadata("key", "value");

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe().Failure();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region ToResult - Edge Cases

    [Fact]
    public void ToResult_WithComplexType_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(new { Name = "John", Age = 30 });

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe().Success();
    }

    [Fact]
    public void ToResult_CanChainWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(42)
            .Map(x => x * 2);

        // Act (When)
        var converted = result.ToResult();

        // Assert (Then)
        converted.ShouldBe().Success();
    }

    #endregion
}
