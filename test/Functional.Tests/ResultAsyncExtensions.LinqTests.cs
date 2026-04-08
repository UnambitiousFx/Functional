using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Linq Tests

    [Fact]
    public async Task Select_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Select(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Select_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Select(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Select_Async_WithSuccess_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.Select(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Select_Async_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.Select(x => ValueTask.FromResult(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task SelectMany_WithSuccess_BindsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.SelectMany(x => Result.Success(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task SelectMany_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.SelectMany(x => Result.Success(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task SelectMany_Async_WithSuccess_BindsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.SelectMany(x => ValueTask.FromResult(Result.Success(x * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task SelectMany_Async_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.SelectMany(x => ValueTask.FromResult(Result.Success(x * 2)));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task SelectMany_WithProjector_WithSuccess_ProjectsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.SelectMany(
            x => Result.Success(x * 2),
            (x, y) => x + y);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(15, v));
    }

    [Fact]
    public async Task SelectMany_AsyncWithProjector_WithSuccess_ProjectsValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(5));

        // Act (When)
        var result = await resultTask.SelectMany(
            x => ValueTask.FromResult(Result.Success(x * 2)),
            (x, y) => x + y);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(15, v));
    }

    [Fact]
    public async Task SelectMany_AsyncWithProjector_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));

        // Act (When)
        var result = await resultTask.SelectMany(
            x => ValueTask.FromResult(Result.Success(x * 2)),
            (x, y) => x + y);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
    }

    [Fact]
    public async Task Where_WithPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Where(x => x > 0);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Where_WithPredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Where(x => x < 0);

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public async Task Where_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error      = new Failure("Test error");
        var resultTask = ValueTask.FromResult(Result.Failure<int>(error));
        var predicateCalled = false;

        // Act (When)
        var result = await resultTask.Where(x => {
            predicateCalled = true;
            return x > 0;
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Test error");
        Assert.False(predicateCalled);
    }

    [Fact]
    public async Task Where_Async_WithPredicateTrue_ReturnsSuccess()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Where(x => ValueTask.FromResult(x > 0));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Where_Async_WithPredicateFalse_ReturnsFailure()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var result = await resultTask.Where(x => ValueTask.FromResult(x < 0));

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    #endregion
}
