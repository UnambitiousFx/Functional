using System.Net;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.Errors;

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
    public virtual (int StatusCode, object? Body)? GetResponse(IError error)
    {
        var problem = error switch
        {
            ValidationError validation => FromValidationError(validation),
            NotFoundError notFound => FromNotFoundError(notFound),
            UnauthorizedError unauthorized => FromUnauthorizedError(unauthorized),
            UnauthenticatedError unauthenticated => FromUnauthenticatedError(unauthenticated),
            ConflictError conflict => FromConflictError(conflict),
            ExceptionalError exceptional => FromExceptionalError(exceptional),
            _ => FromError(error)
        };

        return (problem.Status ?? 500, problem);
    }

    private static ProblemDetails FromError(IError error)
    {
        return new ProblemDetails
        {
            Title = "An error occurred.",
            Detail = error.Message,
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static ProblemDetails FromConflictError(ConflictError error)
    {
        return new ProblemDetails
        {
            Title = "Conflict",
            Detail = error.Message,
            Status = (int)HttpStatusCode.Conflict,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
        };
    }

    private static ProblemDetails FromExceptionalError(ExceptionalError error)
    {
        return new ProblemDetails
        {
            Title = "Internal Server Error",
            Detail = error.Message,
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static ProblemDetails FromUnauthorizedError(UnauthorizedError error)
    {
        return new ProblemDetails
        {
            Title = "Unauthorized",
            Detail = error.Message,
            Status = (int)HttpStatusCode.Unauthorized,
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };
    }

    private static ProblemDetails FromUnauthenticatedError(UnauthenticatedError error)
    {
        return new ProblemDetails
        {
            Title = "Unauthorized",
            Detail = error.Message,
            Status = (int)HttpStatusCode.Forbidden,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };
    }

    private static ProblemDetails FromNotFoundError(NotFoundError error)
    {
        return new ProblemDetails
        {
            Title = "Not Found",
            Detail = error.Message,
            Status = (int)HttpStatusCode.NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Extensions =
            {
                { "code", error.Code },
                { "resource", error.Resource },
                { "identifier", error.Identifier }
            }
        };
    }

    private static ProblemDetails FromValidationError(ValidationError error)
    {
        var details = new ProblemDetails
        {
            Title = "Validation Error",
            Detail = error.Message,
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions =
            {
                { "code", error.Code }
            }
        };
        for (var i = 0; i < error.Failures.Count; i++) details.Extensions.Add($"failure[{i}]", error.Failures[i]);
        return details;
    }
}