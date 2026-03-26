using Microsoft.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Internal;

internal static class FailureHttpResponseResolver
{
    public static FailureHttpResponse Resolve(Failure           failure,
                                            IFailureHttpMapper? customMapper)
    {
        ArgumentNullException.ThrowIfNull(failure);

        return customMapper?.GetFailureResponse(failure) ??
               DefaultFailureHttpMapper.Instance.GetFailureResponse(failure) ?? new FailureHttpResponse { StatusCode = StatusCodes.Status500InternalServerError };
    }
}
