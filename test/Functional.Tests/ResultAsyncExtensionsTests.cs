using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultAsyncExtensionsTests
{
    [Fact]
    public async Task ValueTaskResult_Pipeline_CanBindMapTapWithoutWrapper()
    {
        // Arrange (Given)
        ValueTask<Result<int>> start = ValueTask.FromResult(Result.Ok(2));
        var tapped = 0;

        // Act (When)
        var result = await start
            .Bind(v => ValueTask.FromResult(Result.Ok(v + 1)))
            .Map(v => v * 10)
            .Tap(v => tapped = v);

        // Assert (Then)
        result.ShouldBe().Success().And(v => Assert.Equal(30, v));
        Assert.Equal(30, tapped);
    }

    [Fact]
    public async Task TaskResult_Switch_UsesDirectAsyncExtensions()
    {
        // Arrange (Given)
        Task<Result<int>> start = Task.FromResult(Result.Ok(3));
        var output = "";

        // Act (When)
        await start.Switch(
            onSuccess: v => output = $"ok:{v}",
            onFailure: _ => output = "fail");

        // Assert (Then)
        Assert.Equal("ok:3", output);
    }
}
