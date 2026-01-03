using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Maps errors to RFC 7807 Problem Details responses.
/// </summary>
public sealed class ProblemDetailsErrorMapper : IErrorHttpMapper
{
    private readonly bool _includeExceptionDetails;

    /// <summary>
    ///     Creates a Problem Details mapper.
    /// </summary>
    /// <param name="includeExceptionDetails">Whether to include exception stack traces in responses (for development).</param>
    public ProblemDetailsErrorMapper(bool includeExceptionDetails = false)
    {
        _includeExceptionDetails = includeExceptionDetails;
    }

    /// <inheritdoc />
    public int? GetStatusCode(IError error)
    {
        return error switch
        {
            ValidationError => StatusCodes.Status400BadRequest,
            NotFoundError => StatusCodes.Status404NotFound,
            UnauthorizedError => StatusCodes.Status401Unauthorized,
            ConflictError => StatusCodes.Status409Conflict,
            ExceptionalError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    /// <inheritdoc />
    public object? GetResponseBody(IError error)
    {
        var statusCode = GetStatusCode(error);
        if (!statusCode.HasValue)
        {
            return null;
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode.Value, Title = GetTitle(error), Detail = error.Message, Type = GetTypeUri(error)
        };

        // Add error code as extension
        problemDetails.Extensions["code"] = error.Code;

        // Add metadata as extensions
        foreach (var (key, value) in error.Metadata)
        {
            problemDetails.Extensions[key] = value;
        }

        // Handle validation errors specifically
        if (error is ValidationError validation)
        {
            problemDetails.Extensions["failures"] = validation.Failures;
        }

        // Handle not found errors specifically
        if (error is NotFoundError notFound)
        {
            problemDetails.Extensions["resource"] = notFound.Resource;
            problemDetails.Extensions["identifier"] = notFound.Identifier;
        }

        // Include exception details if requested
        if (_includeExceptionDetails && error is ExceptionalError exceptional)
        {
            problemDetails.Extensions["exception"] = new
            {
                type = exceptional.Exception.GetType().FullName,
                message = exceptional.Exception.Message,
                stackTrace = exceptional.Exception.StackTrace
            };
        }

        return problemDetails;
    }

    private static string GetTitle(IError error)
    {
        return error switch
        {
            ValidationError => "Validation Failed",
            NotFoundError => "Resource Not Found",
            UnauthorizedError => "Unauthorized",
            ConflictError => "Conflict",
            ExceptionalError => "Internal Server Error",
            _ => "Bad Request"
        };
    }

    private static string GetTypeUri(IError error)
    {
        return error switch
        {
            ValidationError => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            NotFoundError => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            UnauthorizedError => "https://tools.ietf.org/html/rfc7235#section-3.1",
            ConflictError => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            ExceptionalError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            _ => "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
    }
}
