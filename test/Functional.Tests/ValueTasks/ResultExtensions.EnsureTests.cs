using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for Then, Bind, Ensure, and TryAsync awaitable wrapper extensions using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task EnsureAsync_WithSuccess_ConditionTrue_RemainsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var ensured = await result.EnsureAsync(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 0;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Error("Validation failed");
            });

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureAsync_WithSuccess_ConditionFalse_BecomesFailure()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var ensured = await result.EnsureAsync(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 10;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Error("Value must be greater than 10");
            });

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Value must be greater than 10");
    }

    [Fact]
    public async Task EnsureAsync_WithFailure_DoesNotEvaluateCondition()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");
        var conditionEvaluated = false;

        // Act (When)
        var ensured = await result.EnsureAsync(
            async x =>
            {
                await ValueTask.CompletedTask;
                conditionEvaluated = true;
                return true;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Error("Should not appear");
            });

        // Assert (Then)
        Assert.False(conditionEvaluated);
        ensured.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task EnsureAsync_WithAwaitableSuccess_ConditionTrue_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var ensured = await awaitableResult.EnsureAsync(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 0;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Error("Validation failed");
            });

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureAsync_WithAwaitableSuccess_ConditionFalse_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var ensured = await awaitableResult.EnsureAsync(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 10;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Error("Value must be greater than 10");
            });

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Value must be greater than 10");
    }
}
