using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for async ToResult extension methods on Result using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    #region ToResultAsync - Success Cases

    [Fact]
    public async Task ToResultAsync_WithSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(42));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe().Success();
    }

    [Fact]
    public async Task ToResultAsync_WithSuccessResult_DiscardsValue()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success("some value"));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.IsType<Result>(converted);
    }

    [Fact]
    public async Task ToResultAsync_WithSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(10).WithMetadata("key", "value"));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe().Success();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region ToResultAsync - Failure Cases

    [Fact]
    public async Task ToResultAsync_WithFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndMessage("Test error");
    }

    [Fact]
    public async Task ToResultAsync_WithFailureResult_PreservesErrorCode()
    {
        // Arrange (Given)
        var error = new Error("TEST_CODE", "Test error");
        var result = Task.FromResult(Result.Failure<string>(error));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe()
            .Failure()
            .AndCode("TEST_CODE");
    }

    [Fact]
    public async Task ToResultAsync_WithFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Task.FromResult(Result.Failure<int>(error).WithMetadata("key", "value"));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe().Failure();
        Assert.Equal("value", converted.Metadata["key"]);
    }

    #endregion

    #region ToResultAsync - Edge Cases

    [Fact]
    public async Task ToResultAsync_WithComplexType_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(new { Name = "John", Age = 30 }));

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe().Success();
    }

    [Fact]
    public async Task ToResultAsync_CanChainWithOtherOperations()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(42))
            .MapAsync(x => x * 2);

        // Act (When)
        var converted = await result.ToResultAsync();

        // Assert (Then)
        converted.ShouldBe().Success();
    }

    #endregion
}
