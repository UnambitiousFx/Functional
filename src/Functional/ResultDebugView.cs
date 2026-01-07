using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional;

internal sealed class ResultDebugView<TValue>
    where TValue : notnull
{
    private readonly Result<TValue> _result;

    public ResultDebugView(Result<TValue> result)
    {
        _result = result;
    }

    public bool IsSuccess => _result.IsSuccess;
    public bool IsFaulted => _result.IsFaulted;

    public TValue? Value => _result.TryGet(out TValue? value) ? value : default;

    public Error? Error => _result.TryGet(out _, out var error) ? null : error;

    public Metadata Metadata => (Metadata)_result.Metadata;

    public object? ErrorDetails
    {
        get
        {
            if (!_result.TryGet(out _, out var error))
            {
                return error switch
                {
                    AggregateError agg => new
                    {
                        Type = "AggregateError", ErrorCount = agg.Errors.Count(), Errors = agg.Errors.ToArray()
                    },
                    ExceptionalError exc => new
                    {
                        Type = "ExceptionalError", exc.Exception, ExceptionType = exc.Exception.GetType().Name
                    },
                    _ => new { Type = error.GetType().Name, error.Code, error.Message }
                };
            }

            return null;
        }
    }
}

internal sealed class ResultDebugView
{
    private readonly Result _result;

    public ResultDebugView(Result result)
    {
        _result = result;
    }

    public bool IsSuccess => _result.IsSuccess;
    public bool IsFaulted => _result.IsFaulted;

    public Error? Error => _result.TryGet(out var error) ? null : error;

    public Metadata Metadata => (Metadata)_result.Metadata;

    public object? ErrorDetails
    {
        get
        {
            if (_result.TryGet(out var error))
            {
                return null;
            }

            return error switch
            {
                AggregateError agg => new
                {
                    Type = "AggregateError", ErrorCount = agg.Errors.Count(), Errors = agg.Errors.ToArray()
                },
                ExceptionalError exc => new
                {
                    Type = "ExceptionalError", exc.Exception, ExceptionType = exc.Exception.GetType().Name
                },
                _ => new { Type = error?.GetType().Name, error?.Code, error?.Message }
            };
        }
    }
}