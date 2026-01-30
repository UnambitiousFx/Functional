using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed partial class MaybeTaskExtensionsTests
{
    [Fact]
    public async Task Map_WithSome_ReturnsMappedSome()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.Some(5)));

        // Act (When)
        var mapped = await maybeTask.Map(value => value * 2);

        // Assert (Then)
        mapped.ShouldBe()
            .Some()
            .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task Map_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybeTask = new MaybeTask<int>(ValueTask.FromResult(Maybe.None<int>()));

        // Act (When)
        var mapped = await maybeTask.Map(value => value * 2);

        // Assert (Then)
        mapped.ShouldBe()
            .None();
    }
}
