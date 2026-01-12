using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for async Map extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Chaining

    [Fact]
    public async Task MapAsync_CanBeChained()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var final = await result
            .MapAsync(async x =>
            {
                await ValueTask.CompletedTask;
                return x * 2;
            })
            .MapAsync(x => x + 10);

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(20, value));
    }

    #endregion

    #region MapAsync (Non-Generic Result with ValueTask) - Edge Cases

    [Fact]
    public async Task MapAsync_WithNonGenericResult_CanChainWithGenericMap()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var mapped = await result
            .MapAsync(() => ValueTask.FromResult(10))
            .MapAsync(x => x * 2)
            .MapAsync(x => x.ToString());

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("20", value));
    }

    #endregion

    #region MapAsync - Result to ValueTask

    [Fact]
    public async Task MapAsync_WithSuccessAndAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var mapped = await result.MapAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task MapAsync_WithFailure_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error occurred");

        // Act (When)
        var mapped = await result.MapAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return x * 2;
        });

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Error occurred");
    }

    #endregion

    #region MapAsync - ValueTask<Result> with sync mapper

    [Fact]
    public async Task MapAsync_WithValueTaskResultAndSyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(10));

        // Act (When)
        var mapped = await result.MapAsync(x => x + 5);

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal(15, value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskResultFailureAndSyncMapper_ReturnsFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>("Failed"));

        // Act (When)
        var mapped = await result.MapAsync(x => x + 5);

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Failed");
    }

    #endregion

    #region MapAsync - ValueTask<Result> with async mapper

    [Fact]
    public async Task MapAsync_WithValueTaskResultAndAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(20));

        // Act (When)
        var mapped = await result.MapAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return x.ToString();
        });

        // Assert (Then)
        mapped.ShouldBe().Success().And(value => Assert.Equal("20", value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskResultFailureAndAsyncMapper_ReturnsFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var mapped = await result.MapAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return x.ToString();
        });

        // Assert (Then)
        mapped.ShouldBe().Failure().AndMessage("Error");
    }

    #endregion

    #region MapAsync (Non-Generic Result with ValueTask) - Success Cases

    [Fact]
    public async Task MapAsync_WithNonGenericSuccessResult_TransformsToValueUsingAsyncFunc()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(42));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task MapAsync_WithNonGenericSuccessResult_TransformsToStringUsingAsyncFunc()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult("Hello Async"));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("Hello Async", value));
    }

    [Fact]
    public async Task MapAsync_WithNonGenericSuccessResult_TransformsToComplexObjectUsingAsyncFunc()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(new { Name = "Async", Value = 999 }));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal("Async", value.Name);
                Assert.Equal(999, value.Value);
            });
    }

    [Fact]
    public async Task MapAsync_WithNonGenericSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success().WithMetadata("key", "async-value");

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(100));

        // Assert (Then)
        mapped.ShouldBe().Success();
        Assert.Equal("async-value", mapped.Metadata["key"]);
    }

    [Fact]
    public async Task MapAsync_WithNonGenericSuccessResult_ExecutesAsyncOperation()
    {
        // Arrange (Given)
        var result = Result.Success();
        var wasExecuted = false;

        // Act (When)
        var mapped = await result.MapAsync(async () =>
        {
            await Task.Delay(1);
            wasExecuted = true;
            return 123;
        });

        // Assert (Then)
        Assert.True(wasExecuted);
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(123, value));
    }

    #endregion

    #region MapAsync (Non-Generic Result with ValueTask) - Failure Cases

    [Fact]
    public async Task MapAsync_WithNonGenericFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Async operation failed");
        var result = Result.Failure(error);

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(42));

        // Assert (Then)
        mapped.ShouldBe()
            .Failure()
            .AndMessage("Async operation failed");
    }

    [Fact]
    public async Task MapAsync_WithNonGenericFailureResult_DoesNotExecuteAsyncMapper()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);
        var executed = false;

        // Act (When)
        var mapped = await result.MapAsync(() =>
        {
            executed = true;
            return ValueTask.FromResult(42);
        });

        // Assert (Then)
        Assert.False(executed);
        mapped.ShouldBe().Failure();
    }

    [Fact]
    public async Task MapAsync_WithNonGenericFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("Async test error");
        var result = Result.Failure(error).WithMetadata("key", "async-value");

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult("test"));

        // Assert (Then)
        mapped.ShouldBe().Failure();
        Assert.Equal("async-value", mapped.Metadata["key"]);
    }

    #endregion

    #region MapAsync (ValueTask<Result> with sync map) - Success Cases

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResult_TransformsToValueUsingSyncFunc()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => 42);

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResult_TransformsToString()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => "Mapped Value");

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("Mapped Value", value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResult_TransformsToComplexObject()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => new { Id = 100, Name = "Test" });

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal(100, value.Id);
                Assert.Equal("Test", value.Name);
            });
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success().WithMetadata("key", "preserved"));

        // Act (When)
        var mapped = await result.MapAsync(() => 777);

        // Assert (Then)
        mapped.ShouldBe().Success();
        Assert.Equal("preserved", mapped.Metadata["key"]);
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResult_ExecutesMappingFunction()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());
        var executed = false;

        // Act (When)
        var mapped = await result.MapAsync(() =>
        {
            executed = true;
            return 999;
        });

        // Assert (Then)
        Assert.True(executed);
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(999, value));
    }

    #endregion

    #region MapAsync (ValueTask<Result> with sync map) - Failure Cases

    [Fact]
    public async Task MapAsync_WithValueTaskFailureResult_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("ValueTask error");
        var result = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var mapped = await result.MapAsync(() => 42);

        // Assert (Then)
        mapped.ShouldBe()
            .Failure()
            .AndMessage("ValueTask error");
    }

    [Fact]
    public async Task MapAsync_WithValueTaskFailureResult_DoesNotExecuteMapper()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = ValueTask.FromResult(Result.Failure(error));
        var executed = false;

        // Act (When)
        var mapped = await result.MapAsync(() =>
        {
            executed = true;
            return 42;
        });

        // Assert (Then)
        Assert.False(executed);
        mapped.ShouldBe().Failure();
    }

    [Fact]
    public async Task MapAsync_WithValueTaskFailureResult_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("ValueTask failure");
        var result = ValueTask.FromResult(Result.Failure(error).WithMetadata("key", "error-data"));

        // Act (When)
        var mapped = await result.MapAsync(() => "test");

        // Assert (Then)
        mapped.ShouldBe().Failure();
        Assert.Equal("error-data", mapped.Metadata["key"]);
    }

    #endregion

    #region MapAsync (ValueTask<Result> with sync map) - Edge Cases

    [Fact]
    public async Task MapAsync_WithValueTaskResult_CanChainMultipleMaps()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result
            .MapAsync(() => 5)
            .MapAsync(x => x * 3)
            .MapAsync(x => x.ToString());

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("15", value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskResult_WorksWithDifferentTypes()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => new List<int> { 1, 2, 3 });

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal(3, value.Count);
                Assert.Contains(1, value);
                Assert.Contains(2, value);
                Assert.Contains(3, value);
            });
    }

    #endregion

    #region MapAsync (ValueTask<Result> with async map) - Success Cases

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResultAndAsyncFunc_TransformsToValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(42));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResultAndAsyncFunc_TransformsToString()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult("Async Value"));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("Async Value", value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResultAndAsyncFunc_TransformsToComplexObject()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(new { Id = 123, Name = "AsyncTest" }));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal(123, value.Id);
                Assert.Equal("AsyncTest", value.Name);
            });
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResultAndAsyncFunc_PreservesMetadata()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success().WithMetadata("key", "async-preserved"));

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(888));

        // Assert (Then)
        mapped.ShouldBe().Success();
        Assert.Equal("async-preserved", mapped.Metadata["key"]);
    }

    [Fact]
    public async Task MapAsync_WithValueTaskSuccessResultAndAsyncFunc_ExecutesAsyncMappingFunction()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());
        var executed = false;

        // Act (When)
        var mapped = await result.MapAsync(async () =>
        {
            await Task.Delay(1);
            executed = true;
            return 555;
        });

        // Assert (Then)
        Assert.True(executed);
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal(555, value));
    }

    #endregion

    #region MapAsync (ValueTask<Result> with async map) - Failure Cases

    [Fact]
    public async Task MapAsync_WithValueTaskFailureResultAndAsyncFunc_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("ValueTask async error");
        var result = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(42));

        // Assert (Then)
        mapped.ShouldBe()
            .Failure()
            .AndMessage("ValueTask async error");
    }

    [Fact]
    public async Task MapAsync_WithValueTaskFailureResultAndAsyncFunc_DoesNotExecuteAsyncMapper()
    {
        // Arrange (Given)
        var error = new Error("Async test error");
        var result = ValueTask.FromResult(Result.Failure(error));
        var executed = false;

        // Act (When)
        var mapped = await result.MapAsync(() =>
        {
            executed = true;
            return ValueTask.FromResult(42);
        });

        // Assert (Then)
        Assert.False(executed);
        mapped.ShouldBe().Failure();
    }

    [Fact]
    public async Task MapAsync_WithValueTaskFailureResultAndAsyncFunc_PreservesMetadata()
    {
        // Arrange (Given)
        var error = new Error("ValueTask async failure");
        var result = ValueTask.FromResult(Result.Failure(error).WithMetadata("key", "async-error-data"));

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult("test"));

        // Assert (Then)
        mapped.ShouldBe().Failure();
        Assert.Equal("async-error-data", mapped.Metadata["key"]);
    }

    #endregion

    #region MapAsync (ValueTask<Result> with async map) - Edge Cases

    [Fact]
    public async Task MapAsync_WithValueTaskResultAndAsyncFunc_CanChainMultipleMaps()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result
            .MapAsync(() => ValueTask.FromResult(10))
            .MapAsync(x => x * 4)
            .MapAsync(x => x.ToString());

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("40", value));
    }

    [Fact]
    public async Task MapAsync_WithValueTaskResultAndAsyncFunc_WorksWithDifferentTypes()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(() => ValueTask.FromResult(new List<string> { "a", "b", "c" }));

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value =>
            {
                Assert.Equal(3, value.Count);
                Assert.Contains("a", value);
                Assert.Contains("b", value);
                Assert.Contains("c", value);
            });
    }

    [Fact]
    public async Task MapAsync_WithValueTaskResultAndAsyncFunc_HandlesDelayedExecution()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var mapped = await result.MapAsync(async () =>
        {
            await Task.Delay(10);
            return "Delayed Result";
        });

        // Assert (Then)
        mapped.ShouldBe()
            .Success()
            .And(value => Assert.Equal("Delayed Result", value));
    }

    #endregion
}