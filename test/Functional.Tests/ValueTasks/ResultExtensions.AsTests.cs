using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for async As extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    #region AsAsync - Success Cases

    [Fact]
    public async Task AsAsync_WithSuccessResult_ReplacesValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

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
        var result = ValueTask.FromResult(Result.Success("original"));

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
        var result = ValueTask.FromResult(Result.Success(10).WithMetadata("key", "value"));

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
        var result = ValueTask.FromResult(Result.Success(42));
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
        var result = ValueTask.FromResult(Result.Failure<int>(error));

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
        var result = ValueTask.FromResult(Result.Failure<int>(error));

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
        var result = ValueTask.FromResult(Result.Failure<string>(error));

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
        var result = ValueTask.FromResult(Result.Failure<int>(error).WithMetadata("key", "value"));

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
        var result = ValueTask.FromResult(Result.Success(42));

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
        var result = ValueTask.FromResult(Result.Success(5));

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
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var converted = await result.AsAsync(99);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(99, value));
    }

    #endregion

    #region AsAsync (Non-Generic Result) - Success Cases

    [Fact]
    public async Task AsAsync_NonGenericResult_WithSuccess_ReplacesWithValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal("new value", value));
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_WithSuccess_CanUseAnyType()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var converted = await result.AsAsync(42);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_WithSuccess_PreservesMetadata()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success().WithMetadata("key", "value"));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_WithSuccess_CanUseComplexType()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());
        var newValue = new { Name = "Jane", Age = 25 };

        // Act (When)
        var converted = await result.AsAsync(newValue);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal("Jane", value.Name);
                Assert.Equal(25, value.Age);
            });
    }

    #endregion

    #region AsAsync (Non-Generic Result) - Failure Cases

    [Fact]
    public async Task AsAsync_NonGenericResult_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndMessage("Test error");
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_WithFailure_DoesNotUseNewValue()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var converted = await result.AsAsync(42);

        // Assert (Then)
        converted.ShouldBe().Failure();
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_WithFailure_PreservesErrorCode()
    {
        // Arrange (Given)
        var error = new Error("TEST_CODE", "Test error");
        var result = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var converted = await result.AsAsync(123);

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndCode("TEST_CODE");
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_WithFailure_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = ValueTask.FromResult(Result.Failure(error).WithMetadata("key", "value"));

        // Act (When)
        var converted = await result.AsAsync("new value");

        // Assert (Then)
        converted.ShouldBe().Failure();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region AsAsync (Non-Generic Result) - Edge Cases

    [Fact]
    public async Task AsAsync_NonGenericResult_CanChainMultipleTimes()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

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
    public async Task AsAsync_NonGenericResult_CanBeUsedAfterBind()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var converted = await result
            .BindAsync(() => ValueTask.FromResult(Result.Success()))
            .AsAsync("replaced");

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal("replaced", value));
    }

    [Fact]
    public async Task AsAsync_NonGenericResult_CanConvertToGenericResult()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var converted = await result.AsAsync(42);

        // Assert (Then)
        converted.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    #endregion
}
