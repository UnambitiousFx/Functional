using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for ResultTask Then extension methods.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Then_WithSuccess_ExecutesThenFunc()
    {
        // Arrange (Given)
        var result = Result.Success(5).AsAsync();

        // Act (When)
        var then = await result.Then(value => Result.Success(value * 2));

        // Assert (Then)
        then.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public async Task Then_WithFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var then = await result.Then(value => Result.Success(value * 2));

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task Then_WithNonGenericResult_Failure_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(42)
            .WithMetadata("key1", "value1")
            .AsAsync();

        // Act (When)
        var then = await result.Then(value => Result.Failure("Validation failed"));

        // Assert (Then)
        then.ShouldBe().Failure().AndMessage("Validation failed");
        Assert.Equal("value1", then.Metadata["key1"]);
    }
}