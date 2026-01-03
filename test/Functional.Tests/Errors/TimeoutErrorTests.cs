using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests.Errors;

/// <summary>
/// Tests for TimeoutError type.
/// </summary>
public class TimeoutErrorTests
{
    [Fact]
    public void TimeoutError_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromSeconds(30);
        var elapsed = TimeSpan.FromSeconds(45);

        // Act (When)
        var error = new TimeoutError(configuredTimeout, elapsed);

        // Assert (Then)
        Assert.Equal(configuredTimeout, error.ConfiguredTimeout);
        Assert.Equal(elapsed, error.Elapsed);
    }

    [Fact]
    public void TimeoutError_Message_ContainsTimeoutInformation()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromSeconds(30);
        var elapsed = TimeSpan.FromSeconds(45);

        // Act (When)
        var error = new TimeoutError(configuredTimeout, elapsed);

        // Assert (Then)
        Assert.Contains("30000", error.Message); // 30 seconds = 30000ms
        Assert.Contains("45000", error.Message); // 45 seconds = 45000ms
        Assert.Contains("exceeded timeout", error.Message.ToLower());
    }

    [Fact]
    public void TimeoutError_Code_IsTimeoutErrorCode()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromSeconds(10);
        var elapsed = TimeSpan.FromSeconds(15);

        // Act (When)
        var error = new TimeoutError(configuredTimeout, elapsed);

        // Assert (Then)
        Assert.Equal(ErrorCodes.Timeout, error.Code);
    }

    [Fact]
    public void TimeoutError_WithMilliseconds_FormatsMessageCorrectly()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromMilliseconds(500);
        var elapsed = TimeSpan.FromMilliseconds(750);

        // Act (When)
        var error = new TimeoutError(configuredTimeout, elapsed);

        // Assert (Then)
        Assert.Contains("500", error.Message);
        Assert.Contains("750", error.Message);
    }

    [Fact]
    public void TimeoutError_WithZeroTimeout_FormatsMessageCorrectly()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.Zero;
        var elapsed = TimeSpan.FromSeconds(1);

        // Act (When)
        var error = new TimeoutError(configuredTimeout, elapsed);

        // Assert (Then)
        Assert.Contains("0", error.Message);
        Assert.Contains("1000", error.Message);
    }

    [Fact]
    public void TimeoutError_RecordEquality_WorksCorrectly()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromSeconds(30);
        var elapsed = TimeSpan.FromSeconds(45);
        var error1 = new TimeoutError(configuredTimeout, elapsed);
        var error2 = new TimeoutError(configuredTimeout, elapsed);

        // Act & Assert (When/Then)
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void TimeoutError_RecordInequality_WithDifferentTimeout_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new TimeoutError(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45));
        var error2 = new TimeoutError(TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(45));

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void TimeoutError_RecordInequality_WithDifferentElapsed_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new TimeoutError(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(45));
        var error2 = new TimeoutError(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(60));

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void TimeoutError_CanBeUsedInResult()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromSeconds(30);
        var elapsed = TimeSpan.FromSeconds(45);
        var timeoutError = new TimeoutError(configuredTimeout, elapsed);

        // Act (When)
        var result = Result.Failure<string>(timeoutError);

        // Assert (Then)
        Assert.True(result.IsFaulted);
        result.TryGet(out Error? error);
        Assert.IsType<TimeoutError>(error);
        var typedError = (TimeoutError)error;
        Assert.Equal(configuredTimeout, typedError.ConfiguredTimeout);
        Assert.Equal(elapsed, typedError.Elapsed);
    }

    [Fact]
    public void TimeoutError_WithLargeTimeSpans_FormatsCorrectly()
    {
        // Arrange (Given)
        var configuredTimeout = TimeSpan.FromHours(2);
        var elapsed = TimeSpan.FromHours(3);

        // Act (When)
        var error = new TimeoutError(configuredTimeout, elapsed);

        // Assert (Then)
        // 2 hours = 7200000ms, 3 hours = 10800000ms
        Assert.Contains("7200000", error.Message);
        Assert.Contains("10800000", error.Message);
    }
}
