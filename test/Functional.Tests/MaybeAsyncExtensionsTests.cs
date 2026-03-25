using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsTests
{
    [Fact]
    public async Task ValueTaskMaybe_Pipeline_CanBindMapTapSomeTapNoneWithoutWrapper()
    {
        // Arrange (Given)
        ValueTask<Maybe<int>> start = ValueTask.FromResult(Maybe.Some(2));
        var seen = 0;
        var noneHit = false;

        // Act (When)
        var maybe = await start
            .Bind(v => ValueTask.FromResult(Maybe.Some(v + 1)))
            .Map(v => v * 10)
            .TapSome(v => seen = v)
            .TapNone(() => noneHit = true);

        // Assert (Then)
        maybe.ShouldBe().Some().And(v => Assert.Equal(30, v));
        Assert.Equal(30, seen);
        Assert.False(noneHit);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_UsesDirectAsyncExtensions()
    {
        // Arrange (Given)
        Task<Maybe<int>> start = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await start.ToResult(new ValidationFailure("missing"));

        // Assert (Then)
        result.ShouldBe().Failure().AndCode(ErrorCodes.Validation);
    }
}
