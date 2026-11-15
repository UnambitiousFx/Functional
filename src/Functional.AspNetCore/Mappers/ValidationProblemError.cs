using System.Text.Json.Serialization;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     One entry of the RFC 9457 <c>errors</c> list written for a <see cref="ValidationFailure" /> when the format is
///     <see cref="ValidationErrorsFormat.Rfc9457" />.
/// </summary>
/// <param name="Detail">The error message (<see cref="ValidationError.Message" />).</param>
/// <param name="Pointer">
///     The invalid field as a JSON Pointer (<see cref="ValidationError.Pointer" />), omitted when the error is not tied
///     to a field.
/// </param>
/// <param name="Code">The error code (<see cref="ValidationError.Code" />), omitted when absent.</param>
public sealed record ValidationProblemError(
    [property: JsonPropertyName("detail")] string Detail,
    [property: JsonPropertyName("pointer")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Pointer,
    [property: JsonPropertyName("code")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Code)
{
    /// <summary>
    ///     Creates the entry for a <see cref="ValidationError" />.
    /// </summary>
    /// <param name="error">The validation error.</param>
    /// <returns>The RFC 9457 entry.</returns>
    public static ValidationProblemError From(ValidationError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new ValidationProblemError(error.Message, error.Pointer, error.Code);
    }
}
