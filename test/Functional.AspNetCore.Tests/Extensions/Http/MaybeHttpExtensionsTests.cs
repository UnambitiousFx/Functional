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

    [Fact]
    public async Task AsHttpBuilder_WithHeader_CopiesExistingHeadersWhenChaining()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .WithHeader("X-First", "one")
                                    .WithHeader("X-Second", "two");
        var headers = GetWrappedHeaders(httpResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("one", headers!["X-First"]);
        Assert.Equal("two", headers["X-Second"]);
    }

    [Fact]
    public async Task AsHttpBuilder_WithStatusCode_OverridesDefaultStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .WithStatusCode(202);

        // Assert (Then)
        Assert.Equal(202, GetStatusCode(httpResult));
    }

    [Fact]
    public async Task AsHttpBuilder_WithResponseFormatter_TransformsResponseBody()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .WithResponseFormatter(v => v.ToString());

        // Assert (Then)
        var ok = Assert.IsType<Ok<string>>(UnwrapWrappedResult(httpResult));
        Assert.Equal("42", ok.Value);
    }

    [Fact]
    public async Task AsHttpBuilder_WithCreatedStatusCodeAndNoLocationFactory_ReturnsCreatedStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .WithStatusCode(201);

        // Assert (Then)
        Assert.Equal(201, GetStatusCode(httpResult));
    }

    [Fact]
    public async Task AsHttpBuilder_WithDefaultCustomStatusCode_ReturnsCustomStatusCode()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = await maybe.AsHttpBuilder()
                                    .WithStatusCode(418);

        // Assert (Then)
        Assert.Equal(418, GetStatusCode(httpResult));
    }

    private static int GetStatusCode(Microsoft.AspNetCore.Http.IResult result)
    {
        var inner = UnwrapWrappedResult(result);
        return inner switch
        {
            Microsoft.AspNetCore.Http.IStatusCodeHttpResult { StatusCode: { } statusCode } => statusCode,
            _ => throw new InvalidOperationException($"Cannot get status code from {inner.GetType().Name}")
        };
    }

    private static IReadOnlyDictionary<string, string>? GetWrappedHeaders(Microsoft.AspNetCore.Http.IResult result)
    {
        var headersField = result.GetType().GetField("_headers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return headersField?.GetValue(result) as IReadOnlyDictionary<string, string>;
    }

    private static Microsoft.AspNetCore.Http.IResult UnwrapWrappedResult(Microsoft.AspNetCore.Http.IResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as Microsoft.AspNetCore.Http.IResult ?? result;
    }
}
