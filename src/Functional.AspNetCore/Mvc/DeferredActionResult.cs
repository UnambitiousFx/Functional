using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     An action result chosen at execution time, when the request services are available.
/// </summary>
/// <remarks>
///     MVC result filters run before this result executes, so they see the deferred wrapper rather than the
///     resolved result. To keep the behavior of <c>[ApiController]</c> endpoints, a resolved client error
///     (status 400 or above) is turned into a <see cref="ProblemDetails" /> response through the registered
///     <see cref="IClientErrorFactory" />, as MVC's client error result filter would have done.
/// </remarks>
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
        var result = _resolve(context.HttpContext);
        if (result is IClientErrorActionResult { StatusCode: >= StatusCodes.Status400BadRequest } clientError) {
            result = MapClientError(context, clientError);
        }

        return result.ExecuteResultAsync(context);
    }

    private static IActionResult MapClientError(ActionContext            context,
                                                IClientErrorActionResult clientError)
    {
        var services = context.HttpContext.RequestServices;
        if (services.GetService<IClientErrorFactory>() is not { } factory ||
            services.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value.SuppressMapClientErrors ||
            !context.ActionDescriptor.EndpointMetadata.Any(m => m is IApiBehaviorMetadata)) {
            return clientError;
        }

        return factory.GetClientError(context, clientError) ?? clientError;
    }
}
