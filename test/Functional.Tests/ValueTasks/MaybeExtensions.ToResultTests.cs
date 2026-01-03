using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

public sealed class MaybeExtensionsToResultTests
{
    [Fact]
    public async Task ToResult_WithError_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));
        var error = new Error("code", "message");

        // Act (When)
        var result = await maybeTask.ToResult(error);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ToResult_WithError_WithNone_ReturnsFailure()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.None<int>());
        var error = new Error("code", "message");

        // Act (When)
        var result = await maybeTask.ToResult(error);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(error.Code)
            .AndMessage(error.Message);
    }

    [Fact]
    public async Task ToResult_WithErrorFactory_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybeTask.ToResult(() => new Error("code", "message"));

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ToResult_WithErrorFactory_WithNone_ReturnsFailure()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.None<int>());
        var error = new Error("code", "message");

        // Act (When)
        var result = await maybeTask.ToResult(() => error);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndCode(error.Code)
            .AndMessage(error.Message);
    }

    [Fact]
    public async Task ToResult_WithMessage_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));
        var message = "error message";

        // Act (When)
        var result = await maybeTask.ToResult(message);

        // Assert (Then)
        result.ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ToResult_WithMessage_WithNone_ReturnsFailure()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.None<int>());
        var message = "error message";

        // Act (When)
        var result = await maybeTask.ToResult(message);

        // Assert (Then)
        result.ShouldBe()
            .Failure()
            .AndMessage(message);
    }
}
