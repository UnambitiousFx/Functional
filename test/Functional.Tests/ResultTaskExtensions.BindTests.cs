using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for ResultTask Bind extension methods.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Bind_WithResultTaskAndAsyncFunction_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(5).AsAsync();

        // Act (When)
        var bound = await result.Bind(async value =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(value * 2);
        });

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task Bind_WithResultTaskFailure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Original error").AsAsync();

        // Act (When)
        var bound = await result.Bind(value => Result.Success(value * 2));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Original error");
    }

    [Fact]
    public async Task Bind_WithResultTaskToNonGenericResult_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(10).AsAsync();

        // Act (When)
        var bound = await result.Bind(value => value > 0 ? Result.Success() : Result.Failure("Invalid"));

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task Bind_WithResultTaskToNonGenericResult_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(10).AsAsync();

        // Act (When)
        var bound = await result.Bind(value => value > 50 ? Result.Success() : Result.Failure("Too small"));

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Too small");
    }

    [Fact]
    public async Task Bind_PreservesMetadata()
    {
        // Arrange (Given)
        var result = Result.Success(5)
            .WithMetadata("TraceId", "123")
            .AsAsync();

        // Act (When)
        var bound = await result.Bind(value => Result.Success(value + 1));

        // Assert (Then)
        bound.ShouldBe().Success().And(value => Assert.Equal(6, value));
        Assert.Equal("123", bound.Metadata["TraceId"]);
    }
}