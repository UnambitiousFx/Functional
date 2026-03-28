using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;
using IHttpResult = Microsoft.AspNetCore.Http.IResult;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Provides a mechanism for building HTTP results from a functional result structure.
///     This class is designed to help construct ASP.NET Core <see cref="Microsoft.AspNetCore.Http.IResult" />
///     responses based on a <see cref="UnambitiousFx.Functional.Result" /> instance,
///     allowing for streamlined error and success handling in functional programming scenarios.
/// </summary>
public sealed class ResultHttpBuilder
{
    private readonly ValueTask<Result>                    _awaitableResult;
    private readonly IFailureHttpMapper                   _failureMapper;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly int                                  _successStatusCode;

    /// <summary>
    ///     A class used to build HTTP results based on the functional result model.
    /// </summary>
    public ResultHttpBuilder(Result             result,
                             IFailureHttpMapper failureMapper)
        : this(new ValueTask<Result>(result), failureMapper)
    {
    }

    /// <summary>
    ///     Provides a class for constructing HTTP results based on a functional result structure.
    ///     The <see cref="ResultHttpBuilder" /> class is designed to create and customize HTTP responses
    ///     derived from a <see cref="Result" /> instance. This integration improves handling
    ///     of functional results in ASP.NET Core by mapping successes and failures efficiently.
    /// </summary>
    public ResultHttpBuilder(ValueTask<Result>  awaitableResult,
                             IFailureHttpMapper failureMapper)
        : this(awaitableResult, failureMapper, null)
    {
    }

    private ResultHttpBuilder(ValueTask<Result>                    awaitableResult,
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
    ///     Gets an awaiter used to await the completion of the HTTP result construction process.
    /// </summary>
    /// <returns>
    ///     An object of type <see cref="ValueTaskAwaiter{TResult}" /> that allows awaiting the creation
    ///     of an <see cref="Microsoft.AspNetCore.Http.IResult" /> response derived from the functional result model.
    /// </returns>
    public ValueTaskAwaiter<IHttpResult> GetAwaiter()
    {
        return Build()
           .GetAwaiter();
    }

    /// <summary>
    ///     Adds a header to the HTTP response being built.
    /// </summary>
    /// <param name="key">The name of the header to add to the response.</param>
    /// <param name="value">The value associated with the specified header key.</param>
    /// <returns>
    ///     A new instance of <see cref="ResultHttpBuilder" /> with the additional header included.
    /// </returns>
    public ResultHttpBuilder WithHeader(string key,
                                        string value)
    {
        var newHeaders = new Dictionary<string, string>();
        if (_headers is not null) {
            foreach (var (k, v) in _headers) {
                newHeaders[k] = v;
            }
        }

        newHeaders[key] = value;
        return new ResultHttpBuilder(_awaitableResult, _failureMapper, newHeaders, _successStatusCode);
    }

    /// <summary>
    ///     Specifies the HTTP status code to be used for a successful result and returns a new instance
    ///     of the <see cref="ResultHttpBuilder" /> with the updated status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to associate with a successful result.</param>
    /// <returns>A new <see cref="ResultHttpBuilder" /> instance with the specified status code.</returns>
    public ResultHttpBuilder WithStatusCode(int statusCode)
    {
        return new ResultHttpBuilder(_awaitableResult, _failureMapper, _headers, statusCode);
    }

    private async ValueTask<IHttpResult> Build()
    {
        var result = await _awaitableResult;
        return result.Match(OnSuccess, OnFailure);
    }

    private IHttpResult OnFailure(Failure failure)
    {
        var response = _failureMapper.GetFailureResponse(failure);
        return OnFailure(response);
    }

    private IHttpResult OnSuccess()
    {
        return OnSuccess(_headers, _successStatusCode);
    }

    internal static IHttpResult OnFailure(FailureHttpResponse? failureHttpResponse)
    {
        IHttpResult httpResult;
        var         headers     = failureHttpResponse?.Headers;
        var         contentType = failureHttpResponse?.ContentType;

        if (failureHttpResponse is null) {
            httpResult = HttpResults.Problem(statusCode: 500);
        }
        else if (failureHttpResponse.Body is null) {
            httpResult = TypedResults.StatusCode(failureHttpResponse.StatusCode);
        }
        else if (failureHttpResponse.Body is ProblemDetails problemDetails) {
            httpResult = TypedResults.Problem(problemDetails);
        }
        else {
            // Handle different status codes with appropriate result types
            httpResult = failureHttpResponse.StatusCode switch
            {
                400 => HttpResults.BadRequest(failureHttpResponse.Body),
                401 => HttpResults.Unauthorized(),
                403 => HttpResults.Forbid(),
                404 => HttpResults.NotFound(failureHttpResponse.Body),
                409 => HttpResults.Conflict(failureHttpResponse.Body),
                500 => HttpResults.Problem(statusCode: 500),
                _   => HttpResults.StatusCode(failureHttpResponse.StatusCode)
            };
        }

        return new WrapperHttpResult(httpResult, headers, contentType);
    }

    internal static IHttpResult OnSuccess(IReadOnlyDictionary<string, string>? headers,
                                          int                                  statusCode = 204)
    {
        var httpResult = statusCode == 204
                             ? HttpResults.NoContent()
                             : HttpResults.StatusCode(statusCode);
        return new WrapperHttpResult(httpResult, headers);
    }
}

/// <summary>
///     Provides functionality to build HTTP results for scenarios involving instances
///     of <see cref="UnambitiousFx.Functional.Result{TValue}" />.
///     This generic builder facilitates handling failures using an
///     <see cref="UnambitiousFx.Functional.AspNetCore.Mappers.IFailureHttpMapper" /> implementation
///     and enables customization of the response for success and failure states.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public sealed class ResultHttpBuilder<TValue> : ResultHttpBuilder<TValue, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     A class that facilitates the construction of HTTP responses based on the functional result model,
    ///     parameterized by a value type.
    /// </summary>
    public ResultHttpBuilder(Result<TValue>     result,
                             IFailureHttpMapper failureMapper)
        : base(result, failureMapper)
    {
    }

