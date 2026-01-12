using UnambitiousFx.Functional.xunit.Tasks;
using UnambitiousFx.Functional.xunit.ValueTasks;

namespace UnambitiousFx.Functional.xunit.Tests;

public sealed class OneOfAssertionTests
{
    [Fact]
    public void OneOf_EnsureFirst_Chaining()
    {
        OneOf<int, string>.FromFirst(42)
            .ShouldBe()
            .First()
            .And(v => Assert.Equal(42, v))
            .Map(v => v.ToString())
            .And(v => Assert.Equal("42", v))
            .Where(v => v == "42");
    }

    [Fact]
    public void OneOf_EnsureSecond_Chaining()
    {
        OneOf<int, string>.FromSecond("hello")
            .ShouldBe()
            .Second()
            .And(v => Assert.Equal("hello", v))
            .Map(v => v.Length)
            .And(v => Assert.Equal(5, v))
            .Where(v => v == 5);
    }

    [Fact]
    public async Task Async_Task_OneOf_EnsureFirst()
    {
        await Task.FromResult(OneOf<int, string>.FromFirst(1))
            .ShouldBe()
            .First()
            .And(v => Assert.Equal(1, v))
            .Where(v => v == 1);
    }

    [Fact]
    public async Task Async_ValueTask_OneOf_EnsureSecond()
    {
        await ValueTask.FromResult(OneOf<int, string>.FromSecond("two"))
            .ShouldBe()
            .Second()
            .And(v => Assert.Equal("two", v))
            .Where(v => v == "two");
    }
}
