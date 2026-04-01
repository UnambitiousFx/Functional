using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultAsyncExtensionsTests
{
    #region Combine Tests

    [Fact]
    public async Task Combine_ValueTask_AllSuccess_ReturnsSuccess()
    {
        // Arrange (Given)
        var tasks = new[] {
            ValueTask.FromResult(Result.Success()),
            ValueTask.FromResult(Result.Success()),
        };

        // Act (When)
        var result = await tasks.Combine();

        // Assert (Then)
        result.ShouldBe()
              .Success();
    }

    [Fact]
    public async Task Combine_ValueTask_SomeFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var error = new Failure("error1");
        var tasks = new[] {
            ValueTask.FromResult(Result.Success()),
            ValueTask.FromResult(Result.Failure(error)),
        };

        // Act (When)
        var result = await tasks.Combine();

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public async Task Combine_ValueTaskGeneric_AllSuccess_ReturnsAllValues()
    {
        // Arrange (Given)
        var tasks = new[] {
            ValueTask.FromResult(Result.Success(1)),
            ValueTask.FromResult(Result.Success(2)),
            ValueTask.FromResult(Result.Success(3)),
        };

        // Act (When)
        var result = await tasks.Combine();

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(values => Assert.Equal(new[] { 1, 2, 3 }, values));
    }

    [Fact]
    public async Task Combine_ValueTaskGeneric_SomeFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var error = new Failure("error1");
        var tasks = new[] {
            ValueTask.FromResult(Result.Success(1)),
            ValueTask.FromResult(Result.Failure<int>(error)),
        };

        // Act (When)
        var result = await tasks.Combine();

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    #endregion
}
