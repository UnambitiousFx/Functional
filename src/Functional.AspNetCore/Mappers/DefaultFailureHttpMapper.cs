using System.Net;
using Microsoft.AspNetCore.Http;
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

    private readonly Dictionary<string, int>?    _codeStatuses;
    private readonly int                         _defaultStatusCode = (int)HttpStatusCode.InternalServerError;
    private readonly string[]                    _exposedMetadata   = [];
    private readonly bool                        _includeExceptionDetails;
    private readonly KeyValuePair<string, int>[] _prefixStatuses    = [];
    private readonly ValidationErrorsFormat      _validationErrorsFormat;
    private readonly int                         _validationStatusCode = StatusCodes.Status422UnprocessableEntity;

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
        : this(includeExceptionDetails, ValidationErrorsFormat.AspNetCore)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultFailureHttpMapper" /> class.
    /// </summary>
    /// <param name="includeExceptionDetails">
    ///     When <see langword="true" />, 500 responses include the failure message, and for an
    ///     <see cref="ExceptionalFailure" /> the exception type and stack trace. Enable only in development.
    /// </param>
    /// <param name="validationErrorsFormat">How the <c>errors</c> of a <see cref="ValidationFailure" /> are written.</param>
    public DefaultFailureHttpMapper(bool                   includeExceptionDetails,
                                    ValidationErrorsFormat validationErrorsFormat)
    {
        _includeExceptionDetails = includeExceptionDetails;
        _validationErrorsFormat  = validationErrorsFormat;
    }

    internal DefaultFailureHttpMapper(ResultHttpOptions options)
        : this(options.IncludeExceptionDetails, options.ValidationErrorsFormat)
    {
        _codeStatuses = options.CodeStatuses.Count > 0
                            ? new Dictionary<string, int>(options.CodeStatuses, StringComparer.Ordinal)
                            : null;
        _prefixStatuses    = options.PrefixStatuses.ToArray();
        _exposedMetadata   = options.ExposedMetadata.ToArray();
        _defaultStatusCode    = options.DefaultStatusCode;
        _validationStatusCode = options.ValidationStatusCode;
    }

    /// <inheritdoc />
    public virtual FailureHttpResponse GetFailureResponse(IFailure failure)
    {
        var mappedStatus = GetMappedStatus(failure.Code);
        var problem = failure switch
        {
            ValidationFailure validation           => FromValidationError(validation),
            NotFoundFailure notFound               => FromNotFoundError(notFound),
            UnauthorizedFailure unauthorized       => FromUnauthorizedError(unauthorized),
            UnauthenticatedFailure unauthenticated => FromUnauthenticatedError(unauthenticated),
            ConflictFailure conflict               => FromConflictError(conflict),
            BadRequestFailure badRequest           => FromBadRequestError(badRequest),
            ExceptionalFailure exceptional         => FromExceptionalError(exceptional),
            _                                      => FromError(failure, mappedStatus ?? _defaultStatusCode)
        };

        if (mappedStatus is { } status &&
            problem.Status != status) {
            problem.Status = status;
            problem.Type   = null; // the type URI described the built-in status
        }

        problem.Extensions.TryAdd("code", failure.Code);
        foreach (var key in _exposedMetadata) {
            if (failure.Metadata.TryGetValue(key, out var value)) {
                problem.Extensions.TryAdd(key, value);
            }
        }

        return new FailureHttpResponse
        {
            StatusCode = problem.Status ?? 500,
            Body       = problem
        };
    }

    private int? GetMappedStatus(string code)
    {
        if (_codeStatuses is not null &&
            _codeStatuses.TryGetValue(code, out var status)) {
            return status;
        }

        foreach (var (prefix, prefixStatus) in _prefixStatuses) {
            if (code.StartsWith(prefix, StringComparison.Ordinal)) {
                return prefixStatus;
            }
        }

        return null;
    }

    private ProblemDetails FromError(IFailure failure,
                                     int      statusCode)
    {
        var isServerError = statusCode >= (int)HttpStatusCode.InternalServerError;
        return new ProblemDetails
        {
            Title  = "An error occurred.",
            Detail = !isServerError || _includeExceptionDetails ? failure.Message : null,
            Status = statusCode,
            Type   = statusCode == (int)HttpStatusCode.InternalServerError
                         ? ProblemTypes.For(StatusCodes.Status500InternalServerError)
                         : null
        };
    }

    private static ProblemDetails FromConflictError(ConflictFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Conflict",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Conflict,
            Type   = ProblemTypes.For(StatusCodes.Status409Conflict)
        };
    }

    private ProblemDetails FromExceptionalError(ExceptionalFailure failure)
    {
        var details = new ProblemDetails
        {
            Title  = "Internal Server Error",
            Status = (int)HttpStatusCode.InternalServerError,
            Type   = ProblemTypes.For(StatusCodes.Status500InternalServerError)
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
            Title  = "Forbidden",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Forbidden,
            Type   = ProblemTypes.For(StatusCodes.Status403Forbidden)
        };
    }

    private static ProblemDetails FromUnauthenticatedError(UnauthenticatedFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Unauthorized",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.Unauthorized,
            Type   = ProblemTypes.For(StatusCodes.Status401Unauthorized)
        };
    }

    private static ProblemDetails FromNotFoundError(NotFoundFailure failure)
    {
        return new ProblemDetails
        {
            Title  = "Not Found",
            Detail = failure.Message,
            Status = (int)HttpStatusCode.NotFound,
            Type   = ProblemTypes.For(StatusCodes.Status404NotFound),
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
            Type   = ProblemTypes.For(StatusCodes.Status400BadRequest)
        };
    }

    private ProblemDetails FromValidationError(ValidationFailure failure)
    {
        var details = new ProblemDetails
        {
            Title  = "Validation Error",
            Detail = failure.Message,
            Status = _validationStatusCode,
            Type   = ProblemTypes.For(_validationStatusCode),
            Extensions =
            {
                { "code", failure.Code }
            }
        };
        if (_validationErrorsFormat == ValidationErrorsFormat.Rfc9457) {
            // RFC 9457 section 3: one entry per error, in order, locating its field with a JSON Pointer.
            details.Extensions["errors"] = failure.Errors.Select(ValidationProblemError.From).ToArray();
            return details;
        }

        // Same shape as ASP.NET Core ValidationProblemDetails; "" holds errors not tied to a field.
        details.Extensions["errors"] = failure.Errors
                                              .GroupBy(e => e.Field ?? string.Empty)
                                              .ToDictionary(g => g.Key, g => g.Select(e => e.Message).ToArray());
        var codes = failure.Errors
                           .Where(e => e.Code is not null)
                           .GroupBy(e => e.Field ?? string.Empty)
                           .ToDictionary(g => g.Key, g => g.Select(e => e.Code!).ToArray());
        if (codes.Count > 0) {
            details.Extensions["errorCodes"] = codes;
        }

        return details;
    }
}
