using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task MatchErrorAsync_NonGenericResult_WithMatchingError_ExecutesErrorFunc()
    {
        // Arrange (Given)
        var result = Result.Failure(new ExceptionalError(new InvalidOperationException("Test")));

        // Act (When)
        var output = await result.MatchErrorAsync<ExceptionalError, string>(
            async error =>
            {
                await ValueTask.CompletedTask;
                return "Matched";
            },
            async () =>
            {
                await ValueTask.CompletedTask;
                return "Not matched";
            });

        // Assert (Then)
        Assert.Equal("Matched", output);
    }

    [Fact]
    public async Task MatchErrorAsync_NonGenericResult_WithAwaitableMatchingError_ExecutesErrorFunc()
    {
        // Arrange (Given)
        var awaitableResult =
            ValueTask.FromResult(Result.Failure(new ExceptionalError(new InvalidOperationException("Test"))));

        // Act (When)
        var output = await awaitableResult.MatchErrorAsync<ExceptionalError, string>(
            async error =>
            {
                await ValueTask.CompletedTask;
                return "Matched";
            },
            async () =>
            {
                await ValueTask.CompletedTask;
                return "Not matched";
            });

        // Assert (Then)
        Assert.Equal("Matched", output);
    }

    [Fact]
    public async Task MatchErrorAsync_GenericResult_WithMatchingError_ExecutesErrorFunc()
    {
        // Arrange (Given)
        var result = Result.Failure<int>(new ExceptionalError(new InvalidOperationException("Test")));

        // Act (When)
        var output = await result.MatchErrorAsync<ExceptionalError, int, string>(
            async error =>
            {
                await ValueTask.CompletedTask;
                return "Matched";
            },
            async () =>
            {
                await ValueTask.CompletedTask;
                return "Not matched";
            });

        // Assert (Then)
        Assert.Equal("Matched", output);
    }

    [Fact]
    public async Task MatchErrorAsync_GenericResult_WithAwaitableSuccess_ExecutesOtherwiseFunc()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await awaitableResult.MatchErrorAsync<ExceptionalError, int, string>(
            async error =>
            {
                await ValueTask.CompletedTask;
                return "Matched";
            },
            async () =>
            {
                await ValueTask.CompletedTask;
                return "Not matched";
            });

        // Assert (Then)
        Assert.Equal("Not matched", output);
    }
}
