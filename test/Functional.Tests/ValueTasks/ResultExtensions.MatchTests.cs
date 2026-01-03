using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task MatchAsync_WithAwaitableSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await awaitableResult.MatchAsync(
            async value =>
            {
                await ValueTask.CompletedTask;
                return $"Success: {value}";
            },
            async error =>
            {
                await ValueTask.CompletedTask;
                return $"Failure: {error.Message}";
            });

        // Assert (Then)
        Assert.Equal("Success: 42", output);
    }

    [Fact]
    public async Task MatchAsync_WithAwaitableFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var output = await awaitableResult.MatchAsync(
            async value =>
            {
                await ValueTask.CompletedTask;
                return $"Success: {value}";
            },
            async error =>
            {
                await ValueTask.CompletedTask;
                return $"Failure: {error.Message}";
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", output);
    }

    [Fact]
    public async Task MatchAsync_VoidReturn_WithAwaitableSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        await awaitableResult.MatchAsync(
            async value =>
            {
                await ValueTask.CompletedTask;
                executed = $"Success: {value}";
            },
            async error =>
            {
                await ValueTask.CompletedTask;
                executed = $"Failure: {error.Message}";
            });

        // Assert (Then)
        Assert.Equal("Success: 42", executed);
    }

    [Fact]
    public async Task MatchAsync_VoidReturn_WithAwaitableFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        await awaitableResult.MatchAsync(
            async value =>
            {
                await ValueTask.CompletedTask;
                executed = $"Success: {value}";
            },
            async error =>
            {
                await ValueTask.CompletedTask;
                executed = $"Failure: {error.Message}";
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }
}
