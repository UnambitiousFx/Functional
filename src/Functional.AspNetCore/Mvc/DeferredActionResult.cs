using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     An action result chosen at execution time, when the request services are available.
/// </summary>
internal sealed class DeferredActionResult : IActionResult
{
    private readonly Func<HttpContext, IActionResult> _resolve;

    public DeferredActionResult(Func<HttpContext, IActionResult> resolve)
    {
        _resolve = resolve;
    }

    internal IActionResult Resolve(HttpContext httpContext)
    {
        return _resolve(httpContext);
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        return _resolve(context.HttpContext)
           .ExecuteResultAsync(context);
    }
}
