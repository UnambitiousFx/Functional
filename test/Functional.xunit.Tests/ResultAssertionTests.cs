using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit.Tasks;
using UnambitiousFx.Functional.xunit.ValueTasks;

namespace UnambitiousFx.Functional.xunit.Tests;

public sealed class ResultAssertionTests
{
    [Fact]
    public void NonGenericResult_EnsureSuccess_Chaining()
    {
        Result.Success()
            .ShouldBe()
            .Success();
    }

    [Fact]
    public void GenericResult_EnsureSuccess_Chaining()
    {
        Result.Success(42)
            .ShouldBe()
            .Success()
            .And(v => Assert.Equal(42, v))
            .Map(v => v + 1)
            .And(v => Assert.Equal(43, v));
    }

    [Fact]
    public void GenericResult_ToResult_Chaining()
    {
        var r2 = Result.Success(10)
            .ShouldBe()
            .Success()
            .ToResult(v => v * 2);
        r2.ShouldBe()
            .Success()
            .Where(doubled => doubled == 20);
    }

    [Fact]
    public void GenericResult_EnsureFailure_Chaining()
    {
        Result.Failure<int>(new Error("some code", "boom"))
            .ShouldBe()
            .Failure()
            .And(error => { Assert.Equal("boom", error.Message); })
            .AndMessage("boom")
            .AndCode("some code");
    }


    [Fact]
    public async Task Async_Task_Generic_EnsureSuccess()
    {
        var assertion = await Task.FromResult(Result.Success(5))
            .ShouldBe()
            .Success();
        assertion.And(v => Assert.Equal(5, v));
    }

    [Fact]
    public async Task Async_Task_Generic_EnsureFailure()
    {
        await Task.FromResult(Result.Failure<int>(new Exception("x")))
            .ShouldBe()
            .Failure()
            .AndMessage("x");
    }

    [Fact]
    public async Task Async_ValueTask_Generic_EnsureSuccess()
    {
        await  ValueTask.FromResult(Result.Success(9))
            .ShouldBe()
            .Success()
            .And(v => Assert.Equal(9, v));
    }
}
