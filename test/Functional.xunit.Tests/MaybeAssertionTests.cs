using UnambitiousFx.Functional.xunit.ValueTasks;

namespace UnambitiousFx.Functional.xunit.Tests;

public sealed class MaybeAssertionTests
{
    [Fact]
    public void EnsureSome_Chaining()
    {
        Maybe<int>.Some(10)
            .ShouldBe()
            .Some()
            .And(v => Assert.Equal(10, v))
            .Map(v => v + 5)
            .And(v => Assert.Equal(15, v))
            .Where(v => v == 15);
    }

    [Fact]
    public void EnsureNone_Chaining()
    {
        Maybe<string>.None()
            .ShouldBe()
            .None();
    }

    [Fact]
    public async Task Async_ValueTask_EnsureSome()
    {
        await new ValueTask<Maybe<int>>(Maybe<int>.Some(7))
            .ShouldBe()
            .Some()
            .And(v => Assert.Equal(7, v));
    }

    [Fact]
    public async Task Async_ValueTask_EnsureNone()
    {
        await new ValueTask<Maybe<int>>(Maybe<int>.None())
            .ShouldBe()
            .None();
    }
}
