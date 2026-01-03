namespace UnambitiousFx.Functional.Errors;

internal sealed class ExceptionalErrorDebugView
{
    private readonly ExceptionalError _error;

    public ExceptionalErrorDebugView(ExceptionalError error)
    {
        _error = error;
    }

    public Exception Exception => _error.Exception;
    public string ExceptionType => _error.Exception.GetType().Name;
    public string Message => _error.Message;
    public string Code => _error.Code;
    public string? MessageOverride => _error.MessageOverride;
    public Metadata Metadata => _error.Metadata;
    public IReadOnlyDictionary<string, object?>? Extra => _error.Extra;
}
