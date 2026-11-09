using System.Net;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Default error-to-HTTP mapper with built-in mappings for standard error types.
/// </summary>
public class DefaultFailureHttpMapper : IFailureHttpMapper
{
    /// <summary>
    ///     Provides a singleton instance of the <see cref="DefaultFailureHttpMapper" /> class,
    ///     which implements default mappings for error reasons to HTTP status codes and response bodies.
    /// </summary>
    internal static IFailureHttpMapper Instance { get; } = new DefaultFailureHttpMapper();

    private readonly bool _includeExceptionDetails;

    /// <summary>
    ///     Initializes a new instance that hides failure details on 500 responses.
    /// </summary>
    public DefaultFailureHttpMapper()
        : this(false)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultFailureHttpMapper" /> class.
    /// </summary>
    /// <param name="includeExceptionDetails">
    ///     When <see langword="true" />, 500 responses include the failure message, and for an
    ///     <see cref="ExceptionalFailure" /> the exception type and stack trace. Enable only in development.
    /// </param>
    public DefaultFailureHttpMapper(bool includeExceptionDetails)
    {
        _includeExceptionDetails = includeExceptionDetails;
    }

    /// <inheritdoc />
    public virtual FailureHttpResponse GetFailureResponse(IFailure failure)
    {
        var problem = failure switch
        {
            ValidationFailure validation           => FromValidationError(validation),
            NotFoundFailure notFound               => FromNotFoundError(notFound),
            UnauthorizedFailure unauthorized       => FromUnauthorizedError(unauthorized),
            UnauthenticatedFailure unauthenticated => FromUnauthenticatedError(unauthenticated),
            ConflictFailure conflict               => FromConflictError(conflict),
            BadRequestFailure badRequest           => FromBadRequestError(badRequest),
            ExceptionalFailure exceptional         => FromExceptionalError(exceptional),
            _                                      => FromError(failure)
        };

        return new FailureHttpResponse
        {
            StatusCode = problem.Status ?? 500,
            Body       = problem
        };
    }

    private ProblemDetails FromError(IFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "An error occurred.",
            Detail = _includeExceptionDetails ? failure.Message : null,
            Status = (int)HttpStatusCode.InternalServerError,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
    }

    private static ProblemDetails FromConflictError(ConflictFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Conflict",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Conflict,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
        };
    }

    private ProblemDetails FromExceptionalError(ExceptionalFailure failure)
    {
        var details = new ProblemDetails
        {
            Title  = "Internal Server Error",
            Status = (int)HttpStatusCode.InternalServerError,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };
        if (_includeExceptionDetails) {
            details.Detail                      = failure.Message;
            details.Extensions["exceptionType"] = failure.Exception.GetType().FullName;
            details.Extensions["stackTrace"]    = failure.Exception.StackTrace;
        }

        return details;
    }

    private static ProblemDetails FromUnauthorizedError(UnauthorizedFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Unauthorized",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Unauthorized,
            Type   = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };
    }

    private static ProblemDetails FromUnauthenticatedError(UnauthenticatedFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Unauthorized",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Forbidden,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };
    }

    private static ProblemDetails FromNotFoundError(NotFoundFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Not Found",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.NotFound,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Extensions =
            {
                { "code", failure.Code },
                { "resource", failure.Resource },
                { "identifier", failure.Identifier }
            }
        };
    }

    private static ProblemDetails FromBadRequestError(BadRequestFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Bad Request",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.BadRequest,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
    }

    private static ProblemDetails FromValidationError(ValidationFailure failure)
    {
        var details = new ProblemDetails
        {
            Title  = "Validation Error",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.BadRequest,
            Type   = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions =
            {
                { "code", failure.Code }
            }
        };
        for (var i = 0; i < failure.Failures.Count; i++) {
            details.Extensions.Add($"failure[{i}]", failure.Failures[i]);
        }

        return details;
    }
}
