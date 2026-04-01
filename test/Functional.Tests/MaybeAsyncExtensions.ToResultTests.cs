using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsToResultTests {
    [Fact]
    public async Task TaskMaybe_ToResult_UsesDirectAsyncExtensions() {
        // Arrange (Given)
        var start = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await start.ToResult(new ValidationFailure("missing"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithFactory_CreatesFailureFromFactory() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(() => new ValidationFailure("error"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithMessage_CreatesFailureFromMessage() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult("Value not found");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value not found");
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithAsyncFactory_CreatesFailureFromAsyncFactory() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(async () => {
            await Task.Delay(1);
            return new ValidationFailure("async error");
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithSome_ReturnsSuccess() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));
        var error = new Failure("ERROR", "Should not be used");

        // Act (When)
        var result = await maybe.ToResult(error);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithFactory_SomeDoesNotCallFactory() {
        // Arrange (Given)
        var maybe         = ValueTask.FromResult(Maybe.Some(42));
        var factoryCalled = false;

        // Act (When)
        var result = await maybe.ToResult(() => {
            factoryCalled = true;
            return new Failure("ERROR", "Should not be used");
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(factoryCalled);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithMessage_CreatesFailureFromMessage() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult("Value not found");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value not found");
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithAsyncFactory_CreatesFailureFromAsyncFactory() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(async () => {
            await Task.Delay(1);
            return new ValidationFailure("async error");
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithFactory_UsesFactory() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(() => new ValidationFailure("missing"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithSome_ReturnsSuccess() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ToResult(new Failure("ERROR", "Should not be used"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }
}
