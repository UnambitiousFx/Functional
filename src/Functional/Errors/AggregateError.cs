using System.Diagnostics;

namespace UnambitiousFx.Functional.Errors;

/// <summary>
///     Represents an error that contains multiple sub-errors, useful for collecting multiple validation failures or
///     errors.
/// </summary>
[DebuggerDisplay("AggregateError: {Errors.Count()} errors")]
[DebuggerTypeProxy(typeof(AggregateErrorDebugView))]
public sealed record AggregateError : Error, IAggregateError
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AggregateError" /> class with a collection of errors.
    /// </summary>
    /// <param name="errors">The collection of errors to aggregate.</param>
    public AggregateError(params IEnumerable<Error> errors)
        : base(ErrorCodes.AggregateError, "Multiple errors occurred")
    {
        Errors = errors.ToArray();
    }

    /// <summary>
    ///     Gets the collection of errors contained in this aggregate error.
    /// </summary>
    public IEnumerable<Error> Errors { get; }
}
