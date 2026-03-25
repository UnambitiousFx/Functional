using Microsoft.AspNetCore.Http.HttpResults;
using UnambitiousFx.Functional.AspNetCore.Http;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Http;

public class MaybeHttpExtensionsTests
{
    [Fact]
    public void ToHttpResult_WithSome_ReturnsOk()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = maybe.ToHttpResult();

        // Assert (Then)
        var ok = Assert.IsType<Ok<int>>(httpResult);
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public void ToHttpResult_WithNone_ReturnsNotFound()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var httpResult = maybe.ToHttpResult();

        // Assert (Then)
        Assert.IsType<NotFound>(httpResult);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToHttpResult_WithSome_ReturnsOk()
    {
        // Arrange (Given)
        var maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var httpResult = await maybeTask.ToHttpResult();

        // Assert (Then)
        var ok = Assert.IsType<Ok<int>>(httpResult);
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public void ToCreatedHttpResult_WithSome_ReturnsCreated()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var httpResult = maybe.ToCreatedHttpResult(id => $"/items/{id}");

        // Assert (Then)
        var created = Assert.IsType<Created<int>>(httpResult);
        Assert.Equal(42,          created.Value);
        Assert.Equal("/items/42", created.Location);
    }

    [Fact]
    public void ToHttpResult_WithNoneAndNoContentPolicy_ReturnsNoContent()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var policy = new ResultHttpAdapterPolicy
        {
            MaybeNoneBehavior = MaybeNoneHttpBehavior.NoContent
        };

        // Act (When)
        var httpResult = maybe.ToHttpResult(policy: policy);

        // Assert (Then)
        Assert.IsType<NoContent>(httpResult);
    }
}
