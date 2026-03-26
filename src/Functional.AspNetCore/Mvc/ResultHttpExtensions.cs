using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Internal;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Provides extension methods for handling Result objects in the context of ASP.NET Core MVC.
///     These methods convert Result and Result&lt;T&gt; instances to IActionResult, supporting both
///     success and error scenarios with mapping to HTTP status codes.
/// </summary>
public static class ResultHttpExtensions
{
    private static Func<IActionResult> BuildDefaultSuccessMapper(ResultHttpAdapterPolicy? policy)
    {
        var effectivePolicy = policy ?? ResultHttpAdapterPolicy.Default;
        return effectivePolicy.ResultSuccessBehavior switch
        {
            ResultSuccessHttpBehavior.Ok => () => new OkResult(),
            _                            => () => new NoContentResult()
        };
    }

    private static ObjectResult ProblemDetailToActionResult(ProblemDetails details)
    {
        return new ObjectResult(details)
        {
            StatusCode = details.Status ?? StatusCodes.Status500InternalServerError
        };
    }

    private static IActionResult DefaultActionResult(int     statusCode,
                                                     object? body)
    {
        if (body is null) {
            return new StatusCodeResult(statusCode);
        }

        return new ObjectResult(body) { StatusCode = statusCode };
    }

    private static IActionResult BodyToActionResult(int     statusCode,
                                                    object? body)
    {
        return statusCode switch
        {
            400 => new BadRequestObjectResult(body),
            401 => new UnauthorizedObjectResult(body),
            403 => new ForbidResult(),
            404 => new NotFoundObjectResult(body),
            409 => new ConflictObjectResult(body),
            500 => new ObjectResult(body)
            {
                StatusCode = 500
            },
            _ => DefaultActionResult(statusCode, body)
        };
    }

    private static IActionResult ResponseToActionResult(int     statusCode,
                                                        object? body)
    {
        return body switch
        {
            null                          => new StatusCodeResult(statusCode),
            ProblemDetails problemDetails => ProblemDetailToActionResult(problemDetails),
            _                             => BodyToActionResult(statusCode, body)
        };
    }

    private static IActionResult FailureHttpResponseToActionResult(FailureHttpResponse response)
    {
        var result = ResponseToActionResult(response.StatusCode, response.Body);

        // Apply headers if present
        if (response.Headers is not null &&
            response.Headers.Count > 0) {
            return new HeaderedActionResult(result, response.Headers);
        }

        return result;
    }

    private static IActionResult MapErrorToActionResult(Failure           failure,
                                                        IFailureHttpMapper? customMapper)
    {
        var response = FailureHttpResponseResolver.Resolve(failure, customMapper);
        return FailureHttpResponseToActionResult(response);
    }

    /// <summary>
    ///     Converts an asynchronous Result to an IActionResult, providing appropriate HTTP responses
    ///     for success or failure cases.
    /// </summary>
    public static async ValueTask<IActionResult> ToActionResult(this ValueTask<Result>   resultTask,
                                                                Func<IActionResult>?     successHttpMapper = null,
                                                                IFailureHttpMapper?        failureMapper       = null,
                                                                ResultHttpAdapterPolicy? policy            = null)
    {
        successHttpMapper ??= BuildDefaultSuccessMapper(policy);
        var result = await resultTask;
        return result.Match(successHttpMapper, error => MapErrorToActionResult(error, failureMapper));
    }

    /// <summary>
    ///     Converts an asynchronous Result to an IActionResult.
    /// </summary>
    public static ValueTask<IActionResult> ToActionResult(this Task<Result>        resultTask,
                                                          Func<IActionResult>?     successHttpMapper = null,
                                                          IFailureHttpMapper?        failureMapper       = null,
                                                          ResultHttpAdapterPolicy? policy            = null)
    {
        return new ValueTask<Result>(resultTask).ToActionResult(successHttpMapper, failureMapper, policy);
    }

