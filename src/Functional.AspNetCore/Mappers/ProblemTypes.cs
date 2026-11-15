using Microsoft.AspNetCore.Http;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Problem type URIs (RFC 9457 <c>type</c>) pointing at the RFC 9110 section that defines each status code.
/// </summary>
internal static class ProblemTypes
{
    private const string Rfc9110 = "https://www.rfc-editor.org/rfc/rfc9110#section-";

    /// <summary>
    ///     Gets the RFC 9110 URI describing <paramref name="statusCode" />, or <see langword="null" /> when the
    ///     default mapper does not write a type for that status.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>The type URI, or <see langword="null" />.</returns>
    public static string? For(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest          => Rfc9110 + "15.5.1",
            StatusCodes.Status401Unauthorized        => Rfc9110 + "15.5.2",
            StatusCodes.Status403Forbidden           => Rfc9110 + "15.5.4",
            StatusCodes.Status404NotFound            => Rfc9110 + "15.5.5",
            StatusCodes.Status409Conflict            => Rfc9110 + "15.5.10",
            StatusCodes.Status422UnprocessableEntity => Rfc9110 + "15.5.21",
            StatusCodes.Status500InternalServerError => Rfc9110 + "15.6.1",
            _                                        => null
        };
    }
}
