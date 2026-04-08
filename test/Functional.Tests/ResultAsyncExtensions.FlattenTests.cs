using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Flatten Tests

    [Fact]
    public async Task Flatten_ValueTask_WithNestedSuccess_ReturnsInnerValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(Result.Success(42)));

        // Act (When)
        var result = await resultTask.Flatten();

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Flatten_ValueTask_WithOuterFailure_PropagatesFailure()
    {
        // Arrange (Given)
        var error      = new Failure("outer error");
        var resultTask = ValueTask.FromResult(Result.Failure<Result<int>>(error));

        // Act (When)
        var result = await resultTask.Flatten();

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("outer error");
    }

    [Fact]
    public async Task Flatten_ValueTask_WithNestedAsyncSuccess_ReturnsInnerValue()
    {
        // Arrange (Given)
        var inner      = ValueTask.FromResult(Result.Success(42));
        var resultTask = ValueTask.FromResult(Result.Success(inner));

        // Act (When)
        var result = await resultTask.Flatten();

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Flatten_ValueTask_WithOuterFailureAndAsyncInner_PropagatesFailure()
    {
        // Arrange (Given)
        var error      = new Failure("outer error");
        var resultTask = ValueTask.FromResult(Result.Failure<ValueTask<Result<int>>>(error));

        // Act (When)
        var result = await resultTask.Flatten();

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("outer error");
    }

    #endregion
}
