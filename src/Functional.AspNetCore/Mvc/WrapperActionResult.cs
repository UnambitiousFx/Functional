using Microsoft.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

internal sealed class WrapperActionResult : IActionResult
{
    private readonly string?                              _contentType;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly IActionResult                        _inner;

    public WrapperActionResult(IActionResult                        inner,
                               IReadOnlyDictionary<string, string>? headers,
                               string?                              contentType = null)
    {
        _inner       = inner;
        _headers     = headers;
        _contentType = contentType;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        if (_headers is not null) {
            foreach (var (key, value) in _headers) {
                context.HttpContext.Response.Headers[key] = value;
            }
        }

        if (_contentType is not null) {
            context.HttpContext.Response.ContentType = _contentType;
        }

        return _inner.ExecuteResultAsync(context);
    }
}
