using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc;

public class MaybeHttpExtensionsTests
{
    [Fact]
    public void ToActionResult_WithSome_ReturnsOkObjectResult()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = maybe.ToActionResult();

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public void ToActionResult_WithNone_ReturnsNotFoundResult()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var actionResult = maybe.ToActionResult();

        // Assert (Then)
        Assert.IsType<NotFoundResult>(actionResult);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToActionResult_WithSome_ReturnsOkObjectResult()
    {
        // Arrange (Given)
        ValueTask<Maybe<int>> maybeTask = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var actionResult = await maybeTask.ToActionResult();

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public void ToActionResult_WithCreatedMapper_WithSome_ReturnsCreatedAtActionResult()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = maybe.ToActionResult(v => new CreatedAtActionResult("GetItem", null, new { id = v }, v));

        // Assert (Then)
        var created = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetItem", created.ActionName);
        Assert.Equal(42, created.Value);
    }

    [Fact]
    public void ToActionResult_WithNoneAndNoContentPolicy_ReturnsNoContentResult()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var policy = new ResultHttpAdapterPolicy
        {
            MaybeNoneBehavior = MaybeNoneHttpBehavior.NoContent
        };

        // Act (When)
        var actionResult = maybe.ToActionResult(policy: policy);

        // Assert (Then)
        Assert.IsType<NoContentResult>(actionResult);
    }
}