using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result TapFailure extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    #region TapFailure - Exception Details

    [Fact]
    public void TapFailure_CanInspectErrorMetadata()
    {
        // Arrange (Given)
        var error = new Failure(
            "ERR_001",
            "Test error",
            new Dictionary<string, object?> { ["timestamp"] = "2024-01-01", ["severity"] = "high" });
        var result       = Result.Failure<int>(error);
        var metadataKeys = new List<string>();

        // Act (When)
        var tapped = result.TapFailure(err =>
        {
            foreach (var key in err.Metadata.Keys) {
                metadataKeys.Add(key);
            }
        });

        // Assert (Then)
        tapped.ShouldBe()
              .Failure();
        Assert.Contains("timestamp", metadataKeys);
        Assert.Contains("severity",  metadataKeys);
    }

    #endregion

    #region TapFailure - Result<T>

    [Fact]
    public void TapFailure_WithSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result   = Result.Success(42);
        var executed = false;

        // Act (When)
        var tapped = result.TapFailure(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.False(executed);
    }

    [Fact]
    public void TapFailure_WithFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var error           = new Failure("Test error");
        var result          = Result.Failure<int>(error);
        var capturedMessage = "";

        // Act (When)
        var tapped = result.TapFailure(err => capturedMessage = err.Message);

        // Assert (Then)
        tapped.ShouldBe()
              .Failure();
        Assert.Equal("Test error", capturedMessage);
    }

    [Fact]
    public void TapFailure_WithFailure_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var error  = new Failure("Test error");
        var result = Result.Failure<int>(error);

        // Act (When)
        var tapped = result.TapFailure(err => { });

        // Assert (Then)
        tapped.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    #endregion

    #region TapFailure - Non-generic Result

    [Fact]
    public void TapFailure_NonGeneric_WithSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result   = Result.Success();
        var executed = false;

        // Act (When)
        var tapped = result.TapFailure(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.False(executed);
    }

    [Fact]
    public void TapFailure_NonGeneric_WithFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var error           = new Failure("Test error");
        var result          = Result.Failure(error);
        var capturedMessage = "";

        // Act (When)
        var tapped = result.TapFailure(err => capturedMessage = err.Message);

        // Assert (Then)
        tapped.ShouldBe()
              .Failure();
        Assert.Equal("Test error", capturedMessage);
    }

    #endregion

    #region TapFailure - Error Logging Use Case

    [Fact]
    public void TapFailure_CanBeUsedForErrorLogging()
    {
        // Arrange (Given)
        var error    = new Failure("Database connection failed");
        var result   = Result.Failure<string>(error);
        var errorLog = new List<string>();

        // Act (When)
        var tapped = result.TapFailure(err => errorLog.Add($"[ERROR] {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe()
              .Failure();
        Assert.Single(errorLog);
        Assert.Equal("[ERROR] Database connection failed", errorLog[0]);
    }

    [Fact]
    public void TapFailure_WithSuccess_DoesNotLog()
    {
        // Arrange (Given)
        var result   = Result.Success("Success data");
        var errorLog = new List<string>();

        // Act (When)
        var tapped = result.TapFailure(err => errorLog.Add($"[ERROR] {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.Empty(errorLog);
    }

    #endregion

    #region TapFailure - Chaining

    [Fact]
    public void TapFailure_CanChainMultipleTapFailures()
    {
        // Arrange (Given)
        var error  = new Failure("ERR_001", "Test error");
        var result = Result.Failure<int>(error);
        var log    = new List<string>();

        // Act (When)
        var tapped = result
                    .TapFailure(err => log.Add($"Code: {err.Code}"))
                    .TapFailure(err => log.Add($"Message: {err.Message}"))
                    .TapFailure(err => log.Add($"Full: [{err.Code}] {err.Message}"));

        // Assert (Then)
        tapped.ShouldBe()
              .Failure();
        Assert.Equal(3,                            log.Count);
        Assert.Equal("Code: ERR_001",              log[0]);
        Assert.Equal("Message: Test error",        log[1]);
        Assert.Equal("Full: [ERR_001] Test error", log[2]);
    }

    [Fact]
    public void TapFailure_CanCombineWithTap()
    {
        // Arrange (Given)
        var result     = Result.Success(42);
        var successLog = new List<int>();
        var errorLog   = new List<string>();

        // Act (When)
        var tapped = result
                    .Tap(value => successLog.Add(value))
                    .TapFailure(err => errorLog.Add(err.Message))
                    .Map(x => x * 2)
                    .Tap(value => successLog.Add(value))
                    .TapFailure(err => errorLog.Add(err.Message));

        // Assert (Then)
        tapped.ShouldBe()
              .Success();
        Assert.Equal(new[] { 42, 84 }, successLog);
        Assert.Empty(errorLog);
    }

    [Fact]
    public void TapFailure_InPipeline_TracksErrors()
    {
        // Arrange (Given)
        var result   = Result.Success(5);
        var errorLog = new List<string>();

        // Act (When)
        var final = result
                   .TapFailure(err => errorLog.Add($"Step 1: {err.Message}"))
                   .Bind(x => x > 10
                                  ? Result.Success(x)
                                  : Result.Failure<int>("Too small"))
                   .TapFailure(err => errorLog.Add($"Step 2: {err.Message}"))
                   .Map(x => x * 2)
                   .TapFailure(err => errorLog.Add($"Step 3: {err.Message}"));

        // Assert (Then)
        final.ShouldBe()
             .Failure();
        Assert.Equal(2,                   errorLog.Count);
        Assert.Equal("Step 2: Too small", errorLog[0]);
        Assert.Equal("Step 3: Too small", errorLog[1]);
    }

    #endregion
}
