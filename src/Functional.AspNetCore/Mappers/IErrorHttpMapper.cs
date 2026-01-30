using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Maps error reasons to HTTP status codes and response bodies.
/// </summary>
public interface IErrorHttpMapper
{
    /// <summary>
    ///     Creates the response body for the specified error.
    ///     Returns null if this mapper cannot handle the error type.
    /// </summary>
    /// <param name="failure">The error to map.</param>
    /// <returns>A tuple containing the HTTP status code and the response body, or null if not handled.</returns>
    (int StatusCode, object? Body)? GetResponse(IFailure failure);
}