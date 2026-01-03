using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Maps error reasons to HTTP status codes and response bodies.
/// </summary>
public interface IErrorHttpMapper
{
    /// <summary>
    ///     Gets the HTTP status code for the specified error.
    ///     Returns null if this mapper cannot handle the error type.
    /// </summary>
    /// <param name="error">The error to map.</param>
    /// <returns>HTTP status code or null if not handled.</returns>
    int? GetStatusCode(IError error);

    /// <summary>
    ///     Creates the response body for the specified error.
    ///     Returns null if this mapper cannot handle the error type.
    /// </summary>
    /// <param name="error">The error to map.</param>
    /// <returns>Response body object or null if not handled.</returns>
    object? GetResponseBody(IError error);
}
