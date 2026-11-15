using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     How the default mapper writes the <c>errors</c> of a <see cref="ValidationFailure" /> into the problem details body.
/// </summary>
public enum ValidationErrorsFormat
{
    /// <summary>
    ///     The ASP.NET Core <c>ValidationProblemDetails</c> shape: <c>errors</c> is a dictionary of field → messages
    ///     (<c>""</c> for errors not tied to a field), and <c>errorCodes</c> a dictionary of field → codes, present only
    ///     when at least one error has a code. This is the current default.
    /// </summary>
    AspNetCore = 0,

    /// <summary>
    ///     The RFC 9457 shape: <c>errors</c> is a list with one <see cref="ValidationProblemError" /> per
    ///     <see cref="ValidationError" />, in order, each with its <c>detail</c>, its field as a JSON Pointer
    ///     (<c>pointer</c>) and its <c>code</c>. No <c>errorCodes</c> is written.
    /// </summary>
    Rfc9457 = 1
}
