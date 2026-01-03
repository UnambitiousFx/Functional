using UnambitiousFx.Functional.Tasks;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task MatchAsync_WithAwaitableSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        var output = await awaitableResult.MatchAsync(
            async value =>
            {
                await Task.CompletedTask;
                return $"Success: {value}";
            },
            async error =>
            {
                await Task.CompletedTask;
                return $"Failure: {error.Message}";
            });

        // Assert (Then)
        Assert.Equal("Success: 42", output);
    }

    [Fact]
    public async Task MatchAsync_WithAwaitableFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var output = await awaitableResult.MatchAsync(
            async value =>
            {
                await Task.CompletedTask;
                return $"Success: {value}";
            },
            async error =>
            {
                await Task.CompletedTask;
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
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        await awaitableResult.MatchAsync(
            async value =>
            {
                await Task.CompletedTask;
                executed = $"Success: {value}";
            },
            async error =>
            {
                await Task.CompletedTask;
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
        var awaitableResult = Task.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        await awaitableResult.MatchAsync(
            async value =>
            {
                await Task.CompletedTask;
                executed = $"Success: {value}";
            },
            async error =>
            {
                await Task.CompletedTask;
                executed = $"Failure: {error.Message}";
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }
}
