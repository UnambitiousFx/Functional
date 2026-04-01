using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UnambitiousFx.Functional.AspNetCore.Http;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Http;

public class WrapperHttpResultExecutionTests
{
    private static DefaultHttpContext CreateHttpContext()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var context = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
        return context;
    }

    [Fact]
    public async Task ExecuteAsync_WithHeaders_SetsHeadersOnResponse()
    {
        // Arrange (Given)
        var inner   = new SimpleStatusResult(204);
        var headers = new Dictionary<string, string> { ["X-Custom"] = "value" };
        var wrapper = new WrapperHttpResult(inner, headers);
        var context = CreateHttpContext();

        // Act (When)
        await wrapper.ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal("value", context.Response.Headers["X-Custom"].ToString());
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleHeaders_SetsAllHeadersOnResponse()
    {
        // Arrange (Given)
        var inner = new SimpleStatusResult(204);
        var headers = new Dictionary<string, string>
        {
            ["X-Custom-A"] = "aaa",
            ["X-Custom-B"] = "bbb"
        };
        var wrapper = new WrapperHttpResult(inner, headers);
        var context = CreateHttpContext();

        // Act (When)
        await wrapper.ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal("aaa", context.Response.Headers["X-Custom-A"].ToString());
        Assert.Equal("bbb", context.Response.Headers["X-Custom-B"].ToString());
    }

    [Fact]
    public async Task ExecuteAsync_WithContentType_SetsContentTypeOnResponse()
    {
        // Arrange (Given)
        var inner   = new SimpleStatusResult(400);
        var wrapper = new WrapperHttpResult(inner, null, "application/json");
        var context = CreateHttpContext();

        // Act (When)
        await wrapper.ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal("application/json", context.Response.ContentType);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoHeadersAndNoContentType_ExecutesInnerResult()
    {
        // Arrange (Given)
        var inner   = new SimpleStatusResult(204);
        var wrapper = new WrapperHttpResult(inner, null);
        var context = CreateHttpContext();

        // Act (When)
        await wrapper.ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_WithHeadersAndContentType_SetsBothOnResponse()
    {
        // Arrange (Given)
        var inner   = new SimpleStatusResult(204);
        var headers = new Dictionary<string, string> { ["X-Trace"] = "trace-id" };
        var wrapper = new WrapperHttpResult(inner, headers, "text/plain");
        var context = CreateHttpContext();

        // Act (When)
        await wrapper.ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal("trace-id", context.Response.Headers["X-Trace"].ToString());
        Assert.Equal("text/plain", context.Response.ContentType);
    }

    private sealed class SimpleStatusResult(int statusCode) : Microsoft.AspNetCore.Http.IResult
    {
        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = statusCode;
            return Task.CompletedTask;
        }
    }
}
