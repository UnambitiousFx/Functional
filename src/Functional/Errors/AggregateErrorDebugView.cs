using System.Diagnostics;

namespace UnambitiousFx.Functional.Errors;

internal sealed class AggregateErrorDebugView
{
    private readonly AggregateError _error;

    public AggregateErrorDebugView(AggregateError error)
    {
        _error = error;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Error[] Errors => _error.Errors.ToArray();

    public string Code => _error.Code;
    public string Message => _error.Message;
    public Metadata Metadata => _error.Metadata;
    public int ErrorCount => _error.Errors.Count();
}
