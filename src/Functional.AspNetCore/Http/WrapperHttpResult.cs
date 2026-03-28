using Microsoft.AspNetCore.Http;

namespace UnambitiousFx.Functional.AspNetCore.Http;

internal sealed class WrapperHttpResult : Microsoft.AspNetCore.Http.IResult
{
    private readonly string?                              _contentType;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly Microsoft.AspNetCore.Http.IResult    _inner;

    public WrapperHttpResult(Microsoft.AspNetCore.Http.IResult    inner,
                             IReadOnlyDictionary<string, string>? headers,
                             string?                              contentType = null)
    {
        _inner       = inner;
        _headers     = headers;
        _contentType = contentType;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        if (_headers is not null) {
            foreach (var (key, value) in _headers) {
                httpContext.Response.Headers[key] = value;
            }
        }

        if (_contentType is not null) {
            httpContext.Response.ContentType = _contentType;
        }

        return _inner.ExecuteAsync(httpContext);
    }
}