    /// <summary>
    ///     A class responsible for constructing HTTP responses based on the Result functional model.
    /// </summary>
    public ResultHttpBuilder(ValueTask<Result<TValue>> awaitableResult,
                             IFailureHttpMapper        failureMapper)
        : base(awaitableResult, failureMapper)
    {
    }
}

/// <summary>
///     Facilitates the construction of HTTP responses based on the result of a functional operation or computation.
///     This class allows mapping success values or failure cases to <see cref="Microsoft.AspNetCore.Http.IResult" />
///     responses,
///     providing options for customization of status codes, headers, and response formatting.
/// </summary>
public class ResultHttpBuilder<TValue, TResponse>
    where TValue : notnull
{
    private readonly ValueTask<Result<TValue>>            _awaitableResult;
    private readonly IFailureHttpMapper                   _failureMapper;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly Func<TValue, string>?                _locationFactory;
    private readonly Func<TValue, TResponse>?             _responseFormatter;
    private readonly int                                  _successStatusCode;

    /// <summary>
    ///     A builder class designed to construct HTTP results using a functional result model,
    ///     tailored specifically for a single type of response value.
    /// </summary>
    public ResultHttpBuilder(Result<TValue>     result,
                             IFailureHttpMapper failureMapper)
        : this(new ValueTask<Result<TValue>>(result), failureMapper, null)
    {
    }

    /// <summary>
    ///     A class used to construct HTTP results asynchronously for a given result value type,
    ///     specializing in mapping failure responses and optionally formatting successful responses.
    /// </summary>
    public ResultHttpBuilder(ValueTask<Result<TValue>> awaitableResult,
                             IFailureHttpMapper        failureMapper)
        : this(awaitableResult, failureMapper, null)
    {
    }

    private ResultHttpBuilder(ValueTask<Result<TValue>>            awaitableResult,
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
    ///     Retrieves an awaiter used to asynchronously await the completion of the HTTP result construction process.
    ///     This method allows the asynchronous flow to pause and resume once the result is ready.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.Runtime.CompilerServices.ValueTaskAwaiter{TResult}" />
    ///     that can be used to await the final <see cref="Microsoft.AspNetCore.Http.IResult" /> instance.
    /// </returns>
    public ValueTaskAwaiter<IHttpResult> GetAwaiter()
    {
        return Build()
           .GetAwaiter();
    }

    /// <summary>
    ///     Adds or updates a header to be included in the HTTP response.
    /// </summary>
    /// <param name="key">The key of the header to add or update.</param>
    /// <param name="value">The value of the header to associate with the specified key.</param>
    /// <returns>A new instance of <see cref="ResultHttpBuilder{TValue, TResponse}" /> with the specified header included.</returns>
    public ResultHttpBuilder<TValue, TResponse> WithHeader(string key,
                                                           string value)
    {
        var newHeaders = new Dictionary<string, string>();
        if (_headers is not null) {
            foreach (var (k, v) in _headers) {
                newHeaders[k] = v;
            }
        }

        newHeaders[key] = value;
        return new ResultHttpBuilder<TValue, TResponse>(
            _awaitableResult,
            _failureMapper,
            _responseFormatter,
            newHeaders,
            _successStatusCode,
            _locationFactory
        );
    }

    /// <summary>
    ///     Configures the builder to include the specified HTTP status code in the resulting response.
    /// </summary>
    /// <param name="statusCode">
    ///     The HTTP status code to associate with the successful outcome of the operation.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="ResultHttpBuilder{TValue, TResponse}" /> with the specified status code applied.
    /// </returns>
    public ResultHttpBuilder<TValue, TResponse> WithStatusCode(int statusCode)
    {
        return new ResultHttpBuilder<TValue, TResponse>(
            _awaitableResult,
            _failureMapper,
            _responseFormatter,
            _headers,
            statusCode,
            _locationFactory
        );
    }

    /// <summary>
    ///     Configures the HTTP response formatter by specifying a transformation function
    ///     to convert the result value into a response of the specified type.
    /// </summary>
    /// <typeparam name="TNewResponse">The type of the new response to be produced.</typeparam>
    /// <param name="formatter">
    ///     A function that transforms the successful result value of type <typeparamref name="TValue" />
    ///     into a response of type <typeparamref name="TNewResponse" />.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="ResultHttpBuilder{TValue, TNewResponse}" /> configured with
    ///     the specified response formatter.
    /// </returns>
    public ResultHttpBuilder<TValue, TNewResponse> WithResponseFormatter<TNewResponse>(Func<TValue, TNewResponse> formatter)
    {
        return new ResultHttpBuilder<TValue, TNewResponse>(
            _awaitableResult,
            _failureMapper,
            formatter,
            _headers,
            _successStatusCode,
            _locationFactory
        );
    }

    /// <summary>
    ///     Configures the HTTP response for the current result to have a 201 (Created) status code with a specified location.
    /// </summary>
    /// <param name="locationFactory">
    ///     A function that generates the location URI string based on the success value of the result.
    /// </param>
    /// <returns>
    ///     A new <see cref="ResultHttpBuilder{TValue, TResponse}" /> instance configured with a 201 (Created) status code and
    ///     the provided location factory.
    /// </returns>
    public ResultHttpBuilder<TValue, TResponse> AsCreated(Func<TValue, string> locationFactory)
    {
        return new ResultHttpBuilder<TValue, TResponse>(
            _awaitableResult,
            _failureMapper,
            _responseFormatter,
            _headers,
            201,
            locationFactory
        );
    }

    /// <summary>
    ///     Builds an HTTP response represented as an <see cref="Microsoft.AspNetCore.Http.IResult" />
    ///     based on the result of a functional operation. This method selects between success and failure
    ///     outcomes and constructs the corresponding HTTP response accordingly.
    /// </summary>
    /// <returns>
    ///     An <see cref="Microsoft.AspNetCore.Http.IResult" /> that represents the HTTP response.
    ///     The response is determined by the result of the computation: success results invoke a success handler,
    ///     while failure cases invoke a failure handler.
    /// </returns>
    public async ValueTask<IHttpResult> Build()
    {
        var result = await _awaitableResult;
        return result.Match(OnSuccess, OnFailure);
    }

    private IHttpResult OnFailure(Failure failure)
    {
        var response = _failureMapper.GetFailureResponse(failure);
        return ResultHttpBuilder.OnFailure(response);
    }

    private IHttpResult OnSuccess(TValue value)
    {
        var responseValue = _responseFormatter is not null
                                ? _responseFormatter(value)
                                : (TResponse)(object)value;

        IHttpResult httpResult;
        if (_successStatusCode == 201 &&
            _locationFactory is not null) {
            var location = _locationFactory(value);
            httpResult = HttpResults.Created(location, responseValue);
        }
        else {
            httpResult = _successStatusCode switch
            {
                200 => HttpResults.Ok(responseValue),
                201 => HttpResults.Created(string.Empty, responseValue),
                202 => HttpResults.Accepted(null, responseValue),
                204 => HttpResults.NoContent(),
                _   => HttpResults.StatusCode(_successStatusCode)
            };
        }

        return new WrapperHttpResult(httpResult, _headers);
    }
}
