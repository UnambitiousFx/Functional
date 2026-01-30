using System.Diagnostics;

namespace UnambitiousFx.Functional.Failures;

internal sealed class AggregateFailureDebugView
{
    private readonly AggregateFailure _failure;

    public AggregateFailureDebugView(AggregateFailure failure)
    {
        _failure = failure;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Failure[] Errors => _failure.Errors.ToArray();

    public string Code => _failure.Code;
    public string Message => _failure.Message;
    public Metadata Metadata => _failure.Metadata;
    public int ErrorCount => _failure.Errors.Count();
}
