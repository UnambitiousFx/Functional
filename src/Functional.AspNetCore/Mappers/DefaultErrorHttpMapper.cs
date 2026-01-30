using System.Net;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Default error-to-HTTP mapper with built-in mappings for standard error types.
/// </summary>
public class DefaultErrorHttpMapper : IErrorHttpMapper
{
    /// <summary>
    ///     Provides a singleton instance of the <see cref="DefaultErrorHttpMapper" /> class,
    ///     which implements default mappings for error reasons to HTTP status codes and response bodies.
    /// </summary>
    internal static IErrorHttpMapper Instance { get; } = new DefaultErrorHttpMapper();

    /// <inheritdoc />
    public virtual (int StatusCode, object? Body)? GetResponse(IFailure failure)
    {
        var problem = failure switch
        {
            ValidationFailure validation => FromValidationError(validation),
            NotFoundFailure notFound => FromNotFoundError(notFound),
            UnauthorizedFailure unauthorized => FromUnauthorizedError(unauthorized),
            UnauthenticatedFailure unauthenticated => FromUnauthenticatedError(unauthenticated),
            ConflictFailure conflict => FromConflictError(conflict),
            ExceptionalFailure exceptional => FromExceptionalError(exceptional),
            _ => FromError(failure)
        };

        return (problem.Status ?? 500, problem);
    }

    private static ProblemDetails FromError(IFailure failure)
    {
        return new ProblemDetails
        {
            Title = "An error occurred.",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static ProblemDetails FromConflictError(ConflictFailure failure)
    {
        return new ProblemDetails
        {
            Title = "Conflict",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Conflict,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
        };
    }

    private static ProblemDetails FromExceptionalError(ExceptionalFailure failure)
    {
        return new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static ProblemDetails FromUnauthorizedError(UnauthorizedFailure failure)
    {
        return new ProblemDetails
        {
            Title = "Unauthorized",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Unauthorized,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };
    }

    private static ProblemDetails FromUnauthenticatedError(UnauthenticatedFailure failure)
    {
        return new ProblemDetails
        {
            Title = "Unauthorized",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Forbidden,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };
    }

    private static ProblemDetails FromNotFoundError(NotFoundFailure failure)
    {
        return new ProblemDetails
        {
            Title = "Not Found",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Extensions =
            {
                { "code", failure.Code },
                { "resource", failure.Resource },
                { "identifier", failure.Identifier }
            }
        };
    }

    private static ProblemDetails FromValidationError(ValidationFailure failure)
    {
        var details = new ProblemDetails
        {
            Title = "Validation Error",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions =
            {
                { "code", failure.Code }
            }
        };
        for (var i = 0; i < failure.Failures.Count; i++) details.Extensions.Add($"failure[{i}]", failure.Failures[i]);
        return details;
    }
}