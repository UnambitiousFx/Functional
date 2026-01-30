namespace UnambitiousFx.Functional.Failures;

internal sealed class ExceptionalFailureDebugView
{
    private readonly ExceptionalFailure _failure;

    public ExceptionalFailureDebugView(ExceptionalFailure failure)
    {
        _failure = failure;
    }

    public Exception Exception => _failure.Exception;
    public string ExceptionType => _failure.Exception.GetType().Name;
    public string Message => _failure.Message;
    public string Code => _failure.Code;
    public string? MessageOverride => _failure.MessageOverride;
    public Metadata Metadata => _failure.Metadata;
    public IReadOnlyDictionary<string, object?>? Extra => _failure.Extra;
}
