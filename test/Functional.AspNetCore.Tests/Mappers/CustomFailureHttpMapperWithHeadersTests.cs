using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class CustomErrorHttpMapperWithHeadersTests
{
    [Fact(DisplayName = "Custom mapper can return 401 with WWW-Authenticate header")]
    public void GetErrorResponse_UnauthorizedFailure_ReturnsWithWwwAuthenticateHeader()
    {
        // Arrange (Given)
        var mapper  = new UnauthorizedWithWwwAuthenticateMapper();
        var failure = new UnauthorizedFailure("Invalid token");

        // Act (When)
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(401, response.StatusCode);
        Assert.NotNull(response.Headers);
        Assert.Single(response.Headers);
        Assert.Equal("Bearer realm=\"api\"", response.Headers["WWW-Authenticate"]);
    }

    [Fact(DisplayName = "Custom mapper can return 429 with rate limit headers")]
    public void GetErrorResponse_RateLimitFailure_ReturnsWithRateLimitHeaders()
    {
        // Arrange (Given)
        var mapper  = new TooManyRequestsMapper();
        var failure = new Failure("RATE_LIMIT", "Too many requests");

        // Act (When)
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(429, response.StatusCode);
        Assert.NotNull(response.Headers);
        Assert.Equal(3,     response.Headers.Count);
        Assert.Equal("60",  response.Headers["Retry-After"]);
        Assert.Equal("100", response.Headers["X-RateLimit-Limit"]);
        Assert.Equal("0",   response.Headers["X-RateLimit-Remaining"]);
    }

    [Fact(DisplayName = "Custom mapper returns null for unhandled failure types")]
    public void GetErrorResponse_UnhandledFailure_ReturnsNull()
    {
        // Arrange (Given)
        var mapper  = new UnauthorizedWithWwwAuthenticateMapper();
        var failure = new ValidationFailure(["Field is required"]);

        // Act (When)
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.Null(response);
    }

    [Fact(DisplayName = "CompositeFailureHttpMapper preserves headers from custom mapper")]
    public void CompositeMapper_WithCustomMapperReturningHeaders_PreservesHeaders()
    {
        // Arrange (Given)
        var customMapper    = new UnauthorizedWithWwwAuthenticateMapper();
        var defaultMapper   = new DefaultFailureHttpMapper();
        var compositeMapper = new CompositeFailureHttpMapper(customMapper, defaultMapper);
        var failure         = new UnauthorizedFailure("Invalid token");

        // Act (When)
        var response = compositeMapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(401, response.StatusCode);
        Assert.NotNull(response.Headers);
        Assert.Single(response.Headers);
        Assert.Equal("Bearer realm=\"api\"", response.Headers["WWW-Authenticate"]);
    }

    private class UnauthorizedWithWwwAuthenticateMapper : IFailureHttpMapper
    {
        public FailureHttpResponse? GetFailureResponse(IFailure failure)
        {
            if (failure is UnauthorizedFailure unauthorized) {
                return new FailureHttpResponse
                {
                    StatusCode = 401,
                    Body       = new { error = unauthorized.Message },
                    Headers = new Dictionary<string, string>
                    {
                        { "WWW-Authenticate", "Bearer realm=\"api\"" }
                    }
                };
            }

            return null;
        }
    }

    private class TooManyRequestsMapper : IFailureHttpMapper
    {
        public FailureHttpResponse? GetFailureResponse(IFailure failure)
        {
            if (failure.Code == "RATE_LIMIT") {
                return new FailureHttpResponse
                {
                    StatusCode = 429,
                    Body       = new { error = failure.Message },
                    Headers = new Dictionary<string, string>
                    {
                        { "Retry-After", "60" },
                        { "X-RateLimit-Limit", "100" },
                        { "X-RateLimit-Remaining", "0" }
                    }
                };
            }

            return null;
        }
    }
}
