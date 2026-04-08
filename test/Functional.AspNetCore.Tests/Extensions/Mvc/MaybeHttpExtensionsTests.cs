using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc;

public class MaybeHttpExtensionsTests
{
    [Fact]
    public async Task AsActionResultBuilder_WithSome_ReturnsOkObjectResultWithValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder();

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithNone_ReturnsNotFoundStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder();

        // Assert (Then)
        Assert.IsType<NotFoundResult>(UnwrapWrappedResult(actionResult));
    }

    [Fact]
    public async Task ValueTaskMaybe_AsActionResultBuilder_WithSome_ReturnsOkObjectResultWithValue()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var actionResult = await maybeTask.AsActionResultBuilder();

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_AsCreated_WithSome_ReturnsCreatedResultWithLocationAndValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .AsCreated(v => $"/items/{v}");

        // Assert (Then)
        var created = Assert.IsType<CreatedResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal("/items/42", created.Location);
        Assert.Equal(42, created.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_AsNone_WithNone_ReturnsNoContentStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .AsNone(() => new NoContentResult());

        // Assert (Then)
        Assert.IsType<NoContentResult>(UnwrapWrappedResult(actionResult));
    }

    private static IActionResult UnwrapWrappedResult(IActionResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as IActionResult ?? result;
    }
}
