namespace UnambitiousFx.Functional.AspNetCore;

/// <summary>
///     Defines policy defaults for adapter behavior when converting Functional types to HTTP responses.
/// </summary>
public sealed record ResultHttpAdapterPolicy
{
    /// <summary>
    ///     Shared immutable default policy.
    /// </summary>
    public static ResultHttpAdapterPolicy Default { get; } = new();

    /// <summary>
    ///     Determines how a successful non-generic Result is mapped when no explicit success mapper is provided.
    /// </summary>
    public ResultSuccessHttpBehavior ResultSuccessBehavior { get; init; } = ResultSuccessHttpBehavior.NoContent;

    /// <summary>
    ///     Determines how Maybe.None is mapped when no explicit none mapper is provided.
    /// </summary>
    public MaybeNoneHttpBehavior MaybeNoneBehavior { get; init; } = MaybeNoneHttpBehavior.NotFound;
}

/// <summary>
///     Policy for successful non-generic Result responses.
/// </summary>
public enum ResultSuccessHttpBehavior
{
    /// <summary>
    ///     Map success to HTTP 204 No Content.
    /// </summary>
    NoContent,

    /// <summary>
    ///     Map success to HTTP 200 OK.
    /// </summary>
    Ok
}

/// <summary>
///     Policy for Maybe.None responses.
/// </summary>
public enum MaybeNoneHttpBehavior
{
    /// <summary>
    ///     Map none to HTTP 404 Not Found.
    /// </summary>
    NotFound,

    /// <summary>
    ///     Map none to HTTP 204 No Content.
    /// </summary>
    NoContent
}