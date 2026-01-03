using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for async TapError extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task TapErrorAsync_WithSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success();
        var executed = false;

        // Act (When)
        var tapped = await result.TapErrorAsync(async error =>
        {
            await ValueTask.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.False(executed);
    }

    [Fact]
    public async Task TapErrorAsync_WithFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Failure("Error occurred");
        Error? capturedError = null;

        // Act (When)
        var tapped = await result.TapErrorAsync(async error =>
        {
            await ValueTask.CompletedTask;
            capturedError = error;
        });

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.NotNull(capturedError);
        Assert.Equal("Error occurred", capturedError.Message);
    }


    [Fact]
    public async Task TapErrorAsync_WithGenericSuccess_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var executed = false;

        // Act (When)
        var tapped = await result.TapErrorAsync(async error =>
        {
            await ValueTask.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(42, value));
        Assert.False(executed);
    }

    [Fact]
    public async Task TapErrorAsync_WithGenericFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Failed");
        Error? capturedError = null;

        // Act (When)
        var tapped = await result.TapErrorAsync(async error =>
        {
            await ValueTask.CompletedTask;
            capturedError = error;
        });

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.NotNull(capturedError);
        Assert.Equal("Failed", capturedError.Message);
    }


    [Fact]
    public async Task TapErrorAsync_WithValueTaskResultSuccess_DoesNotExecute()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());
        var executed = false;

        // Act (When)
        var tapped = await result.TapErrorAsync(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.False(executed);
    }

    [Fact]
    public async Task TapErrorAsync_WithValueTaskResultFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure("Error"));
        var executed = false;

        // Act (When)
        var tapped = await result.TapErrorAsync(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.True(executed);
    }


    [Fact]
    public async Task TapErrorAsync_WithValueTaskGenericResultSuccess_DoesNotExecute()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(100));
        var executed = false;

        // Act (When)
        var tapped = await result.TapErrorAsync(error => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(100, value));
        Assert.False(executed);
    }

    [Fact]
    public async Task TapErrorAsync_WithValueTaskGenericResultFailure_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>("Failed"));
        Error? capturedError = null;

        // Act (When)
        var tapped = await result.TapErrorAsync(error => capturedError = error);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.NotNull(capturedError);
        Assert.Equal("Failed", capturedError.Message);
    }


    [Fact]
    public async Task TapErrorAsync_WithAwaitableSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var executed = false;
        var awaitableResult = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await awaitableResult.TapErrorAsync(async _ =>
        {
            await ValueTask.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        Assert.False(executed);
        result.ShouldBe().Success();
    }

    [Fact]
    public async Task TapErrorAsync_WithAwaitableFailure_ExecutesActionWithError()
    {
        // Arrange (Given)
        Error? capturedError = null;
        var awaitableResult = ValueTask.FromResult(Result.Failure("Error"));

        // Act (When)
        var result = await awaitableResult.TapErrorAsync(async error =>
        {
            await ValueTask.CompletedTask;
            capturedError = error;
        });

        // Assert (Then)
        Assert.NotNull(capturedError);
        Assert.Equal("Error", capturedError.Message);
        result.ShouldBe().Failure();
    }

    [Fact]
    public async Task TapErrorAsync_Generic_WithAwaitableSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var executed = false;
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await awaitableResult.TapErrorAsync(async _ =>
        {
            await ValueTask.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        Assert.False(executed);
        result.ShouldBe().Success();
    }

    [Fact]
    public async Task TapErrorAsync_Generic_WithAwaitableFailure_ExecutesActionWithError()
    {
        // Arrange (Given)
        Error? capturedError = null;
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var result = await awaitableResult.TapErrorAsync(async error =>
        {
            await ValueTask.CompletedTask;
            capturedError = error;
        });

        // Assert (Then)
        Assert.NotNull(capturedError);
        Assert.Equal("Error", capturedError.Message);
        result.ShouldBe().Failure();
    }
}
