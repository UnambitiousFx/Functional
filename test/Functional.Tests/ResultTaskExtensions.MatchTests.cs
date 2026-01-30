namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Match_WithResultTaskSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            value => ValueTask.FromResult($"Success: {value}"),
            error => ValueTask.FromResult($"Failure: {error.Message}"));

        // Assert (Then)
        Assert.Equal("Success: 42", output);
    }

    [Fact]
    public async Task Match_WithResultTaskFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            value => ValueTask.FromResult($"Success: {value}"),
            error => ValueTask.FromResult($"Failure: {error.Message}"));

        // Assert (Then)
        Assert.Equal("Failure: Error", output);
    }

    [Fact]
    public async Task Match_VoidReturn_WithResultTaskSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value =>
            {
                executed = $"Success: {value}";
                return ValueTask.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Success: 42", executed);
    }

    [Fact]
    public async Task Match_VoidReturn_WithResultTaskFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value =>
            {
                executed = $"Success: {value}";
                return ValueTask.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }
}