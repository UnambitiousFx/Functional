using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result TapBoth extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region TapBoth - Result<T>

    [Fact]
    public void TapBoth_WithSuccess_ExecutesOnSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        var tapped = result.TapBoth(
            value => successValue = value,
            error => failureCalled = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public void TapBoth_WithFailure_ExecutesOnFailure()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);
        var successCalled = false;
        var errorMessage = "";

        // Act (When)
        var tapped = result.TapBoth(
            value => successCalled = true,
            err => errorMessage = err.Message);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(successCalled);
        Assert.Equal("Test error", errorMessage);
    }

    [Fact]
    public void TapBoth_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var tapped = result.TapBoth(
            value => { },
            error => { });

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    #endregion

    #region TapBoth - Non-generic Result

    [Fact]
    public void TapBoth_NonGeneric_WithSuccess_ExecutesOnSuccess()
    {
        // Arrange (Given)
        var result = Result.Success();
        var successCalled = false;
        var failureCalled = false;

        // Act (When)
        var tapped = result.TapBoth(
            () => successCalled = true,
            error => failureCalled = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.True(successCalled);
        Assert.False(failureCalled);
    }

    [Fact]
    public void TapBoth_NonGeneric_WithFailure_ExecutesOnFailure()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);
        var successCalled = false;
        var errorMessage = "";

        // Act (When)
        var tapped = result.TapBoth(
            () => successCalled = true,
            err => errorMessage = err.Message);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(successCalled);
        Assert.Equal("Test error", errorMessage);
    }

    #endregion

    #region TapBoth - Logging Use Case

    [Fact]
    public void TapBoth_CanBeUsedForLogging()
    {
        // Arrange (Given)
        var result = Result.Success(100);
        var log = new List<string>();

        // Act (When)
        var tapped = result.TapBoth(
            value => log.Add($"Success: {value}"),
            error => log.Add($"Error: {error.Message}"));

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.Single(log);
        Assert.Equal("Success: 100", log[0]);
    }

    [Fact]
    public void TapBoth_WithFailure_CanBeUsedForLogging()
    {
        // Arrange (Given)
        var error = new Error("Database connection failed");
        var result = Result.Failure<int>(error);
        var log = new List<string>();

        // Act (When)
        var tapped = result.TapBoth(
            value => log.Add($"Success: {value}"),
            err => log.Add($"Error: {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.Single(log);
        Assert.Equal("Error: Database connection failed", log[0]);
    }

    #endregion

    #region TapBoth - Chaining

    [Fact]
    public void TapBoth_CanChainWithOtherOperations()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var log = new List<string>();

        // Act (When)
        var final = result
            .TapBoth(
                value => log.Add($"Initial: {value}"),
                error => log.Add($"Error: {error.Message}"))
            .Map(x => x * 2)
            .TapBoth(
                value => log.Add($"After map: {value}"),
                error => log.Add($"Error: {error.Message}"));

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(10, value));
        Assert.Equal(2, log.Count);
        Assert.Equal("Initial: 5", log[0]);
        Assert.Equal("After map: 10", log[1]);
    }

    [Fact]
    public void TapBoth_InPipeline_TracksSuccessAndFailure()
    {
        // Arrange (Given)
        var result = Result.Success(150);
        var log = new List<string>();

        // Act (When)
        var final = result
            .TapBoth(
                value => log.Add($"Step 1 success: {value}"),
                error => log.Add($"Step 1 error: {error.Message}"))
            .Bind(x => x < 100 ? Result.Success(x) : Result.Failure<int>("Too large"))
            .TapBoth(
                value => log.Add($"Step 2 success: {value}"),
                error => log.Add($"Step 2 error: {error.Message}"));

        // Assert (Then)
        final.ShouldBe().Failure();
        Assert.Equal(2, log.Count);
        Assert.Equal("Step 1 success: 150", log[0]);
        Assert.Equal("Step 2 error: Too large", log[1]);
    }

    #endregion
}
