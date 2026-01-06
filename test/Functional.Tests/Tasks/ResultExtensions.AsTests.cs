using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for async As extension methods on Result using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    #region AsAsync - Success Cases

    [Fact]
    public async Task AsAsync_WithSuccessResult_ReplacesValue()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(42));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal("new value", value));
    }

    [Fact]
    public async Task AsAsync_WithSuccessResult_ChangesType()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success("original"));

        // Act (When)
        var converted = await result.AsAsync(123);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(123, value));
    }

    [Fact]
    public async Task AsAsync_WithSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(10).WithMetadata("key", "value"));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    [Fact]
    public async Task AsAsync_WithSuccessResult_CanUseComplexType()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(42));
        var newValue = new { Name = "John", Age = 30 };

        // Act (When)
        var converted = await result.AsAsync(newValue);

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

    #region AsAsync - Failure Cases

    [Fact]
    public async Task AsAsync_WithFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndMessage("Test error");
    }

    [Fact]
    public async Task AsAsync_WithFailureResult_DoesNotUseNewValue()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var converted = await result.AsAsync("should not be used");

        // Assert (Then)
        converted.ShouldBe().Failure();
    }

    [Fact]
    public async Task AsAsync_WithFailureResult_PreservesErrorCode()
    {
        // Arrange (Given)
        var error = new Error("TEST_CODE", "Test error");
        var result = Task.FromResult(Result.Failure<string>(error));

        // Act (When)
        var converted = await result.AsAsync(123);

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndCode("TEST_CODE");
    }

    [Fact]
    public async Task AsAsync_WithFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Task.FromResult(Result.Failure<int>(error).WithMetadata("key", "value"));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe().Failure();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region AsAsync - Edge Cases

    [Fact]
    public async Task AsAsync_CanChainMultipleTimes()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(42));

        // Act (When)
        var converted = await result
            .AsAsync("first")
            .AsAsync(100)
            .AsAsync(true);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.True(value));
    }

    [Fact]
    public async Task AsAsync_CanBeUsedAfterMap()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(5));

        // Act (When)
        var converted = await result
            .MapAsync(x => x * 2) // 10
            .AsAsync("replaced");

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal("replaced", value));
    }

    [Fact]
    public async Task AsAsync_WithSameType_ReplacesValue()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(42));

        // Act (When)
        var converted = await result.AsAsync(99);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(99, value));
    }

    #endregion
}
