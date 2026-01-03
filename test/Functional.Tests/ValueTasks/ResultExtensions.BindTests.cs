using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for async Bind extension methods on Result using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    #region Chaining

    [Fact]
    public async Task BindAsync_CanBeChained()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var final = await result
            .BindAsync(async x =>
            {
                await ValueTask.CompletedTask;
                return Result.Success(x * 2);
            })
            .BindAsync(x => Result.Success(x + 10));

        // Assert (Then)
        final.ShouldBe().Success().And(value => Assert.Equal(20, value));
    }

    #endregion

    #region BindAsync - Result with async function returning Result

    [Fact]
    public async Task BindAsync_WithSuccessResult_ExecutesAsyncFunction()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var bound = await result.BindAsync(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task BindAsync_WithFailureResult_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var result = Result.Failure("Original error");

        // Act (When)
        var bound = await result.BindAsync(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Original error");
    }

    #endregion

    #region BindAsync - Result<T> with async function

    [Fact]
    public async Task BindAsync_WithGenericResultAndAsyncFunction_ExecutesFunction()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var bound = await result.BindAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(20, value));
    }

    [Fact]
    public async Task BindAsync_WithGenericResultFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error");

        // Act (When)
        var bound = await result.BindAsync(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(x * 2);
        });

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Error");
    }

    #endregion

    #region BindAsync - ValueTask<Result> with sync function

    [Fact]
    public async Task BindAsync_WithValueTaskResultAndSyncFunction_ExecutesFunction()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var bound = await result.BindAsync(() => Result.Success());

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task BindAsync_WithValueTaskResultFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure("Failed"));

        // Act (When)
        var bound = await result.BindAsync(() => Result.Success());

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Failed");
    }

    #endregion

    #region BindAsync - ValueTask<Result> with sync function returning Result<T>

    [Fact]
    public async Task BindAsync_WithValueTaskResultAndSyncFunctionReturningGenericResult_ExecutesFunction()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var bound = await result.BindAsync(() => Result.Success(42));

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(42, value));
    }

    [Fact]
    public async Task BindAsync_WithValueTaskResultFailureAndSyncFunctionReturningGenericResult_ReturnsFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure("Error occurred"));

        // Act (When)
        var bound = await result.BindAsync(() => Result.Success(42));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Error occurred");
    }

    [Fact]
    public async Task BindAsync_WithValueTaskResultAndSyncFunctionReturningGenericResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success()
            .WithMetadata(m => m.Add("TraceId", "123")));

        // Act (When)
        var bound = await result.BindAsync(() => Result.Success("output"), copyReasonsAndMetadata: true);

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal("output", value));
        Assert.Equal("123", bound.Metadata["TraceId"]);
    }

    [Fact]
    public async Task BindAsync_WithValueTaskResultAndSyncFunctionReturningGenericResult_DoesNotPreserveMetadataWhenDisabled()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success()
            .WithMetadata(m => m.Add("TraceId", "123")));

        // Act (When)
        var bound = await result.BindAsync(() => Result.Success("output"), copyReasonsAndMetadata: false);

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal("output", value));
        Assert.False(bound.Metadata.ContainsKey("TraceId"));
    }

    #endregion

    #region BindAsync - ValueTask<Result<T>> with sync function returning Result

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultAndSyncFunctionReturningNonGenericResult_ExecutesFunction()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(100));

        // Act (When)
        var bound = await result.BindAsync(x => x > 50 ? Result.Success() : Result.Failure("Too small"));

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultAndSyncFunctionReturningNonGenericResult_ExecutesFunctionAndReturnsBindFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(30));

        // Act (When)
        var bound = await result.BindAsync(x => x > 50 ? Result.Success() : Result.Failure("Too small"));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Too small");
    }

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultFailureAndSyncFunctionReturningNonGenericResult_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>("Original error"));

        // Act (When)
        var bound = await result.BindAsync(x => Result.Success());

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultAndSyncFunctionReturningNonGenericResult_PreservesMetadata()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(25)
            .WithMetadata(m => m.Add("RequestId", "req-456")));

        // Act (When)
        var bound = await result.BindAsync(x => Result.Success(), copyReasonsAndMetadata: true);

        // Assert (Then)
        bound.ShouldBe().Success();
        Assert.Equal("req-456", bound.Metadata["RequestId"]);
    }

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultAndSyncFunctionReturningNonGenericResult_DoesNotPreserveMetadataWhenDisabled()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(25)
            .WithMetadata(m => m.Add("RequestId", "req-456")));

        // Act (When)
        var bound = await result.BindAsync(x => Result.Success(), copyReasonsAndMetadata: false);

        // Assert (Then)
        bound.ShouldBe().Success();
        Assert.False(bound.Metadata.ContainsKey("RequestId"));
    }

    #endregion

    #region BindAsync - ValueTask<Result<T>> with sync function

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultAndSyncFunction_ExecutesFunction()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(15));

        // Act (When)
        var bound = await result.BindAsync(x => Result.Success(x + 5));

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(20, value));
    }

    [Fact]
    public async Task BindAsync_WithValueTaskGenericResultFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>("Failed"));

        // Act (When)
        var bound = await result.BindAsync(x => Result.Success(x + 5));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Failed");
    }

    #endregion

    #region Metadata Preservation

    [Fact]
    public async Task BindAsync_WithCopyMetadataTrue_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success()
            .WithMetadata(m => m.Add("Key", "Value"));

        // Act (When)
        var bound = await result.BindAsync(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(10);
        }, true);

        // Assert (Then)
        bound.ShouldBe().Success();
        Assert.Equal("Value", bound.Metadata["Key"]);
    }

    [Fact]
    public async Task BindAsync_WithCopyMetadataFalse_DoesNotPreserveMetadata()
    {
        // Arrange (Given)
        var result = Result.Success()
            .WithMetadata(m => m.Add("Key", "Value"));

        // Act (When)
        var bound = await result.BindAsync(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(10);
        }, false);

        // Assert (Then)
        bound.ShouldBe().Success();
        Assert.False(bound.Metadata.ContainsKey("Key"));
    }

    #endregion
}
