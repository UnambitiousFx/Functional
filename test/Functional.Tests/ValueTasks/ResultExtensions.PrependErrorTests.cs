using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for async PrependError extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task PrependErrorAsync_WithFailedResult_PrependsPrefix()
    {
        // Arrange (Given)
        var result = Result.Failure("Original error");

        // Act (When)
        var prepended = await result.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Context: Original error");
    }

    [Fact]
    public async Task PrependErrorAsync_WithSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var prepended = await result.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Success();
    }

    [Fact]
    public async Task PrependErrorAsync_WithEmptyPrefix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Failure("Original error");

        // Act (When)
        var prepended = await result.PrependErrorAsync("");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task PrependErrorAsync_WithAwaitableFailedResult_PrependsPrefix()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure("Original error"));

        // Act (When)
        var prepended = await awaitableResult.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Context: Original error");
    }

    [Fact]
    public async Task PrependErrorAsync_WithAwaitableSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success());

        // Act (When)
        var prepended = await awaitableResult.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Success();
    }

    [Fact]
    public async Task PrependErrorAsync_Generic_WithFailedResult_PrependsPrefix()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");

        // Act (When)
        var prepended = await result.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Context: Original error");
    }

    [Fact]
    public async Task PrependErrorAsync_Generic_WithSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var prepended = await result.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task PrependErrorAsync_Generic_WithEmptyPrefix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");

        // Act (When)
        var prepended = await result.PrependErrorAsync("");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task PrependErrorAsync_Generic_WithAwaitableFailedResult_PrependsPrefix()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Original error"));

        // Act (When)
        var prepended = await awaitableResult.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Failure().AndMessage("Context: Original error");
    }

    [Fact]
    public async Task PrependErrorAsync_Generic_WithAwaitableSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var prepended = await awaitableResult.PrependErrorAsync("Context: ");

        // Assert (Then)
        prepended.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }
}
