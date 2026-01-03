using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result TapError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region TapError - Exception Details

    [Fact]
    public void TapError_CanInspectErrorMetadata()
    {
        // Arrange (Given)
        var error = new Error(
            "ERR_001",
            "Test error",
            new Dictionary<string, object?> { ["timestamp"] = "2024-01-01", ["severity"] = "high" });
        var result = Result.Failure<int>(error);
        var metadataKeys = new List<string>();

        // Act (When)
        var tapped = result.TapError(err =>
        {
            foreach (var key in err.Metadata.Keys)
            {
                metadataKeys.Add(key);
            }
        });

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.Contains("timestamp", metadataKeys);
        Assert.Contains("severity", metadataKeys);
    }

    #endregion

    #region TapError - Result<T>

    [Fact]
    public void TapError_WithSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var executed = false;

        // Act (When)
        var tapped = result.TapError(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.False(executed);
    }

    [Fact]
    public void TapError_WithFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);
        var capturedMessage = "";

        // Act (When)
        var tapped = result.TapError(err => capturedMessage = err.Message);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.Equal("Test error", capturedMessage);
    }

    [Fact]
    public void TapError_WithFailure_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var tapped = result.TapError(err => { });

        // Assert (Then)
        tapped.ShouldBe().Failure().AndMessage("Test error");
    }

    #endregion

    #region TapError - Non-generic Result

    [Fact]
    public void TapError_NonGeneric_WithSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success();
        var executed = false;

        // Act (When)
        var tapped = result.TapError(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.False(executed);
    }

    [Fact]
    public void TapError_NonGeneric_WithFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);
        var capturedMessage = "";

        // Act (When)
        var tapped = result.TapError(err => capturedMessage = err.Message);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.Equal("Test error", capturedMessage);
    }

    #endregion

    #region TapError - Error Logging Use Case

    [Fact]
    public void TapError_CanBeUsedForErrorLogging()
    {
        // Arrange (Given)
        var error = new Error("Database connection failed");
        var result = Result.Failure<string>(error);
        var errorLog = new List<string>();

        // Act (When)
        var tapped = result.TapError(err => errorLog.Add($"[ERROR] {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.Single(errorLog);
        Assert.Equal("[ERROR] Database connection failed", errorLog[0]);
    }

    [Fact]
    public void TapError_WithSuccess_DoesNotLog()
    {
        // Arrange (Given)
        var result = Result.Success("Success data");
        var errorLog = new List<string>();

        // Act (When)
        var tapped = result.TapError(err => errorLog.Add($"[ERROR] {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.Empty(errorLog);
    }

    #endregion

    #region TapError - Chaining

    [Fact]
    public void TapError_CanChainMultipleTapErrors()
    {
        // Arrange (Given)
        var error = new Error("ERR_001", "Test error");
        var result = Result.Failure<int>(error);
        var log = new List<string>();

        // Act (When)
        var tapped = result
            .TapError(err => log.Add($"Code: {err.Code}"))
            .TapError(err => log.Add($"Message: {err.Message}"))
            .TapError(err => log.Add($"Full: [{err.Code}] {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.Equal(3, log.Count);
        Assert.Equal("Code: ERR_001", log[0]);
        Assert.Equal("Message: Test error", log[1]);
        Assert.Equal("Full: [ERR_001] Test error", log[2]);
    }

    [Fact]
    public void TapError_CanCombineWithTap()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var successLog = new List<int>();
        var errorLog = new List<string>();

        // Act (When)
        var tapped = result
            .Tap(value => successLog.Add(value))
            .TapError(err => errorLog.Add(err.Message))
            .Map(x => x * 2)
            .Tap(value => successLog.Add(value))
            .TapError(err => errorLog.Add(err.Message));

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.Equal(new[] { 42, 84 }, successLog);
        Assert.Empty(errorLog);
    }

    [Fact]
    public void TapError_InPipeline_TracksErrors()
    {
        // Arrange (Given)
        var result = Result.Success(5);
        var errorLog = new List<string>();

        // Act (When)
        var final = result
            .TapError(err => errorLog.Add($"Step 1: {err.Message}"))
            .Bind(x => x > 10 ? Result.Success(x) : Result.Failure<int>("Too small"))
            .TapError(err => errorLog.Add($"Step 2: {err.Message}"))
            .Map(x => x * 2)
            .TapError(err => errorLog.Add($"Step 3: {err.Message}"));

        // Assert (Then)
        final.ShouldBe().Failure();
        Assert.Equal(2, errorLog.Count);
        Assert.Equal("Step 2: Too small", errorLog[0]);
        Assert.Equal("Step 3: Too small", errorLog[1]);
    }

    #endregion
}
