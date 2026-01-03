namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Represents an error indicating that a requested resource was not found.
/// </summary>
/// <param name="Resource">The type or name of the resource that was not found.</param>
/// <param name="Identifier">The identifier of the resource that was not found.</param>
/// <param name="Extra">Optional additional metadata about the error.</param>
public sealed record NotFoundError(
    string Resource,
    string Identifier,
    IReadOnlyDictionary<string, object?>? Extra = null)
    : Error(ErrorCodes.NotFound, $"Resource '{Resource}' with id '{Identifier}' was not found.",
        Merge(Extra,
        [
            new KeyValuePair<string, object?>("resource", Resource),
            new KeyValuePair<string, object?>("identifier", Identifier)
        ]));
