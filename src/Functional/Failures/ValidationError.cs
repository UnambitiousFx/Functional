namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     A single validation error, optionally tied to a field and carrying a stable code for clients.
/// </summary>
/// <param name="Field">The name of the invalid field, or <see langword="null" /> for an error not tied to a field.</param>
/// <param name="Message">The human-readable error message.</param>
/// <param name="Code">An optional stable code (for example <c>"Required"</c>) clients can use for i18n or UI mapping.</param>
public sealed record ValidationError(string? Field, string Message, string? Code = null);
