using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Maps failure reasons to HTTP status codes and response bodies.
/// </summary>
public interface IFailureHttpMapper
{
    /// <summary>
    ///     Creates a complete HTTP failure response for the specified failure, including status code, body, headers, and content
    ///     type.
    ///     Returns null if this mapper cannot handle the failure type.
    /// </summary>
    /// <param name="failure">The failure to map.</param>
    /// <returns>An <see cref="FailureHttpResponse" /> containing the complete HTTP response details, or null if not handled.</returns>
    FailureHttpResponse? GetFailureResponse(IFailure failure);
}
