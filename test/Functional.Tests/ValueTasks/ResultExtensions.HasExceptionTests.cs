using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task HasExceptionAsync_WithAwaitableSuccessResult_ReturnsFalse()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success());

        // Act (When)
        var hasException = await awaitableResult.HasExceptionAsync<InvalidOperationException>();

        // Assert (Then)
        Assert.False(hasException);
    }

    [Fact]
    public async Task HasExceptionAsync_WithAwaitableFailureWithMatchingException_ReturnsTrue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure(new InvalidOperationException("Test")));

        // Act (When)
        var hasException = await awaitableResult.HasExceptionAsync<InvalidOperationException>();

        // Assert (Then)
        Assert.True(hasException);
    }

    [Fact]
    public async Task HasExceptionAsync_WithAwaitableFailureWithNonMatchingException_ReturnsFalse()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure(new InvalidOperationException("Test")));

        // Act (When)
        var hasException = await awaitableResult.HasExceptionAsync<ArgumentException>();

        // Assert (Then)
        Assert.False(hasException);
    }
}
