using System.Diagnostics.CodeAnalysis;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional;

[ExcludeFromCodeCoverage]
internal sealed class ResultDebugView<TValue>
    where TValue : notnull
{
    private readonly Result<TValue> _result;

    public ResultDebugView(Result<TValue> result)
    {
        _result = result;
    }

    public bool IsSuccess => _result.IsSuccess;
    public bool IsFaulted => _result.IsFailure;

    public TValue? Value => _result.TryGetValue(out var value)
                                ? value
                                : default;

    public Failure? Failure => _result.TryGet(out _, out var failure)
                                 ? null
                                 : failure;

    public Metadata Metadata => (Metadata)_result.Metadata;

    public object? FailureDetails
    {
        get
        {
            if (!_result.TryGet(out _, out var failure)) {
                return failure switch
                {
                    AggregateFailure agg => new
                    {
                        Type = "AggregateFailure", FailureCount = agg.Errors.Count(), Failures = agg.Errors.ToArray()
                    },
                    ExceptionalFailure exc => new
                    {
                        Type = "ExceptionalFailure", exc.Exception, ExceptionType = exc.Exception.GetType()
                                                                                     .Name
                    },
                    _ => new
                    {
                        Type = failure.GetType()
                                    .Name,
                        failure.Code, failure.Message
                    }
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
    public bool IsFaulted => _result.IsFailure;

    public Failure? Failure => !_result.TryGetFailure(out var failure)
                                 ? null
                                 : failure;

    public Metadata Metadata => (Metadata)_result.Metadata;

    public object? FailureDetails
    {
        get
        {
            if (!_result.TryGetFailure(out var failure)) {
                return null;
            }

            return failure switch
            {
                AggregateFailure agg => new
                {
                    Type = "AggregateFailure", FailureCount = agg.Errors.Count(), Failures = agg.Errors.ToArray()
                },
                ExceptionalFailure exc => new
                {
                    Type = "ExceptionalFailure", exc.Exception, ExceptionType = exc.Exception.GetType()
                                                                                 .Name
                },
                _ => new
                {
                    Type = failure.GetType()
                                .Name,
                    failure.Code, failure.Message
                }
            };
        }
    }
}
