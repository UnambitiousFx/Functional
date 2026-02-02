using UnambitiousFx.Functional.AspNetCore.Mappers;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class ErrorHttpResponseTests
{
    [Fact(DisplayName = "ErrorHttpResponse can be created with required properties")]
    public void ErrorHttpResponse_WithRequiredProperties_CreatesSuccessfully()
    {
        // Arrange (Given) & Act (When)
        var response = new ErrorHttpResponse
        {
            StatusCode = 404,
            Body = new { error = "Not found" }
        };

        // Assert (Then)
        Assert.Equal(404, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.Null(response.Headers);
        Assert.Null(response.ContentType);
    }

    [Fact(DisplayName = "ErrorHttpResponse can include headers")]
    public void ErrorHttpResponse_WithHeaders_IncludesHeaders()
    {
        // Arrange (Given)
        var headers = new Dictionary<string, string>
        {
            { "WWW-Authenticate", "Bearer realm=\"example\"" },
            { "X-Custom-Header", "custom-value" }
        };

        // Act (When)
        var response = new ErrorHttpResponse
        {
            StatusCode = 401,
            Body = new { error = "Unauthorized" },
            Headers = headers
        };

        // Assert (Then)
        Assert.Equal(401, response.StatusCode);
        Assert.NotNull(response.Headers);
        Assert.Equal(2, response.Headers.Count);
        Assert.Equal("Bearer realm=\"example\"", response.Headers["WWW-Authenticate"]);
        Assert.Equal("custom-value", response.Headers["X-Custom-Header"]);
    }

    [Fact(DisplayName = "ErrorHttpResponse can include content type")]
    public void ErrorHttpResponse_WithContentType_IncludesContentType()
    {
        // Arrange (Given) & Act (When)
        var response = new ErrorHttpResponse
        {
            StatusCode = 400,
            Body = "<error>Bad Request</error>",
            ContentType = "application/xml"
        };

        // Assert (Then)
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("application/xml", response.ContentType);
    }

    [Fact(DisplayName = "ErrorHttpResponse with null body creates successfully")]
    public void ErrorHttpResponse_WithNullBody_CreatesSuccessfully()
    {
        // Arrange (Given) & Act (When)
        var response = new ErrorHttpResponse
        {
            StatusCode = 204
        };

        // Assert (Then)
        Assert.Equal(204, response.StatusCode);
        Assert.Null(response.Body);
    }

    [Fact(DisplayName = "ErrorHttpResponse with all properties creates successfully")]
    public void ErrorHttpResponse_WithAllProperties_CreatesSuccessfully()
    {
        // Arrange (Given)
        var headers = new Dictionary<string, string>
        {
            { "Retry-After", "120" }
        };

        // Act (When)
        var response = new ErrorHttpResponse
        {
            StatusCode = 429,
            Body = new { error = "Too many requests" },
            Headers = headers,
            ContentType = "application/json"
        };

        // Assert (Then)
        Assert.Equal(429, response.StatusCode);
        Assert.NotNull(response.Body);
        Assert.NotNull(response.Headers);
        Assert.Single(response.Headers);
        Assert.Equal("120", response.Headers["Retry-After"]);
        Assert.Equal("application/json", response.ContentType);
    }
}
