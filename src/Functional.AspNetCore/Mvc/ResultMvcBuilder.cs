using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Provides a mechanism for building MVC action results from a functional result structure.
/// </summary>
public sealed class ResultMvcBuilder
{
    private readonly ValueTask<Result>                    _awaitableResult;
    private readonly IFailureHttpMapper                   _failureMapper;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly int                                  _successStatusCode;

    /// <summary>
    ///     Initializes a new <see cref="ResultMvcBuilder" /> from a synchronous <see cref="Result" />.
    /// </summary>
    public ResultMvcBuilder(Result             result,
                            IFailureHttpMapper failureMapper)
        : this(new ValueTask<Result>(result), failureMapper)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="ResultMvcBuilder" /> from an asynchronous <see cref="Result" />.
    /// </summary>
    public ResultMvcBuilder(ValueTask<Result>  awaitableResult,
                            IFailureHttpMapper failureMapper)
        : this(awaitableResult, failureMapper, null)
    {
    }

    private ResultMvcBuilder(ValueTask<Result>                    awaitableResult,
                             IFailureHttpMapper                   failureMapper,
                             IReadOnlyDictionary<string, string>? headers           = null,
                             int                                  successStatusCode = 204)
    {
        _awaitableResult   = awaitableResult;
        _failureMapper     = failureMapper;
        _headers           = headers;
        _successStatusCode = successStatusCode;
    }

    /// <summary>
    ///     Gets an awaiter used to await the completion of action result construction.
    /// </summary>
    public ValueTaskAwaiter<IActionResult> GetAwaiter()
    {
        return Build()
           .GetAwaiter();
    }

    /// <summary>
    ///     Adds a header to the response being built.
    /// </summary>
    public ResultMvcBuilder WithHeader(string key,
                                       string value)
    {
        var newHeaders = new Dictionary<string, string>();
        if (_headers is not null) {
            foreach (var (k, v) in _headers) {
                newHeaders[k] = v;
            }
        }

        newHeaders[key] = value;
        return new ResultMvcBuilder(_awaitableResult, _failureMapper, newHeaders, _successStatusCode);
    }

    /// <summary>
    ///     Specifies the HTTP status code used for successful results.
    /// </summary>
    public ResultMvcBuilder WithStatusCode(int statusCode)
    {
        return new ResultMvcBuilder(_awaitableResult, _failureMapper, _headers, statusCode);
    }

    private async ValueTask<IActionResult> Build()
    {
        var result = await _awaitableResult;
        return result.Match(OnSuccess, OnFailure);
    }

    private IActionResult OnFailure(Failure failure)
    {
        var response = _failureMapper.GetFailureResponse(failure);
        return OnFailure(response);
    }

    private IActionResult OnSuccess()
    {
        return OnSuccess(_headers, _successStatusCode);
    }

    internal static IActionResult OnFailure(FailureHttpResponse? failureHttpResponse)
    {
        IActionResult actionResult;
        var           headers     = failureHttpResponse?.Headers;
        var           contentType = failureHttpResponse?.ContentType;

        if (failureHttpResponse is null) {
            actionResult = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        else if (failureHttpResponse.Body is null) {
            actionResult = new StatusCodeResult(failureHttpResponse.StatusCode);
        }
        else if (failureHttpResponse.Body is ProblemDetails problemDetails) {
            actionResult = new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError
            };
        }
        else {
            actionResult = failureHttpResponse.StatusCode switch
            {
                400 => new BadRequestObjectResult(failureHttpResponse.Body),
                401 => new UnauthorizedObjectResult(failureHttpResponse.Body),
                403 => new ForbidResult(),
                404 => new NotFoundObjectResult(failureHttpResponse.Body),
                409 => new ConflictObjectResult(failureHttpResponse.Body),
                500 => new ObjectResult(failureHttpResponse.Body)
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                },
                _ => new ObjectResult(failureHttpResponse.Body)
                {
                    StatusCode = failureHttpResponse.StatusCode
                }
            };
        }

        return new WrapperActionResult(actionResult, headers, contentType);
    }

    internal static IActionResult OnSuccess(IReadOnlyDictionary<string, string>? headers,
                                            int                                  statusCode = 204)
    {
        var actionResult = statusCode == StatusCodes.Status204NoContent
                               ? new NoContentResult()
                               : new StatusCodeResult(statusCode);

        return new WrapperActionResult(actionResult, headers);
    }
}

/// <summary>
///     Provides functionality to build MVC action results for <see cref="Result{TValue}" /> instances.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public sealed class ResultMvcBuilder<TValue> : ResultMvcBuilder<TValue, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Initializes a new <see cref="ResultMvcBuilder{TValue}" /> from a synchronous <see cref="Result{TValue}" />.
    /// </summary>
    public ResultMvcBuilder(Result<TValue>     result,
                            IFailureHttpMapper failureMapper)
        : base(result, failureMapper)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="ResultMvcBuilder{TValue}" /> from an asynchronous <see cref="Result{TValue}" />.
    /// </summary>
    public ResultMvcBuilder(ValueTask<Result<TValue>> awaitableResult,
                            IFailureHttpMapper        failureMapper)
        : base(awaitableResult, failureMapper)
    {
    }
}

