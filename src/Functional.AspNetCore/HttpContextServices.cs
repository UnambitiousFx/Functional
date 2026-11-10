using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using UnambitiousFx.Functional.AspNetCore.Mappers;

namespace UnambitiousFx.Functional.AspNetCore;

/// <summary>
///     Reads the services registered by <see cref="ServiceCollectionExtensions.AddResultHttp" />,
///     falling back to the defaults when nothing is registered.
/// </summary>
internal static class HttpContextServices
{
    internal static IFailureHttpMapper GetFailureMapper(this HttpContext httpContext)
    {
        return httpContext.RequestServices?.GetService<IFailureHttpMapper>() ?? DefaultFailureHttpMapper.Instance;
    }

    internal static ResultHttpAdapterPolicy GetAdapterPolicy(this HttpContext httpContext)
    {
        return httpContext.RequestServices?.GetService<ResultHttpAdapterPolicy>() ?? ResultHttpAdapterPolicy.Default;
    }
}
