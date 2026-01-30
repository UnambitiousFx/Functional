using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Then, Bind, Ensure, and TryAsync awaitable wrapper extensions using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Ensure_WithSuccess_ConditionTrue_RemainsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var ensured = await result.AsAsync().Ensure(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 0;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Failure("Validation failed");
            });

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task Ensure_WithSuccess_ConditionFalse_BecomesFailure()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var ensured = await result.AsAsync().Ensure(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 10;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Failure("Value must be greater than 10");
            });

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Value must be greater than 10");
    }

    [Fact]
    public async Task Ensure_WithFailure_DoesNotEvaluateCondition()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");
        var conditionEvaluated = false;

        // Act (When)
        var ensured = await result.AsAsync().Ensure(
            async x =>
            {
                await ValueTask.CompletedTask;
                conditionEvaluated = true;
                return true;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Failure("Should not appear");
            });

        // Assert (Then)
        Assert.False(conditionEvaluated);
        ensured.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task Ensure_WithResultTaskSuccess_ConditionTrue_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(5).AsAsync();

        // Act (When)
        var ensured = await awaitableResult.Ensure(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 0;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Failure("Validation failed");
            });

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task Ensure_WithResultTaskSuccess_ConditionFalse_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(5).AsAsync();

        // Act (When)
        var ensured = await awaitableResult.Ensure(
            async x =>
            {
                await ValueTask.CompletedTask;
                return x > 10;
            },
            async x =>
            {
                await ValueTask.CompletedTask;
                return new Failure("Value must be greater than 10");
            });

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Value must be greater than 10");
    }
}