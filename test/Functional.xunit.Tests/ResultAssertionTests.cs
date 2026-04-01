using UnambitiousFx.Functional.Failures;
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
        Result.Failure<int>(new Failure("some code", "boom"))
              .ShouldBe()
              .Failure()
              .And(error => { Assert.Equal("boom", error.Message); })
              .AndMessage("boom")
              .AndCode("some code");
    }

    [Fact]
    public async Task Async_ValueTask_Generic_EnsureSuccess()
    {
        var assertion = await new ValueTask<Result<int>>(Result.Success(5))
                             .ShouldBe()
                             .Success();
        assertion.And(v => Assert.Equal(5, v));
    }

    [Fact]
    public async Task Async_ValueTask_Generic_EnsureFailure()
    {
        await new ValueTask<Result<int>>(Result.Failure<int>(new Exception("x")))
             .ShouldBe()
             .Failure()
             .AndMessage("x");
    }

    [Fact]
    public void SuccessAssertion_Subject_ReturnsValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var assertion = result.ShouldBe().Success();

        // Assert (Then)
        Assert.Equal(42, assertion.Subject);
    }

    [Fact]
    public void SuccessAssertion_Deconstruct_ExtractsValue()
    {
        // Arrange (Given)
        var result = Result.Success(99);

        // Act (When)
        result.ShouldBe().Success().Deconstruct(out var value);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public void SuccessAssertion_Inspect_InvokesActionWithoutBreakingChain()
    {
        // Arrange (Given)
        var result   = Result.Success(7);
        var captured = 0;

        // Act (When)
        result.ShouldBe()
              .Success()
              .Inspect(v => captured = v)
              .And(v => Assert.Equal(7, v));

        // Assert (Then)
        Assert.Equal(7, captured);
    }

    [Fact]
    public void FailureAssertion_Subject_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("CODE", "message"));

        // Act (When)
        var assertion = result.ShouldBe().Failure();

        // Assert (Then)
        Assert.Equal("CODE", assertion.Subject.Code);
    }

    [Fact]
    public void FailureAssertion_Inspect_InvokesActionWithoutBreakingChain()
    {
        // Arrange (Given)
        var result   = Result.Failure(new Failure("ERR", "test error"));
        var captured = string.Empty;

        // Act (When)
        result.ShouldBe()
              .Failure()
              .Inspect(f => captured = f.Code)
              .AndCode("ERR");

        // Assert (Then)
        Assert.Equal("ERR", captured);
    }

    [Fact]
    public void NonGenericResult_Success_WhenFailure_ThrowsXunitException()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "error"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Success());
    }

    [Fact]
    public void NonGenericResult_Success_WhenFailure_WithBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "error"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Success("expected success"));
    }

    [Fact]
    public void NonGenericResult_Failure_WhenSuccess_ThrowsXunitException()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Failure());
    }

    [Fact]
    public void NonGenericResult_Failure_WhenSuccess_WithBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Failure("expected failure"));
    }

    [Fact]
    public void GenericResult_Success_WhenFailure_ThrowsXunitException()
    {
        // Arrange (Given)
        var result = Result.Failure<int>(new Failure("ERR", "not found"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Success());
    }

    [Fact]
    public void GenericResult_Success_WhenFailure_WithBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Failure<int>(new Failure("ERR", "not found"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Success("expected success"));
    }

    [Fact]
    public void GenericResult_Failure_WhenSuccess_ThrowsXunitException()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Failure());
    }

    [Fact]
    public void GenericResult_Failure_WhenSuccess_WithBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => result.ShouldBe().Failure("expected failure"));
    }

    [Fact]
    public void NonGenericResult_Success_WithBecauseParam_Succeeds()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When) / Assert (Then)
        result.ShouldBe().Success("should succeed");
    }

    [Fact]
    public void NonGenericResult_Failure_WithBecauseParam_Succeeds()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "msg"));

        // Act (When) / Assert (Then)
        result.ShouldBe().Failure("should be failure");
    }

    // --- Task<T> async overloads via ResultAssertionAsyncExtensions ---

    [Fact]
    public async Task Async_Task_NonGenericResult_EnsureSuccess()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success());

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe().Success();
    }

    [Fact]
    public async Task Async_Task_NonGenericResult_EnsureFailure()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Failure(new Failure("ERR", "msg")));

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe()
                        .Failure()
                        .AndMessage("msg");
    }

    [Fact]
    public async Task Async_Task_GenericResult_EnsureSuccess()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(5));

        // Act (When)
        var assertion = await resultTask.ShouldBe().Success();

        // Assert (Then)
        assertion.And(v => Assert.Equal(5, v));
    }

    [Fact]
    public async Task Async_Task_GenericResult_EnsureFailure()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Failure<int>(new Exception("task err")));

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe()
                        .Failure()
                        .AndMessage("task err");
    }

    [Fact]
    public async Task Async_Task_SuccessAssertion_And_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(8));

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe()
                        .Success()
                        .And(v => Assert.Equal(8, v));
    }

    [Fact]
    public async Task Async_Task_SuccessAssertion_Map_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = Task.FromResult(Result.Success(3));

        // Act (When)
        var result = await resultTask.ShouldBe()
                                     .Success()
                                     .Map(v => v * 2);

        // Assert (Then)
        Assert.Equal(6, result.Value);
    }

    [Fact]
    public async Task Async_ValueTask_NonGenericResult_EnsureSuccess()
    {
        // Arrange (Given)
        var resultTask = new ValueTask<Result>(Result.Success());

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe().Success();
    }

    [Fact]
    public async Task Async_ValueTask_NonGenericResult_EnsureFailure()
    {
        // Arrange (Given)
        var resultTask = new ValueTask<Result>(Result.Failure(new Failure("E", "fail")));

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe()
                        .Failure()
                        .AndMessage("fail");
    }

    [Fact]
    public async Task Async_ValueTask_SuccessAssertion_Map_TransformsValue()
    {
        // Arrange (Given)
        var resultTask = new ValueTask<Result<int>>(Result.Success(6));

        // Act (When)
        var result = await resultTask.ShouldBe()
                                     .Success()
                                     .Map(v => v + 1);

        // Assert (Then)
        Assert.Equal(7, result.Value);
    }

    [Fact]
    public async Task Async_ValueTask_SuccessAssertion_And_ExecutesAction()
    {
        // Arrange (Given)
        var resultTask = new ValueTask<Result<int>>(Result.Success(8));

        // Act (When) / Assert (Then)
        await resultTask.ShouldBe()
                        .Success()
                        .And(v => Assert.Equal(8, v));
    }
}
