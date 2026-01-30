namespace UnambitiousFx.Functional.Tests;

public sealed partial class MaybeTaskExtensionsTests
{
    [Fact]
    public async Task ValueOr_WithSome_ReturnsValue()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(9)));

        // Act (When)
        var value = await maybeTask.ValueOr(42);

        // Assert (Then)
        Assert.Equal(9, value);
    }

    [Fact]
    public async Task ValueOr_WithNone_ReturnsFallback()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.None<int>()));

        // Act (When)
        var value = await maybeTask.ValueOr(42);

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ValueOr_WithFactory_WithSome_DoesNotExecuteFactory()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(9)));
        var executed = false;

        // Act (When)
        var value = await maybeTask.ValueOr(() =>
        {
            executed = true;
            return 42;
        });

        // Assert (Then)
        Assert.Equal(9, value);
        Assert.False(executed);
    }

    [Fact]
    public async Task ValueOr_WithFactory_WithNone_ExecutesFactory()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.None<int>()));
        var executed = false;

        // Act (When)
        var value = await maybeTask.ValueOr(() =>
        {
            executed = true;
            return 42;
        });

        // Assert (Then)
        Assert.Equal(42, value);
        Assert.True(executed);
    }
}
