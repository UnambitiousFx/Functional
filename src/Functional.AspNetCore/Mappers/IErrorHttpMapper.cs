using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Maps error reasons to HTTP status codes and response bodies.
/// </summary>
public interface IErrorHttpMapper
{
    /// <summary>
    ///     Creates a complete HTTP error response for the specified error, including status code, body, headers, and content type.
    ///     Returns null if this mapper cannot handle the error type.
    /// </summary>
    /// <param name="failure">The error to map.</param>
    /// <returns>An <see cref="ErrorHttpResponse"/> containing the complete HTTP response details, or null if not handled.</returns>
    ErrorHttpResponse? GetErrorResponse(IFailure failure);
}