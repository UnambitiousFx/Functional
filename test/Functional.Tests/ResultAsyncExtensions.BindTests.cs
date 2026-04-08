using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    [Fact]
    public async Task ValueTaskResult_Pipeline_CanBindMapTapWithoutWrapper()
    {
        // Arrange (Given)
        var start  = ValueTask.FromResult(Result.Ok(2));
        var tapped = 0;

        // Act (When)
        var result = await start
                          .Bind(v => ValueTask.FromResult(Result.Ok(v + 1)))
                          .Map(v => v * 10)
                          .Tap(v => tapped = v);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(30, v));
        Assert.Equal(30, tapped);
    }

    #region Bind Tests

    [Fact]
    public async Task Bind_ValueTask_WithSyncBind_AndSuccess_TransformsResult()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(10));

        // Act (When)
        var result = await resultTask.Bind(x => Result.Success(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task Bind_ValueTask_WithSyncBind_AndFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Bind(x => Result.Success(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Bind_ValueTask_WithAsyncBind_AndSuccess_TransformsResult()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(10));

        // Act (When)
        var result = await resultTask.Bind(x => ValueTask.FromResult(Result.Success(x * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task Bind_ValueTask_WithAsyncBind_AndFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Bind(x => ValueTask.FromResult(Result.Success(x * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Bind_Task_WithSyncBind_AndSuccess_TransformsResult()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(10));

        // Act (When)
        var result = await resultTask.Bind(x => Result.Success(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task Bind_Task_WithAsyncBind_AndSuccess_TransformsResult()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(10));

        // Act (When)
        var result = await resultTask.Bind(x => ValueTask.FromResult(Result.Success(x * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(20, v));
    }

    #endregion

    #region Bind Non-Generic Tests

    [Fact]
    public async Task Bind_NonGenericResult_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(() => {
            bindCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Success();
        Assert.True(bindCalled);
    }

    [Fact]
    public async Task Bind_NonGenericResult_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(() => {
            bindCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(bindCalled);
    }

    [Fact]
    public async Task Bind_NonGenericResultAsync_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Bind(() => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        result.ShouldBe()
              .Success();
    }

    [Fact]
    public async Task Bind_NonGenericResultAsync_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(() => {
            bindCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(bindCalled);
    }

    [Fact]
    public async Task Bind_NonGenericToGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Bind(() => Result.Success(42));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Bind_NonGenericToGeneric_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));

        // Act (When)
        var result = await resultTask.Bind(() => Result.Success(42));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Bind_NonGenericToGenericAsync_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Bind(() => ValueTask.FromResult(Result.Success(42)));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Bind_NonGenericToGenericAsync_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(() => {
            bindCalled = true;
            return ValueTask.FromResult(Result.Success(42));
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(bindCalled);
    }

    [Fact]
    public async Task Bind_GenericToNonGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(v => {
            bindCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Success();
        Assert.True(bindCalled);
    }

    [Fact]
    public async Task Bind_GenericToNonGeneric_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(v => {
            bindCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(bindCalled);
    }

    [Fact]
    public async Task Bind_GenericToNonGenericAsync_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Bind(v => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        result.ShouldBe()
              .Success();
    }

    [Fact]
    public async Task Bind_GenericToNonGenericAsync_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var bindCalled = false;

        // Act (When)
        var result = await resultTask.Bind(v => {
            bindCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(bindCalled);
    }

    #endregion

    #region Async Bind From Non-Generic Result Tests

    [Fact]
    public async Task Bind_NonGeneric_AsyncToGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var bound = await result.Bind(() => ValueTask.FromResult(Result.Success(42)));

        // Assert (Then)
        bound.ShouldBe()
             .Success()
             .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Bind_NonGeneric_AsyncToGeneric_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error  = new Failure("Test error");
        var result = Result.Failure(error);

        // Act (When)
        var bound = await result.Bind(() => ValueTask.FromResult(Result.Success(42)));

        // Assert (Then)
        bound.ShouldBe()
             .Failure()
             .AndMessage("Test error");
    }

    [Fact]
    public async Task Bind_NonGeneric_AsyncToNonGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result     = Result.Success();
        var bindCalled = false;

        // Act (When)
        var bound = await result.Bind(() => {
            bindCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        bound.ShouldBe()
             .Success();
        Assert.True(bindCalled);
    }

    [Fact]
    public async Task Bind_Generic_AsyncToNonGeneric_WithSuccess_ExecutesBind()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var bound = await result.Bind(v => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        bound.ShouldBe()
             .Success();
    }

    [Fact]
    public async Task Bind_Generic_AsyncToNonGeneric_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var result     = Result.Failure<int>(error);
        var bindCalled = false;

        // Act (When)
        var bound = await result.Bind(v => {
            bindCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        bound.ShouldBe()
             .Failure()
             .AndMessage("Test error");
        Assert.False(bindCalled);
    }

    #endregion
}
