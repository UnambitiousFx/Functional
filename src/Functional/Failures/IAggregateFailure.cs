namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Interface for errors that contain multiple sub-errors.
/// </summary>
public interface IAggregateFailure : IFailure
{
    /// <summary>
    ///     Gets the collection of errors contained in this aggregate error.
    /// </summary>
    IEnumerable<Failure> Errors { get; }
}
