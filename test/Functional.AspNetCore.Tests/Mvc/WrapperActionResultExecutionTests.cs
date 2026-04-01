using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mvc;

public class WrapperActionResultExecutionTests
{
    [Fact]
    public async Task ExecuteResultAsync_WithHeaders_SetsHeadersOnResponse()
    {
        // Arrange (Given)
        var inner         = new SimpleStatusActionResult(204);
        var headers       = new Dictionary<string, string> { ["X-Custom"] = "value" };
        var wrapper       = new WrapperActionResult(inner, headers);
        var httpContext   = new DefaultHttpContext();
        var actionContext = new ActionContext { HttpContext = httpContext };

        // Act (When)
        await wrapper.ExecuteResultAsync(actionContext);

        // Assert (Then)
        Assert.Equal("value", httpContext.Response.Headers["X-Custom"].ToString());
        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task ExecuteResultAsync_WithMultipleHeaders_SetsAllHeadersOnResponse()
    {
        // Arrange (Given)
        var inner   = new SimpleStatusActionResult(204);
        var headers = new Dictionary<string, string>
        {
            ["X-Custom-A"] = "aaa",
            ["X-Custom-B"] = "bbb"
        };
        var wrapper       = new WrapperActionResult(inner, headers);
        var httpContext   = new DefaultHttpContext();
        var actionContext = new ActionContext { HttpContext = httpContext };

        // Act (When)
        await wrapper.ExecuteResultAsync(actionContext);

        // Assert (Then)
        Assert.Equal("aaa", httpContext.Response.Headers["X-Custom-A"].ToString());
        Assert.Equal("bbb", httpContext.Response.Headers["X-Custom-B"].ToString());
    }

    [Fact]
    public async Task ExecuteResultAsync_WithContentType_SetsContentTypeOnResponse()
    {
        // Arrange (Given)
        var inner         = new SimpleStatusActionResult(400);
        var wrapper       = new WrapperActionResult(inner, null, "application/json");
        var httpContext   = new DefaultHttpContext();
        var actionContext = new ActionContext { HttpContext = httpContext };

        // Act (When)
        await wrapper.ExecuteResultAsync(actionContext);

        // Assert (Then)
        Assert.Equal("application/json", httpContext.Response.ContentType);
    }

    [Fact]
    public async Task ExecuteResultAsync_WithNoHeadersAndNoContentType_ExecutesInnerResult()
    {
        // Arrange (Given)
        var inner         = new SimpleStatusActionResult(204);
        var wrapper       = new WrapperActionResult(inner, null);
        var httpContext   = new DefaultHttpContext();
        var actionContext = new ActionContext { HttpContext = httpContext };

        // Act (When)
        await wrapper.ExecuteResultAsync(actionContext);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status204NoContent, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task ExecuteResultAsync_WithHeadersAndContentType_SetsBothOnResponse()
    {
        // Arrange (Given)
        var inner         = new SimpleStatusActionResult(204);
        var headers       = new Dictionary<string, string> { ["X-Trace"] = "trace-id" };
        var wrapper       = new WrapperActionResult(inner, headers, "text/plain");
        var httpContext   = new DefaultHttpContext();
        var actionContext = new ActionContext { HttpContext = httpContext };

        // Act (When)
        await wrapper.ExecuteResultAsync(actionContext);

        // Assert (Then)
        Assert.Equal("trace-id", httpContext.Response.Headers["X-Trace"].ToString());
        Assert.Equal("text/plain", httpContext.Response.ContentType);
    }

    private sealed class SimpleStatusActionResult(int statusCode) : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = statusCode;
            return Task.CompletedTask;
        }
    }
}
