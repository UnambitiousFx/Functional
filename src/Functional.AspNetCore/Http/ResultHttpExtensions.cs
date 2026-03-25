using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Internal;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;
using IHttpResult = Microsoft.AspNetCore.Http.IResult;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Extension methods for converting Result types to HTTP responses (IResult) for minimal APIs.
/// </summary>
public static class ResultHttpExtensions
{
    private static Func<IHttpResult> BuildDefaultSuccessMapper(ResultHttpAdapterPolicy? policy)
    {
        var effectivePolicy = policy ?? ResultHttpAdapterPolicy.Default;
        return effectivePolicy.ResultSuccessBehavior switch
        {
            ResultSuccessHttpBehavior.Ok => () => HttpResults.Ok(),
            _                            => HttpResults.NoContent
        };
    }

    private static IHttpResult ProblemDetailToActionResult(ProblemDetails details)
    {
        return HttpResults.Problem(details);
    }

    private static IHttpResult BodyToActionResult(int     statusCode,
                                                  object? body)
    {
        return statusCode switch
        {
            400 => HttpResults.BadRequest(body),
            401 => HttpResults.Unauthorized(),
            403 => HttpResults.Forbid(),
            404 => HttpResults.NotFound(body),
            409 => HttpResults.Conflict(body),
            500 => HttpResults.Problem(statusCode: statusCode),
            _   => HttpResults.StatusCode(statusCode)
        };
    }

    private static IHttpResult ResponseToHttpResult(int     statusCode,
                                                    object? body)
    {
        return body switch
        {
            null                          => HttpResults.StatusCode(statusCode),
            ProblemDetails problemDetails => ProblemDetailToActionResult(problemDetails),
            _                             => BodyToActionResult(statusCode, body)
        };
    }

    private static IHttpResult ErrorHttpResponseToHttpResult(ErrorHttpResponse response)
    {
        var result = ResponseToHttpResult(response.StatusCode, response.Body);

        // Apply headers if present
        if (response.Headers is not null &&
            response.Headers.Count > 0) {
            // For minimal APIs, we need to use TypedResults or Results.Extensions to add headers
            // Since IResult doesn't expose headers directly, we wrap in a custom result
            return new HeaderedHttpResult(result, response.Headers);
        }

        return result;
    }

    private static IHttpResult MapErrorToHttpResult(Failure           failure,
                                                    IErrorHttpMapper? customMapper)
    {
        var response = FailureHttpResponseResolver.Resolve(failure, customMapper);
        return ErrorHttpResponseToHttpResult(response);
    }

