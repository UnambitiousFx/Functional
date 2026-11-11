using Microsoft.AspNetCore.Http;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     An HTTP result chosen at execution time, when the request services are available.
/// </summary>
internal sealed class DeferredHttpResult : Microsoft.AspNetCore.Http.IResult
{
    private readonly Func<HttpContext, Microsoft.AspNetCore.Http.IResult> _resolve;

    public DeferredHttpResult(Func<HttpContext, Microsoft.AspNetCore.Http.IResult> resolve)
    {
        _resolve = resolve;
    }

    internal Microsoft.AspNetCore.Http.IResult Resolve(HttpContext httpContext)
    {
        return _resolve(httpContext);
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _resolve(httpContext)
           .ExecuteAsync(httpContext);
    }
}
