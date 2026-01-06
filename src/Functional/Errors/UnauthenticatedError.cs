namespace UnambitiousFx.Functional.Errors;

public sealed record UnauthenticatedError(string? Reason = null,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Error(ErrorCodes.Unauthenticated, Reason ?? "Unauthenticated.", Merge(Extra, []));