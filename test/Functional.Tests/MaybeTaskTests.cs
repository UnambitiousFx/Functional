namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeTaskTests
{
    [Fact]
    public async Task ImplicitConversion_FromMaybe_CreatesMaybeTask()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        MaybeTask<int> maybeTask = maybe;
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsSome);
        Assert.True(result.Some(out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ImplicitConversion_FromValue_CreatesSomeMaybeTask()
    {
        // Arrange (Given)
        int value = 42;

        // Act (When)
        MaybeTask<int> maybeTask = value;
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsSome);
        Assert.True(result.Some(out var extractedValue));
        Assert.Equal(42, extractedValue);
    }

    [Fact]
    public async Task ImplicitConversion_FromNone_CreatesNoneMaybeTask()
    {
        // Arrange (Given)
        var maybe = Maybe<int>.None();

        // Act (When)
        MaybeTask<int> maybeTask = maybe;
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task ImplicitConversion_ToValueTask_ReturnsUnderlyingTask()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(maybe));

        // Act (When)
        ValueTask<Maybe<int>> valueTask = maybeTask;
        var result = await valueTask;

        // Assert (Then)
        Assert.True(result.IsSome);
        Assert.True(result.Some(out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task AsValueTask_ReturnsUnderlyingTask()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(maybe));

        // Act (When)
        var valueTask = maybeTask.AsValueTask();
        var result = await valueTask;

        // Assert (Then)
        Assert.True(result.IsSome);
        Assert.True(result.Some(out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task GetAwaiter_CanBeAwaited()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(maybe));

        // Act (When)
        var awaiter = maybeTask.GetAwaiter();
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsSome);
    }

    [Fact]
    public async Task Await_WithSome_ReturnsSome()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(42)));

        // Act (When)
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsSome);
        Assert.True(result.Some(out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task Await_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe<int>.None()));

        // Act (When)
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsNone);
    }

    [Fact]
    public async Task Constructor_WithValueTask_InitializesCorrectly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var maybeTask = new MaybeTask<int>(valueTask);
        var result = await maybeTask;

        // Assert (Then)
        Assert.True(result.IsSome);
        Assert.True(result.Some(out var value));
        Assert.Equal(42, value);
    }
}