/// <summary>
///     Facilitates the construction of MVC action results based on a functional result.
/// </summary>
/// <typeparam name="TValue">The source success value type.</typeparam>
/// <typeparam name="TResponse">The response body type.</typeparam>
public class ResultMvcBuilder<TValue, TResponse>
    where TValue : notnull
{
    private readonly ValueTask<Result<TValue>>            _awaitableResult;
    private readonly IFailureHttpMapper                   _failureMapper;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly Func<TValue, string>?                _locationFactory;
    private readonly Func<TValue, TResponse>?             _responseFormatter;
    private readonly int                                  _successStatusCode;

    /// <summary>
    ///     Initializes a new <see cref="ResultMvcBuilder{TValue, TResponse}" /> from a synchronous result.
    /// </summary>
    public ResultMvcBuilder(Result<TValue>     result,
                            IFailureHttpMapper failureMapper)
        : this(new ValueTask<Result<TValue>>(result), failureMapper, null)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="ResultMvcBuilder{TValue, TResponse}" /> from an asynchronous result.
    /// </summary>
    public ResultMvcBuilder(ValueTask<Result<TValue>> awaitableResult,
                            IFailureHttpMapper        failureMapper)
        : this(awaitableResult, failureMapper, null)
    {
    }

    private ResultMvcBuilder(ValueTask<Result<TValue>>            awaitableResult,
                             IFailureHttpMapper                   failureMapper,
                             Func<TValue, TResponse>?             responseFormatter = null,
                             IReadOnlyDictionary<string, string>? headers           = null,
                             int                                  successStatusCode = 200,
                             Func<TValue, string>?                locationFactory   = null)
    {
        _awaitableResult   = awaitableResult;
        _failureMapper     = failureMapper;
        _responseFormatter = responseFormatter;
        _headers           = headers;
        _successStatusCode = successStatusCode;
        _locationFactory   = locationFactory;
    }

    /// <summary>
    ///     Gets an awaiter used to await the completion of action result construction.
    /// </summary>
    public ValueTaskAwaiter<IActionResult> GetAwaiter()
    {
        return Build()
           .GetAwaiter();
    }

    /// <summary>
    ///     Adds or updates a header to be included in the response.
    /// </summary>
    public ResultMvcBuilder<TValue, TResponse> WithHeader(string key,
                                                          string value)
    {
        var newHeaders = new Dictionary<string, string>();
        if (_headers is not null) {
            foreach (var (k, v) in _headers) {
                newHeaders[k] = v;
            }
        }

        newHeaders[key] = value;
        return new ResultMvcBuilder<TValue, TResponse>(
            _awaitableResult,
            _failureMapper,
            _responseFormatter,
            newHeaders,
            _successStatusCode,
            _locationFactory
        );
    }

    /// <summary>
    ///     Configures the status code to use for successful responses.
    /// </summary>
    public ResultMvcBuilder<TValue, TResponse> WithStatusCode(int statusCode)
    {
        return new ResultMvcBuilder<TValue, TResponse>(
            _awaitableResult,
            _failureMapper,
            _responseFormatter,
            _headers,
            statusCode,
            _locationFactory
        );
    }

    /// <summary>
    ///     Configures the formatter used to transform successful values.
    /// </summary>
    public ResultMvcBuilder<TValue, TNewResponse> WithResponseFormatter<TNewResponse>(Func<TValue, TNewResponse> formatter)
    {
        return new ResultMvcBuilder<TValue, TNewResponse>(
            _awaitableResult,
            _failureMapper,
            formatter,
            _headers,
            _successStatusCode,
            _locationFactory
        );
    }

    /// <summary>
    ///     Configures the response as 201 Created and sets a location factory.
    /// </summary>
    public ResultMvcBuilder<TValue, TResponse> AsCreated(Func<TValue, string> locationFactory)
    {
        return new ResultMvcBuilder<TValue, TResponse>(
            _awaitableResult,
            _failureMapper,
            _responseFormatter,
            _headers,
            StatusCodes.Status201Created,
            locationFactory
        );
    }

    /// <summary>
    ///     Builds the final MVC action result.
    /// </summary>
    public async ValueTask<IActionResult> Build()
    {
        var result = await _awaitableResult;
        return result.Match(OnSuccess, OnFailure);
    }

    private IActionResult OnFailure(Failure failure)
    {
        var response = _failureMapper.GetFailureResponse(failure);
        return ResultMvcBuilder.OnFailure(response);
    }

    private IActionResult OnSuccess(TValue value)
    {
        var responseValue = _responseFormatter is not null
                                ? _responseFormatter(value)
                                : (TResponse)(object)value;

        IActionResult actionResult;
        if (_successStatusCode == StatusCodes.Status201Created &&
            _locationFactory is not null) {
            var location = _locationFactory(value);
            actionResult = new CreatedResult(location, responseValue);
        }
        else {
            actionResult = _successStatusCode switch
            {
                200 => new OkObjectResult(responseValue),
                201 => new CreatedResult(string.Empty, responseValue),
                202 => new AcceptedResult((string?)null, responseValue),
                204 => new NoContentResult(),
                _   => new StatusCodeResult(_successStatusCode)
            };
        }

        return new WrapperActionResult(actionResult, _headers);
    }
}
