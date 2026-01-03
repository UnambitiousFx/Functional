using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for async AppendError extension methods on Result using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task AppendErrorAsync_WithFailedResult_AppendsSuffix()
    {
        // Arrange (Given)
        var result = Result.Failure("Original error");

        // Act (When)
        var appended = await result.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Original error - additional context");
    }

    [Fact]
    public async Task AppendErrorAsync_WithSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var appended = await result.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Success();
    }

    [Fact]
    public async Task AppendErrorAsync_WithEmptySuffix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Failure("Original error");

        // Act (When)
        var appended = await result.AppendErrorAsync("");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task AppendErrorAsync_WithAwaitableFailedResult_AppendsSuffix()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure("Original error"));

        // Act (When)
        var appended = await awaitableResult.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Original error - additional context");
    }

    [Fact]
    public async Task AppendErrorAsync_WithAwaitableSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success());

        // Act (When)
        var appended = await awaitableResult.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Success();
    }

    [Fact]
    public async Task AppendErrorAsync_Generic_WithFailedResult_AppendsSuffix()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");

        // Act (When)
        var appended = await result.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Original error - additional context");
    }

    [Fact]
    public async Task AppendErrorAsync_Generic_WithSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var appended = await result.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task AppendErrorAsync_Generic_WithEmptySuffix_ReturnsOriginalResult()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");

        // Act (When)
        var appended = await result.AppendErrorAsync("");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task AppendErrorAsync_Generic_WithAwaitableFailedResult_AppendsSuffix()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure<int>("Original error"));

        // Act (When)
        var appended = await awaitableResult.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Failure().AndMessage("Original error - additional context");
    }

    [Fact]
    public async Task AppendErrorAsync_Generic_WithAwaitableSuccessResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        var appended = await awaitableResult.AppendErrorAsync(" - additional context");

        // Assert (Then)
        appended.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }
}
