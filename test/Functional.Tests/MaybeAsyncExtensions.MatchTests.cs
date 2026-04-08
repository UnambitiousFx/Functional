using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsMatchTests {
    [Fact]
    public async Task ValueTaskMaybe_Match_WithSome_ReturnsSomeResult() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
                         x => x * 2,
                         () => 0);

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_Match_WithNone_ReturnsNoneResult() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
                         x => x * 2,
                         () => 0);

        // Assert (Then)
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_Match_WithAsyncFunctions_ReturnsSomeResult() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
                         async x => {
                             await Task.Delay(1);
                             return x * 2;
                         },
                         async () => {
                             await Task.Delay(1);
                             return 0;
                         });

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithSyncFunctions_MatchesSome() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
                         some: x => x * 2,
                         none: () => 0
                     );

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithSyncFunctions_MatchesNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
                         some: x => x * 2,
                         none: () => -1
                     );

        // Assert (Then)
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithAsyncFunctions_MatchesSome() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
                         some: async x => {
                             await Task.Delay(1);
                             return x * 2;
                         },
                         none: async () => {
                             await Task.Delay(1);
                             return 0;
                         }
                     );

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithAsyncFunctions_MatchesNone() {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
                         some: async x => {
                             await Task.Delay(1);
                             return x * 2;
                         },
                         none: async () => {
                             await Task.Delay(1);
                             return -1;
                         }
                     );

        // Assert (Then)
        Assert.Equal(-1, result);
    }
}
