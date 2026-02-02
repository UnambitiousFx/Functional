namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Represents a complete HTTP error response including status code, body, headers, and content type.
/// </summary>
public sealed record ErrorHttpResponse
{
    /// <summary>
    ///     The HTTP status code for the error response.
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    ///     The response body object. Can be null for responses without a body.
    /// </summary>
    public object? Body { get; init; }

    /// <summary>
    ///     Optional HTTP headers to include in the response.
    ///     Common examples: WWW-Authenticate, Retry-After, Location, Cache-Control.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Headers { get; init; }

    /// <summary>
    ///     Optional content type for the response body.
    ///     If not specified, the default content type negotiation will be used.
    /// </summary>
    public string? ContentType { get; init; }
}
