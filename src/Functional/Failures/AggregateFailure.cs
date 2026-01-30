using System.Diagnostics;

namespace UnambitiousFx.Functional.Failures;

/// <summary>
///     Represents an error that contains multiple sub-errors, useful for collecting multiple validation failures or
///     errors.
/// </summary>
[DebuggerDisplay("AggregateError: {Errors.Count()} errors")]
[DebuggerTypeProxy(typeof(AggregateFailureDebugView))]
public sealed record AggregateFailure : Failure, IAggregateFailure
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AggregateFailure" /> class with a collection of errors.
    /// </summary>
    /// <param name="errors">The collection of errors to aggregate.</param>
    public AggregateFailure(params IEnumerable<Failure> errors)
        : base(ErrorCodes.AggregateError, "Multiple errors occurred")
    {
        Errors = errors.ToArray();
    }

    /// <summary>
    ///     Gets the collection of errors contained in this aggregate error.
    /// </summary>
    public IEnumerable<Failure> Errors { get; }
}
