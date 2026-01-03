namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Interface for errors that contain multiple sub-errors.
/// </summary>
public interface IAggregateError : IError
{
    /// <summary>
    ///     Gets the collection of errors contained in this aggregate error.
    /// </summary>
    IEnumerable<Error> Errors { get; }
}
