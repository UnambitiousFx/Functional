using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for Then, Bind, Ensure, and TryAsync awaitable wrapper extensions using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task ThenAsync_WithSyncResult_Success_ExecutesThenFunc()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            await Task.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task ThenAsync_WithSyncResult_Failure_DoesNotExecute()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error");

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            await Task.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncFunc_ExecutesThenFunc()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(5));

        // Act (When)
        var then = await awaitableResult.ThenAsync(x => Result.Success(x * 2));

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_AsyncFunc_ExecutesThenFunc()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(5));

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            await Task.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var then = await awaitableResult.ThenAsync(x => Result.Success(x * 2));

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Error");
    }
}
