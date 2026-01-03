using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Default error-to-HTTP mapper with built-in mappings for standard error types.
/// </summary>
public sealed class DefaultErrorHttpMapper : IErrorHttpMapper
{
    /// <summary>
    ///     Provides a singleton instance of the <see cref="DefaultErrorHttpMapper" /> class,
    ///     which implements default mappings for error reasons to HTTP status codes and response bodies.
    /// </summary>
    public static DefaultErrorHttpMapper Instance { get; } = new();

    /// <inheritdoc />
    public int? GetStatusCode(IError error)
    {
        return error switch
        {
            ValidationError => 400,
            NotFoundError => 404,
            UnauthorizedError => 401,
            ConflictError => 409,
            ExceptionalError => 500,
            _ => 400
        };
    }

    /// <inheritdoc />
    public object GetResponseBody(IError error)
    {
        return error switch
        {
            ValidationError validation => new
            {
                error = validation.Code, message = validation.Message, failures = validation.Failures
            },
            NotFoundError notFound => new
            {
                error = notFound.Code,
                message = notFound.Message,
                resource = notFound.Resource,
                identifier = notFound.Identifier
            },
            UnauthorizedError unauthorized => new { error = unauthorized.Code, message = unauthorized.Message },
            ConflictError conflict => new { error = conflict.Code, message = conflict.Message },
            ExceptionalError exceptional => new { error = exceptional.Code, message = exceptional.Message },
            _ => new { error = error.Code, message = error.Message }
        };
    }
}
