using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsFilterTests {
    [Fact]
    public async Task ValueTaskMaybe_Filter_WithPredicateTrue_ReturnsSome() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(x => x > 40);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithPredicateFalse_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(x => x > 50);

        // Assert (Then)
        result.ShouldBe()
              .None();
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithAsyncPredicate_FiltersCorrectly() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x => {
            await Task.Delay(1);
            return x > 40;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithAsyncPredicateFalse_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x => {
            await Task.Delay(1);
            return x > 50;
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithNone_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Filter(x => x > 0);

        // Assert (Then)
        result.ShouldBe()
              .None();
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithPredicate_FiltersCorrectly() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(x => x > 40);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithAsyncPredicate_FiltersCorrectly() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x => {
            await Task.Delay(1);
            return x > 40;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithAsyncPredicateFalse_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x => {
            await Task.Delay(1);
            return x > 50;
        });

        // Assert (Then)
        result.ShouldBe()
              .None();
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithNone_ReturnsNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Filter(x => x > 0);

        // Assert (Then)
        result.ShouldBe()
              .None();
    }
}
