using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result As extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region As - Success Cases

    [Fact]
    public void As_WithSuccessResult_ReplacesValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var converted = result.As("new value");

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal("new value", value));
    }

    [Fact]
    public void As_WithSuccessResult_ChangesType()
    {
        // Arrange (Given)
        var result = Result.Success("original");

        // Act (When)
        var converted = result.As(123);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(123, value));
    }

    [Fact]
    public void As_WithSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(10).WithMetadata("key", "value");

        // Act (When)
        var converted = result.As("new value");

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    [Fact]
    public void As_WithSuccessResult_CanUseComplexType()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var newValue = new { Name = "John", Age = 30 };

        // Act (When)
        var converted = result.As(newValue);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal("John", value.Name);
                Assert.Equal(30, value.Age);
            });
    }

    #endregion

    #region As - Failure Cases

    [Fact]
    public void As_WithFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var converted = result.As("new value");

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndMessage("Test error");
    }

    [Fact]
    public void As_WithFailureResult_DoesNotUseNewValue()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var converted = result.As("should not be used");

        // Assert (Then)
        converted.ShouldBe().Failure();
    }

    [Fact]
    public void As_WithFailureResult_PreservesErrorCode()
    {
        // Arrange (Given)
        var error = new Error("TEST_CODE", "Test error");
        var result = Result.Failure<string>(error);

        // Act (When)
        var converted = result.As(123);

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndCode("TEST_CODE");
    }

    [Fact]
    public void As_WithFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error).WithMetadata("key", "value");

        // Act (When)
        var converted = result.As("new value");

        // Assert (Then)
        converted.ShouldBe().Failure();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region As - Edge Cases

    [Fact]
    public void As_CanChainMultipleTimes()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var converted = result
            .As("first")
            .As(100)
            .As(true);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.True(value));
    }

    [Fact]
    public void As_CanBeUsedAfterMap()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var converted = result
            .Map(x => x * 2) // 10
            .As("replaced");

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal("replaced", value));
    }

    [Fact]
    public void As_WithSameType_ReplacesValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var converted = result.As(99);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(99, value));
    }

    #endregion
}
