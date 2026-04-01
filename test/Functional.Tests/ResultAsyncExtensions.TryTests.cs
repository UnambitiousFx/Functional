using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;
using ExceptionalFailure = UnambitiousFx.Functional.Failures.ExceptionalFailure;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Try Tests

    [Fact]
    public async Task Try_NonGeneric_WithSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask   = ValueTask.FromResult(Result.Success());
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Try(() => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Success();
        Assert.True(actionCalled);
    }

    [Fact]
    public async Task Try_NonGeneric_WithFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var error        = new Failure("Test error");
        var resultTask   = ValueTask.FromResult(Result.Failure(error));
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Try(() => actionCalled = true);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(actionCalled);
    }

    [Fact]
    public async Task Try_NonGeneric_WithActionThatThrows_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Try(() => throw new InvalidOperationException("boom"));

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public async Task Try_NonGenericAsync_WithSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask   = ValueTask.FromResult(Result.Success());
        var actionCalled = false;

        // Act (When)
        var result = await resultTask.Try(() => {
            actionCalled = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        result.ShouldBe()
              .Success();
        Assert.True(actionCalled);
    }

    [Fact]
    public async Task Try_NonGenericAsync_WithActionThatThrows_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Try(async () => {
            await Task.Delay(1);
            throw new InvalidOperationException("async boom");
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public async Task Try_Generic_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Try(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Try_Generic_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Try(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Try_Generic_WithFuncThatThrows_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Try((Func<int, int>)(_ => throw new InvalidOperationException("boom")));

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public async Task Try_GenericAsync_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Try(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Try_GenericAsync_WithFuncThatThrows_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Try<int, int>(async _ => {
            await Task.Delay(1);
            throw new InvalidOperationException("async boom");
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    #endregion
}