    /// <summary>
    ///     Converts an asynchronous Result to an IActionResult for use in ASP.NET Core endpoints.
    /// </summary>
    public static async ValueTask<IActionResult> ToActionResult<TValue>(this ValueTask<Result<TValue>> resultTask,
                                                                        Func<TValue, IActionResult>?   successHttpMapper = null,
                                                                        IFailureHttpMapper?              errorHttpMapper   = null)
        where TValue : notnull
    {
        successHttpMapper ??= v => new OkObjectResult(v);
        var result = await resultTask;
        return result.Match(
            value => successHttpMapper(value),
            error => MapErrorToActionResult(error, errorHttpMapper));
    }

    /// <summary>
    ///     Converts an asynchronous Result to an IActionResult for use in ASP.NET Core endpoints.
    /// </summary>
    public static ValueTask<IActionResult> ToActionResult<TValue>(this Task<Result<TValue>>    resultTask,
                                                                  Func<TValue, IActionResult>? successHttpMapper = null,
                                                                  IFailureHttpMapper?            errorHttpMapper   = null)
        where TValue : notnull
    {
        return new ValueTask<Result<TValue>>(resultTask).ToActionResult(successHttpMapper,
                                                                        errorHttpMapper);
    }

    /// <summary>
    ///     Custom IActionResult implementation that wraps another IActionResult and adds HTTP headers.
    /// </summary>
    private sealed class HeaderedActionResult : IActionResult
    {
        private readonly IReadOnlyDictionary<string, string> _headers;
        private readonly IActionResult                       _innerResult;

        public HeaderedActionResult(IActionResult                       innerResult,
                                    IReadOnlyDictionary<string, string> headers)
        {
            _innerResult = innerResult;
            _headers     = headers;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            // Add headers to the response
            foreach (var (key, value) in _headers) {
                context.HttpContext.Response.Headers[key] = value;
            }

            // Execute the inner result
            return _innerResult.ExecuteResultAsync(context);
        }
    }

    /// <param name="result">The result to convert.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Converts a Result to an IActionResult for use in minimal API endpoints.
        ///     Maps success to a provided custom success response or to a default response,
        ///     and maps failure to an appropriate status code or response using a mapper.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     An optional function mapping a successful Result to an IActionResult. If null, a default
        ///     response (e.g., NoContentResult) will be used for success cases.
        /// </param>
        /// <param name="failureMapper">
        ///     Optional custom implementation of IFailureHttpMapper for handling failures. If not provided,
        ///     a default mapper will be used to generate the error response.
        /// </param>
        /// <param name="policy">
        ///     Optional adapter policy controlling default success behavior when no success mapper is provided.
        /// </param>
        /// <returns>An IActionResult representing the outcome of a Result, based on either success or failure.</returns>
        public IActionResult ToActionResult(Func<IActionResult>?     successHttpMapper = null,
                                            IFailureHttpMapper?        failureMapper       = null,
                                            ResultHttpAdapterPolicy? policy            = null)
        {
            successHttpMapper ??= BuildDefaultSuccessMapper(policy);
            return result.Match(successHttpMapper, error => MapErrorToActionResult(error, failureMapper));
        }
    }

    /// <param name="result">The result to convert.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(Result<TValue> result)
        where TValue : notnull
    {
        /// <summary>
        ///     Converts a <see cref="Result{TValue}" /> to an <see cref="IActionResult" /> for use in ASP.NET Core minimal APIs.
        ///     Maps successful results to an HTTP response based on a custom success mapper or a default implementation,
        ///     and maps failure results to an error response using a custom or default error mapper.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     An optional function to convert a successful result value to an <see cref="IActionResult" />.
        ///     If not provided, a default implementation that maps to an <see cref="OkObjectResult" /> will be used.
        /// </param>
        /// <param name="errorHttpMapper">
        ///     An optional implementation of <see cref="IFailureHttpMapper" /> to handle error result mapping
        ///     into an HTTP response. If not provided, a default error mapper will be used.
        /// </param>
        /// <returns>An <see cref="IActionResult" /> representing the HTTP response for the given result.</returns>
        public IActionResult ToActionResult(Func<TValue, IActionResult>? successHttpMapper = null,
                                            IFailureHttpMapper?            errorHttpMapper   = null)
        {
            successHttpMapper ??= v => new OkObjectResult(v);
            return result.Match(
                value => successHttpMapper(value),
                error => MapErrorToActionResult(error, errorHttpMapper));
        }
    }
}
