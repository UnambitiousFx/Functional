using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class MaybeTaskExtensionsTests
{
    [Fact]
    public async Task BindAsync_WithSomeAndReturningSome_ReturnsSome()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(42)));

        // Act (When)
        var result = await maybeTask.Bind(v => Maybe.Some(v.ToString()));

        // Assert (Then)
        result.ShouldBe()
            .Some()
            .And(v => Assert.Equal("42", v));
    }

    [Fact]
    public async Task BindAsync_WithSomeAndReturningNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(42)));

        // Act (When)
        var result = await maybeTask.Bind(_ => Maybe.None<string>());

        // Assert (Then)
        result.ShouldBe()
            .None();
    }

    [Fact]
    public async Task BindAsync_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.None<int>()));
        var executed = false;

        // Act (When)
        var result = await maybeTask.Bind(v =>
        {
            executed = true;
            return Maybe.Some(v.ToString());
        });

        // Assert (Then)
        result.ShouldBe()
            .None();
        Assert.False(executed);
    }

    [Fact]
    public async Task Bind_AwaitableNonGenericResult_ToNonGenericResult_Success()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        var bound = await awaitableResult.Bind(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task Bind_AwaitableNonGenericResult_ToNonGenericResult_Failure()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure("Error").AsAsync();

        // Act (When)
        var bound = await awaitableResult.Bind(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Error");
    }

    [Fact]
    public async Task Bind_AwaitableNonGenericResult_ToGenericResult_Success()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        var bound = await awaitableResult.Bind(async () =>
        {
            await ValueTask.CompletedTask;
            return Result.Success(42);
        });

        // Assert (Then)
        bound.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task Bind_GenericResult_ToNonGenericResult_Success()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var bound = await result.AsAsync().Bind(async x =>
        {
            await ValueTask.CompletedTask;
            return x > 0 ? Result.Success() : Result.Failure("Invalid");
        });

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task Bind_AwaitableGenericResult_ToNonGenericResult_Success()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(5).AsAsync();

        // Act (When)
        var bound = await awaitableResult.Bind(async x =>
        {
            await ValueTask.CompletedTask;
            return x > 0 ? Result.Success() : Result.Failure("Invalid");
        });

        // Assert (Then)
        bound.ShouldBe().Success();
    }

    [Fact]
    public async Task Bind_AwaitableGenericResult_ToNonGenericResult_Failure()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var bound = await awaitableResult.Bind(async x =>
        {
            await ValueTask.CompletedTask;
            return Result.Success();
        });

        // Assert (Then)
        bound.ShouldBe().Failure().AndMessage("Error");
    }
}
