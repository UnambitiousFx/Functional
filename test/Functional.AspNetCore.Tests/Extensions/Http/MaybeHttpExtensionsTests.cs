using Microsoft.AspNetCore.Http.HttpResults;
using UnambitiousFx.Functional.AspNetCore.Http;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Http;

public class MaybeHttpExtensionsTests
{
    [Fact]
    public async Task AsHttpBuilder_WithSome_ReturnsOkWithValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder();

        // Assert (Then)
        var ok = Assert.IsType<Ok<int>>(UnwrapWrappedResult(httpResult));
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public async Task AsHttpBuilder_WithNone_ReturnsNotFoundStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder();

        // Assert (Then)
        Assert.IsType<NotFound>(UnwrapWrappedResult(httpResult));
    }

    [Fact]
    public async Task ValueTaskMaybe_AsHttpBuilder_WithSome_ReturnsOkWithValue()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var httpResult = await maybeTask.AsHttpBuilder();

        // Assert (Then)
        var ok = Assert.IsType<Ok<int>>(UnwrapWrappedResult(httpResult));
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public async Task AsHttpBuilder_AsCreated_WithSome_ReturnsCreatedWithLocationAndValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .AsCreated(id => $"/items/{id}");

        // Assert (Then)
        var created = Assert.IsType<Created<int>>(UnwrapWrappedResult(httpResult));
        Assert.Equal(42, created.Value);
        Assert.Equal("/items/42", created.Location);
    }

    [Fact]
    public async Task AsHttpBuilder_AsNone_WithNone_ReturnsNoContentStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .AsNone(() => Microsoft.AspNetCore.Http.Results.NoContent());

        // Assert (Then)
        Assert.IsType<NoContent>(UnwrapWrappedResult(httpResult));
    }

    private static Microsoft.AspNetCore.Http.IResult UnwrapWrappedResult(Microsoft.AspNetCore.Http.IResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as Microsoft.AspNetCore.Http.IResult ?? result;
    }
}