    /// <summary>
    ///     Converts an asynchronous Result into an <see cref="Microsoft.AspNetCore.Http.IResult" /> for minimal API endpoints.
    /// </summary>
    /// <param name="resultTask">The asynchronous result to convert.</param>
    /// <param name="successHttpMapper">
    ///     An optional function to define the HTTP response when the operation is successful. Defaults to a NoContent response
    ///     if not provided.
    /// </param>
    /// <param name="errorHttpMapper">
    ///     An optional custom error mapper to translate failure results into specific HTTP responses. Uses the default error
    ///     mapper if not provided.
    /// </param>
    /// <param name="policy">
    ///     Optional adapter policy controlling default success behavior when no success mapper is provided.
    /// </param>
    /// <returns>
    ///     A <see cref="Microsoft.AspNetCore.Http.IResult" /> representing either a successful result or a mapped failure
    ///     response.
    /// </returns>
    public static async ValueTask<IHttpResult> ToHttpResult(this ValueTask<Result>   resultTask,
                                                            Func<IHttpResult>?       successHttpMapper = null,
                                                            IErrorHttpMapper?        errorHttpMapper   = null,
                                                            ResultHttpAdapterPolicy? policy            = null)
    {
        successHttpMapper ??= BuildDefaultSuccessMapper(policy);
        var result = await resultTask;
        return result.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorHttpMapper));
    }

    /// <summary>
    ///     Converts an asynchronous Result into an <see cref="Microsoft.AspNetCore.Http.IResult" /> for minimal API endpoints.
    /// </summary>
    public static ValueTask<IHttpResult> ToHttpResult(this Task<Result>        resultTask,
                                                      Func<IHttpResult>?       successHttpMapper = null,
                                                      IErrorHttpMapper?        errorHttpMapper   = null,
                                                      ResultHttpAdapterPolicy? policy            = null)
    {
        return new ValueTask<Result>(resultTask).ToHttpResult(successHttpMapper, errorHttpMapper, policy);
    }

    /// <summary>
    ///     Converts an asynchronous Result to an HTTP 201 Created response, including a "Location" header derived from the
    ///     provided location factory.
    /// </summary>
    public static ValueTask<IHttpResult> ToCreatedHttpResult<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                     Func<TValue, string>           locationFactory,
                                                                     IErrorHttpMapper?              mapper = null)
        where TValue : notnull
    {
        return resultTask.ToHttpResult(v => HttpResults.Created(locationFactory(v), v), mapper);
    }

    /// <summary>
    ///     Converts an asynchronous Result into an <see cref="IHttpResult" /> for minimal API endpoints,
    ///     allowing customization of success and failure HTTP responses.
    /// </summary>
    public static async ValueTask<IHttpResult> ToHttpResult<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                    Func<TValue, IHttpResult>?     successHttpMapper = null,
                                                                    IErrorHttpMapper?              errorMapper       = null)
        where TValue : notnull
    {
        successHttpMapper ??= HttpResults.Ok;
        var result = await resultTask;
        return result.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorMapper));
    }

    /// <summary>
    ///     Converts an asynchronous Result to an HTTP 201 Created response.
    /// </summary>
    public static ValueTask<IHttpResult> ToCreatedHttpResult<TValue>(this Task<Result<TValue>> resultTask,
                                                                     Func<TValue, string>      locationFactory,
                                                                     IErrorHttpMapper?         mapper = null)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(resultTask).ToCreatedHttpResult(locationFactory, mapper);
    }

    /// <summary>
    ///     Converts an asynchronous Result into an <see cref="IHttpResult" /> for minimal API endpoints.
    /// </summary>
    public static ValueTask<IHttpResult> ToHttpResult<TValue>(this Task<Result<TValue>>  resultTask,
                                                              Func<TValue, IHttpResult>? successHttpMapper = null,
                                                              IErrorHttpMapper?          errorMapper       = null)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(resultTask).ToHttpResult(successHttpMapper, errorMapper);
    }

    /// <summary>
    ///     Custom IResult implementation that wraps another IResult and adds HTTP headers.
    /// </summary>
    private sealed class HeaderedHttpResult : IHttpResult
    {
        private readonly IReadOnlyDictionary<string, string> _headers;
        private readonly IHttpResult                         _innerResult;

        public HeaderedHttpResult(IHttpResult                         innerResult,
                                  IReadOnlyDictionary<string, string> headers)
        {
            _innerResult = innerResult;
            _headers     = headers;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            // Add headers to the response
            foreach (var (key, value) in _headers) {
                httpContext.Response.Headers[key] = value;
            }

            // Execute the inner result
            return _innerResult.ExecuteAsync(httpContext);
        }
    }

    /// <param name="result">The result to convert.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Converts a Result to an IResult for minimal API endpoints, mapping success or failure to corresponding HTTP
        ///     responses.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     Optional function to define the HTTP response for a successful result. Defaults to NoContent if not provided.
        /// </param>
        /// <param name="errorHttpMapper">
        ///     Optional custom error mapper to translate failures into HTTP responses. The default mapper is used if not provided.
        /// </param>
        /// <param name="policy">
        ///     Optional adapter policy controlling default success behavior when no success mapper is provided.
        /// </param>
        /// <returns>An IResult representing either a successful or failed result mapped to an appropriate HTTP response.</returns>
        public IHttpResult ToHttpResult(Func<IHttpResult>?       successHttpMapper = null,
                                        IErrorHttpMapper?        errorHttpMapper   = null,
                                        ResultHttpAdapterPolicy? policy            = null)
        {
            successHttpMapper ??= BuildDefaultSuccessMapper(policy);
            return result.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorHttpMapper));
        }
    }

    /// <param name="result">The result to convert.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(Result<TValue> result)
        where TValue : notnull
    {
        /// <summary>
        ///     Converts a Result&lt;T&gt; to a Created IResult for minimal API endpoints.
        ///     Success returns 201 Created with location and value, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="locationFactory">Function to generate the location URI from the value.</param>
        /// <param name="errorHttpMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToCreatedHttpResult(Func<TValue, string> locationFactory,
                                               IErrorHttpMapper?    errorHttpMapper = null)
        {
            return result.ToHttpResult(v => HttpResults.Created(locationFactory(v), v), errorHttpMapper);
        }

        /// <summary>
        ///     Converts a <see cref="Result{TValue}" /> into an <see cref="IHttpResult" /> for minimal API responses.
        ///     Maps the result's success value to an HTTP response, defaulting to 200 OK with the value when no mapper is given.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     An optional function to create an HTTP result from the success value.
        ///     Defaults to 200 OK with the value when not provided.
        /// </param>
        /// <param name="errorHttpMapper">
        ///     An optional mapper to handle errors and convert them into HTTP responses. Defaults to a
        ///     predefined implementation if not provided.
        /// </param>
        /// <returns>An <see cref="IHttpResult" /> representing the HTTP response for the result.</returns>
        public IHttpResult ToHttpResult(Func<TValue, IHttpResult>? successHttpMapper = null,
                                        IErrorHttpMapper?          errorHttpMapper   = null)
        {
            successHttpMapper ??= HttpResults.Ok;
            return result.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorHttpMapper));
        }
    }
}
