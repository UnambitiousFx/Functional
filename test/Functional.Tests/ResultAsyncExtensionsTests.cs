using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultAsyncExtensionsTests
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

    [Fact]
    public async Task TaskResult_Switch_UsesDirectAsyncExtensions()
    {
        // Arrange (Given)
        var start  = Task.FromResult(Result.Ok(3));
        var output = "";

        // Act (When)
        await start.Switch(
            v => output = $"ok:{v}",
            _ => output = "fail");

        // Assert (Then)
        Assert.Equal("ok:3", output);
    }

    #region Map Tests

    [Fact]
    public async Task Map_ValueTask_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_ValueTask_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_ValueTask_WithAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_ValueTask_WithAsyncMapper_AndFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Map(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_Task_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_Task_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Map_Task_WithAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Map(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    #endregion

    #region Match Tests

    [Fact]
    public async Task Match_ValueTask_WithSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
            v => $"success:{v}",
            _ => "failure");

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_ValueTask_WithFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
            v => $"success:{v}",
            e => $"failure:{e.Message}");

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_ValueTask_WithAsyncFunctions_AndSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
            v => ValueTask.FromResult($"success:{v}"),
            _ => ValueTask.FromResult("failure"));

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_ValueTask_WithAsyncFunctions_AndFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
            v => ValueTask.FromResult($"success:{v}"),
            e => ValueTask.FromResult($"failure:{e.Message}"));

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_Task_WithSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
            v => $"success:{v}",
            _ => "failure");

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_Task_WithFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
            v => $"success:{v}",
            e => $"failure:{e.Message}");

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    [Fact]
    public async Task Match_Task_WithAsyncFunctions_AndSuccess_ExecutesSuccessFunction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var output = await resultTask.Match(
            v => ValueTask.FromResult($"success:{v}"),
            _ => ValueTask.FromResult("failure"));

        // Assert (Then)
        Assert.Equal("success:42", output);
    }

    [Fact]
    public async Task Match_Task_WithAsyncFunctions_AndFailure_ExecutesFailureFunction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var output = await resultTask.Match(
            v => ValueTask.FromResult($"success:{v}"),
            e => ValueTask.FromResult($"failure:{e.Message}"));

        // Assert (Then)
        Assert.Equal("failure:Test error", output);
    }

    #endregion

    #region ValueOr Tests

    [Fact]
    public async Task ValueOr_ValueTask_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithFactory_AndSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithFactory_AndFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithAsyncFactory_AndSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(() => ValueTask.FromResult(99));

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_ValueTask_WithAsyncFactory_AndFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(() => ValueTask.FromResult(99));

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithFailure_ReturnsFallback()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithFactory_AndSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_Task_WithAsyncFactory_AndFailure_ReturnsFactoryValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOr(() => ValueTask.FromResult(99));

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public async Task ValueOrDefault_ValueTask_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrDefault_ValueTask_WithFailure_ReturnsDefault()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(default, value);
    }

    [Fact]
    public async Task ValueOrDefault_Task_WithSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOrDefault_Task_WithFailure_ReturnsDefault()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var value = await resultTask.ValueOrDefault();

        // Assert (Then)
        Assert.Equal(default, value);
    }

    #endregion

    #region Compensate Tests

    [Fact]
    public async Task Compensate_ValueTask_WithSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_ValueTask_WithFailure_AndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_ValueTask_WithFailure_AndFailedRollback_ReturnsAggregateError()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var rollbackError = new Failure("Rollback error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));

        // Act (When)
        var result = await resultTask.Compensate(_ => Result.Failure(rollbackError));

        // Assert (Then)
        result.ShouldBe()
              .Failure();
        Assert.True(result.TryGetFailure(out var error));
        Assert.IsType<AggregateFailure>(error);
    }

    [Fact]
    public async Task Compensate_ValueTask_WithAsyncRollback_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Compensate(_ => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Compensate_ValueTask_WithAsyncRollback_AndFailure_ExecutesRollback()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = ValueTask.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_Task_WithSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_Task_WithFailure_AndSuccessfulRollback_ReturnsOriginalFailure()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = Task.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    [Fact]
    public async Task Compensate_Task_WithAsyncRollback_AndFailure_ExecutesRollback()
    {
        // Arrange (Given)
        var originalError = new Failure("Original error");
        var resultTask    = Task.FromResult(Result.Failure<int>(originalError));
        var rollbackCalled = false;

        // Act (When)
        var result = await resultTask.Compensate(_ =>
        {
            rollbackCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.True(rollbackCalled);
    }

    #endregion

    #region TapFailure Tests

    [Fact]
    public async Task TapFailure_ValueTask_WithSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_ValueTask_WithFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;
        Failure? capturedError = null;

        // Act (When)
        var result = await resultTask.TapFailure(e =>
        {
            actionCalled   = true;
            capturedError = e;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task TapFailure_ValueTask_WithAsyncAction_AndSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_ValueTask_WithAsyncAction_AndFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithAsyncAction_AndSuccess_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task TapFailure_Task_WithAsyncAction_AndFailure_ExecutesAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.TapFailure(_ =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.True(actionCalled);
    }

    #endregion

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
        var resultTask = Task.FromResult(Result.Success(10));

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
        var resultTask = Task.FromResult(Result.Success(10));

        // Act (When)
        var result = await resultTask.Bind(x => ValueTask.FromResult(Result.Success(x * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(20, v));
    }

    #endregion

    #region Tap Tests

    [Fact]
    public async Task Tap_ValueTask_WithSyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x => tappedValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    [Fact]
    public async Task Tap_ValueTask_WithSyncAction_AndFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Tap(x => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task Tap_ValueTask_WithAsyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x =>
        {
            tappedValue = x;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    [Fact]
    public async Task Tap_ValueTask_WithAsyncAction_AndFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Tap(x =>
        {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task Tap_Task_WithSyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x => tappedValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    [Fact]
    public async Task Tap_Task_WithAsyncAction_AndSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var tappedValue = 0;

        // Act (When)
        var result = await resultTask.Tap(x =>
        {
            tappedValue = x;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, tappedValue);
    }

    #endregion

    #region Ensure Tests

    [Fact]
    public async Task Ensure_ValueTask_WithSyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => x > 0,
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Ensure_ValueTask_WithSyncPredicate_AndPredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => x < 0,
            x => new Failure("Value must be negative"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value must be negative");
    }

    [Fact]
    public async Task Ensure_ValueTask_WithAsyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => ValueTask.FromResult(x > 0),
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Ensure_ValueTask_WithAsyncPredicate_AndPredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => ValueTask.FromResult(x < 0),
            x => new Failure("Value must be negative"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value must be negative");
    }

    [Fact]
    public async Task Ensure_ValueTask_WithFailure_DoesNotEvaluatePredicate()
    {
        // Arrange (Given)
        var error      = new Failure("Original error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var predicateCalled = false;

        // Act (When)
        var result = await resultTask.Ensure(
            x =>
            {
                predicateCalled = true;
                return ValueTask.FromResult(true);
            },
            x => new Failure("Should not be called"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Original error");
        Assert.False(predicateCalled);
    }

    [Fact]
    public async Task Ensure_Task_WithSyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => x > 0,
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Ensure_Task_WithAsyncPredicate_AndPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Ensure(
            x => ValueTask.FromResult(x > 0),
            x => new Failure("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    #endregion

    #region Recover Tests

    [Fact]
    public async Task Recover_ValueTask_WithSyncRecover_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Recover(e => 99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Recover_ValueTask_WithSyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => 99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_ValueTask_WithAsyncRecover_AndSuccess_ReturnsOriginalSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Recover(e => ValueTask.FromResult(99));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Recover_ValueTask_WithAsyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => ValueTask.FromResult(99));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_Task_WithSyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => 99);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Recover_Task_WithAsyncRecover_AndFailure_RecoversWithValue()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = Task.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Recover(e => ValueTask.FromResult(99));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(99, v));
    }

    #endregion

    #region Switch Tests

    [Fact]
    public async Task Switch_ValueTask_WithSyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v => successValue = v,
            _ => failureCalled = true);

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_ValueTask_WithSyncActions_AndFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var successCalled = false;
        Failure? capturedError = null;

        // Act (When)
        await resultTask.Switch(
            _ => successCalled = true,
            e => capturedError = e);

        // Assert (Then)
        Assert.False(successCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task Switch_ValueTask_WithAsyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v =>
            {
                successValue = v;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                failureCalled = true;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_ValueTask_WithAsyncActions_AndFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var successCalled = false;
        Failure? capturedError = null;

        // Act (When)
        await resultTask.Switch(
            _ =>
            {
                successCalled = true;
                return ValueTask.CompletedTask;
            },
            e =>
            {
                capturedError = e;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.False(successCalled);
        Assert.NotNull(capturedError);
        Assert.Equal("Test error", capturedError.Message);
    }

    [Fact]
    public async Task Switch_Task_WithSyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v => successValue = v,
            _ => failureCalled = true);

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    [Fact]
    public async Task Switch_Task_WithAsyncActions_AndSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(42));
        var successValue = 0;
        var failureCalled = false;

        // Act (When)
        await resultTask.Switch(
            v =>
            {
                successValue = v;
                return ValueTask.CompletedTask;
            },
            _ =>
            {
                failureCalled = true;
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal(42, successValue);
        Assert.False(failureCalled);
    }

    #endregion
}
