using Microsoft.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Internal;

internal static class FailureHttpResponseResolver
{
    public static ErrorHttpResponse Resolve(Failure failure, IErrorHttpMapper? customMapper)
    {
        ArgumentNullException.ThrowIfNull(failure);

        return customMapper?.GetErrorResponse(failure)
               ?? DefaultErrorHttpMapper.Instance.GetErrorResponse(failure)
               ?? new ErrorHttpResponse { StatusCode = StatusCodes.Status500InternalServerError };
    }
}