using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using UnambitiousFx.Functional.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mvc;

public class ApiControllerClientErrorTests
{
    [Fact]
    public async Task AsActionResultBuilder_MaybeNoneInApiController_ReturnsProblemDetails()
    {
        // Arrange (Given)
        await using var app    = await StartAppAsync(suppressMapClientErrors: false);
        using var       client = app.GetTestClient();

        // Act (When)
        var response = await client.GetAsync("/maybe-none", TestContext.Current.CancellationToken);

        // Assert (Then)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }

    [Fact]
    public async Task AsActionResultBuilder_MaybeNoneWithSuppressedClientErrors_ReturnsEmptyNotFound()
    {
        // Arrange (Given)
        await using var app    = await StartAppAsync(suppressMapClientErrors: true);
        using var       client = app.GetTestClient();

        // Act (When)
        var response = await client.GetAsync("/maybe-none", TestContext.Current.CancellationToken);

        // Assert (Then)
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task AsActionResultBuilder_MaybeNoneWithNoContentPolicy_ReturnsEmptyNoContent()
    {
        // Arrange (Given)
        await using var app = await StartAppAsync(suppressMapClientErrors: false,
                                                  o => o.Policy = new ResultHttpAdapterPolicy
                                                  {
                                                      MaybeNoneBehavior = MaybeNoneHttpBehavior.NoContent
                                                  });
        using var client = app.GetTestClient();

        // Act (When)
        var response = await client.GetAsync("/maybe-none", TestContext.Current.CancellationToken);

        // Assert (Then)
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    private static async Task<WebApplication> StartAppAsync(bool                       suppressMapClientErrors,
                                                            Action<ResultHttpOptions>? configure = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddControllers()
               .AddApplicationPart(typeof(MaybeNoneController).Assembly)
               .ConfigureApiBehaviorOptions(o => o.SuppressMapClientErrors = suppressMapClientErrors);
        builder.Services.AddResultHttp(configure);

        var app = builder.Build();
        app.MapControllers();
        await app.StartAsync(TestContext.Current.CancellationToken);
        return app;
    }
}

[ApiController]
public sealed class MaybeNoneController : ControllerBase
{
    [HttpGet("/maybe-none")]
    public async Task<IActionResult> Get()
    {
        return await Maybe.None<int>().AsActionResultBuilder();
    }
}
