using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Then Tests

    [Fact]
    public async Task Then_NonGeneric_WithSuccess_ExecutesThen()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Then(() => Result.Success());

        // Assert (Then)
        result.ShouldBe()
              .Success();
    }

    [Fact]
    public async Task Then_NonGeneric_WithFailure_PropagatesFailure()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));
        var thenCalled = false;

        // Act (When)
        var result = await resultTask.Then(() => {
            thenCalled = true;
            return Result.Success();
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(thenCalled);
    }

    [Fact]
    public async Task Then_NonGenericAsync_WithSuccess_ExecutesThen()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var result = await resultTask.Then(() => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        result.ShouldBe()
              .Success();
    }

    [Fact]
    public async Task Then_NonGenericAsync_WithFailure_PropagatesFailure()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure(error));
        var thenCalled = false;

        // Act (When)
        var result = await resultTask.Then(() => {
            thenCalled = true;
            return ValueTask.FromResult(Result.Success());
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(thenCalled);
    }

    [Fact]
    public async Task Then_Generic_WithSuccess_ExecutesThen()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Then(v => Result.Success(v * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(84, v));
    }

    [Fact]
    public async Task Then_Generic_WithFailure_PropagatesFailure()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var thenCalled = false;

        // Act (When)
        var result = await resultTask.Then(v => {
            thenCalled = true;
            return Result.Success(v * 2);
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(thenCalled);
    }

    [Fact]
    public async Task Then_GenericAsync_WithSuccess_ExecutesThen()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Then(v => ValueTask.FromResult(Result.Success(v * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(84, v));
    }

    [Fact]
    public async Task Then_GenericAsync_WithFailure_PropagatesFailure()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var thenCalled = false;

        // Act (When)
        var result = await resultTask.Then(v => {
            thenCalled = true;
            return ValueTask.FromResult(Result.Success(v * 2));
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(thenCalled);
    }

    [Fact]
    public async Task Then_GenericReturnsResult_WithSuccess_ExecutesThen()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Then(_ => Result.Success());

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Then_GenericReturnsResult_WithThenFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var thenError  = new Failure("then error");

        // Act (When)
        var result = await resultTask.Then(_ => Result.Failure(thenError));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("then error");
    }

    [Fact]
    public async Task Then_GenericReturnsResultAsync_WithSuccess_ExecutesThen()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Then(_ => ValueTask.FromResult(Result.Success()));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Then_GenericReturnsResultAsync_WithThenFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));
        var thenError  = new Failure("then error");

        // Act (When)
        var result = await resultTask.Then(_ => ValueTask.FromResult(Result.Failure(thenError)));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("then error");
    }

    #endregion
}
