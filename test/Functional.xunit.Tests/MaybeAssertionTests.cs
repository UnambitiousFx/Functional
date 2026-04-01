using UnambitiousFx.Functional.xunit.ValueTasks;

namespace UnambitiousFx.Functional.xunit.Tests;

public sealed class MaybeAssertionTests
{
    [Fact]
    public void EnsureSome_Chaining()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.Some(10);

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .And(v => Assert.Equal(10, v))
             .Map(v => v + 5)
             .And(v => Assert.Equal(15, v))
             .Where(v => v == 15);
    }

    [Fact]
    public void EnsureNone_Chaining()
    {
        // Arrange (Given)
        var maybe = Maybe<string>.None();

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .None();
    }

    [Fact]
    public async Task Async_ValueTask_EnsureSome()
    {
        // Arrange (Given)
        var maybeTask = new ValueTask<Maybe<int>>(Maybe<int>.Some(7));

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe()
                       .Some()
                       .And(v => Assert.Equal(7, v));
    }

    [Fact]
    public async Task Async_ValueTask_EnsureNone()
    {
        // Arrange (Given)
        var maybeTask = new ValueTask<Maybe<int>>(Maybe<int>.None());

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe()
                       .None();
    }

    [Fact]
    public void SomeAssertion_Subject_ReturnsValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var assertion = maybe.ShouldBe().Some();

        // Assert (Then)
        Assert.Equal(42, assertion.Subject);
    }

    [Fact]
    public void SomeAssertion_Deconstruct_ExtractsValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(99);

        // Act (When)
        maybe.ShouldBe().Some().Deconstruct(out var value);

        // Assert (Then)
        Assert.Equal(99, value);
    }

    [Fact]
    public void SomeAssertion_Inspect_InvokesActionWithoutBreakingChain()
    {
        // Arrange (Given)
        var maybe    = Maybe.Some(7);
        var captured = 0;

        // Act (When)
        maybe.ShouldBe()
             .Some()
             .Inspect(v => captured = v)
             .And(v => Assert.Equal(7, v));

        // Assert (Then)
        Assert.Equal(7, captured);
    }

    [Fact]
    public void Some_WithBecauseParam_WhenSome_Succeeds()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(5);

        // Act (When) / Assert (Then)
        maybe.ShouldBe().Some("should be some");
    }

    [Fact]
    public void None_WithBecauseParam_WhenNone_Succeeds()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When) / Assert (Then)
        maybe.ShouldBe().None("should be none");
    }

    [Fact]
    public void None_WhenIsSome_IncludesValueInFailureMessage()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().None());
    }

    [Fact]
    public void None_WhenIsSome_WithBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().None("expected none"));
    }

    [Fact]
    public void Some_WhenIsNone_FailsWithXunitException()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().Some());
    }

    [Fact]
    public void Some_WhenIsNone_WithBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().Some("expected some"));
    }

    // --- Task<T> async overloads via MaybeAssertionAsyncExtensions ---

    [Fact]
    public async Task Async_Task_ShouldBe_Some_ReturnsAssertion()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(10));

        // Act (When)
        var assertion = await maybeTask.ShouldBe();

        // Assert (Then)
        assertion.Some().And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Async_Task_Some_ReturnsValue()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(3));

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe()
                       .Some()
                       .And(v => Assert.Equal(3, v));
    }

    [Fact]
    public async Task Async_Task_None_ReturnsNoneAssertion()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.None<string>());

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe().None();
    }

    [Fact]
    public async Task Async_Task_SomeAssertion_Map_TransformsValue()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(4));

        // Act (When)
        var result = await maybeTask.ShouldBe()
                                    .Some()
                                    .Map(v => v * 3);

        // Assert (Then)
        Assert.Equal(12, result.Value);
    }

    [Fact]
    public async Task Async_ValueTask_SomeAssertion_Map_TransformsValue()
    {
        // Arrange (Given)
        var maybeTask = new ValueTask<Maybe<int>>(Maybe.Some(5));

        // Act (When)
        var result = await maybeTask.ShouldBe()
                                    .Some()
                                    .Map(v => v * 3);

        // Assert (Then)
        Assert.Equal(15, result.Value);
    }
}
