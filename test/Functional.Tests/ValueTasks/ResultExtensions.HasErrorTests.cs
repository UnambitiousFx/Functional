using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task HasErrorAsync_WithAwaitableSuccessResult_ReturnsFalse()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success());

        // Act (When)
        var hasError = await awaitableResult.HasErrorAsync<ExceptionalError>();

        // Assert (Then)
        Assert.False(hasError);
    }

    [Fact]
    public async Task HasErrorAsync_WithAwaitableFailureWithMatchingError_ReturnsTrue()
    {
        // Arrange (Given)
        var awaitableResult =
            ValueTask.FromResult(Result.Failure(new ExceptionalError(new InvalidOperationException())));

        // Act (When)
        var hasError = await awaitableResult.HasErrorAsync<ExceptionalError>();

        // Assert (Then)
        Assert.True(hasError);
    }

    [Fact]
    public async Task HasErrorAsync_WithAwaitableFailureWithNonMatchingError_ReturnsFalse()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure("Simple error"));

        // Act (When)
        var hasError = await awaitableResult.HasErrorAsync<ValidationError>();

        // Assert (Then)
        Assert.False(hasError);
    }
}
