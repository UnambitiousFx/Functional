using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for Then, Bind, Ensure, and TryAsync awaitable wrapper extensions using ValueTask.
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
            await ValueTask.CompletedTask;
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
            await ValueTask.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncFunc_ExecutesThenFunc()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var then = await awaitableResult.ThenAsync(x => Result.Success(x * 2));

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_AsyncFunc_ExecutesThenFunc()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var then = await awaitableResult.ThenAsync(x => Result.Success(x * 2));

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task ThenAsync_WithNonGenericResult_Success_ReturnsOriginalValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ThenAsync_WithNonGenericResult_Failure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Failure("Validation failed");
        });

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Validation failed");
    }

    [Fact]
    public async Task ThenAsync_WithNonGenericResult_OriginalFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error");
        var executed = false;

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            executed = true;
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        Assert.False(executed);
        then.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task ThenAsync_WithNonGenericResult_CopyMetadata_CopiesMetadataOnFailure()
    {
        // Arrange (Given)
        var result = Result.Success(42)
            .WithMetadata("key1", "value1");

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Failure("Validation failed");
        }, copyReasonsAndMetadata: true);

        // Assert (Then)
        then.ShouldBe().Failure();
        Assert.Equal("value1", then.Metadata["key1"]);
    }

    [Fact]
    public async Task ThenAsync_WithNonGenericResult_NoCopyMetadata_DoesNotCopyMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(42)
            .WithMetadata("key1", "value1");

        // Act (When)
        var then = await result.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Failure("Validation failed");
        }, copyReasonsAndMetadata: false);

        // Assert (Then)
        then.ShouldBe().Failure();
        Assert.False(then.Metadata.ContainsKey("key1"));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_NonGenericResult_Success_ReturnsOriginalValue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_NonGenericResult_Failure_ReturnsFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Failure("Validation failed");
        });

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Validation failed");
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_NonGenericResult_OriginalFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Original error"));
        var executed = false;

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            executed = true;
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        Assert.False(executed);
        then.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_NonGenericResult_CopyMetadata_CopiesMetadataOnFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(
            Result.Success(42).WithMetadata("key1", "value1"));

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Failure("Validation failed");
        }, copyReasonsAndMetadata: true);

        // Assert (Then)
        then.ShouldBe().Failure();
        Assert.Equal("value1", then.Metadata["key1"]);
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_NonGenericResult_NoCopyMetadata_DoesNotCopyMetadata()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(
            Result.Success(42).WithMetadata("key1", "value1"));

        // Act (When)
        var then = await awaitableResult.ThenAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Failure("Validation failed");
        }, copyReasonsAndMetadata: false);

        // Assert (Then)
        then.ShouldBe().Failure();
        Assert.False(then.Metadata.ContainsKey("key1"));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncNonGenericResult_Success_ReturnsOriginalValue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var then = await awaitableResult.ThenAsync(x => Result.Success());

        // Assert (Then)
        then.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncNonGenericResult_Failure_ReturnsFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var then = await awaitableResult.ThenAsync(x => Result.Failure("Validation failed"));

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Validation failed");
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncNonGenericResult_OriginalFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Original error"));
        var executed = false;

        // Act (When)
        var then = await awaitableResult.ThenAsync(x =>
        {
            executed = true;
            return Result.Success();
        });

        // Assert (Then)
        Assert.False(executed);
        then.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncNonGenericResult_CopyMetadata_CopiesMetadataOnFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(
            Result.Success(42).WithMetadata("key1", "value1"));

        // Act (When)
        var then = await awaitableResult.ThenAsync(
            x => Result.Failure("Validation failed"),
            copyReasonsAndMetadata: true);

        // Assert (Then)
        then.ShouldBe().Failure();
        Assert.Equal("value1", then.Metadata["key1"]);
    }

    [Fact]
    public async Task ThenAsync_WithAwaitableResult_SyncNonGenericResult_NoCopyMetadata_DoesNotCopyMetadata()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(
            Result.Success(42).WithMetadata("key1", "value1"));

        // Act (When)
        var then = await awaitableResult.ThenAsync(
            x => Result.Failure("Validation failed"),
            copyReasonsAndMetadata: false);

        // Assert (Then)
        then.ShouldBe().Failure();
        Assert.False(then.Metadata.ContainsKey("key1"));
    }
}
