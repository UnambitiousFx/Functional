using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    private static IHttpResult ProblemDetailToActionResult(ProblemDetails details)
    {
        return HttpResults.Problem(details);
    }

    private static IHttpResult BodyToActionResult(int statusCode, object? body)
    {
        return statusCode switch
        {
            400 => HttpResults.BadRequest(body),
            401 => HttpResults.Unauthorized(),
            403 => HttpResults.Forbid(),
            404 => HttpResults.NotFound(body),
            409 => HttpResults.Conflict(body),
            500 => HttpResults.Problem(statusCode: statusCode),
            _ => HttpResults.StatusCode(statusCode)
        };
    }

    private static IHttpResult ResponseToHttpResult(int statusCode, object? body)
    {
        return body switch
        {
            null => HttpResults.StatusCode(statusCode),
            ProblemDetails problemDetails => ProblemDetailToActionResult(problemDetails),
            _ => BodyToActionResult(statusCode, body)
        };
    }

    private static IHttpResult ErrorHttpResponseToHttpResult(ErrorHttpResponse response)
    {
        var result = ResponseToHttpResult(response.StatusCode, response.Body);

        // Apply headers if present
        if (response.Headers is not null && response.Headers.Count > 0)
        {
            // For minimal APIs, we need to use TypedResults or Results.Extensions to add headers
            // Since IResult doesn't expose headers directly, we wrap in a custom result
            return new HeaderedHttpResult(result, response.Headers);
        }

        return result;
    }

    private static IHttpResult MapErrorToHttpResult(Failure failure, IErrorHttpMapper? customMapper)
    {
        var mappedResponse = customMapper?.GetErrorResponse(failure);
        if (mappedResponse != null)
        {
            return ErrorHttpResponseToHttpResult(mappedResponse);
        }

        var defaultResponse = DefaultErrorHttpMapper.Instance.GetErrorResponse(failure);
        if (defaultResponse is not null)
        {
            return ErrorHttpResponseToHttpResult(defaultResponse);
        }

        return HttpResults.StatusCode(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    ///     Custom IResult implementation that wraps another IResult and adds HTTP headers.
    /// </summary>
    private sealed class HeaderedHttpResult : IHttpResult
    {
        private readonly IHttpResult _innerResult;
        private readonly IReadOnlyDictionary<string, string> _headers;

        public HeaderedHttpResult(IHttpResult innerResult, IReadOnlyDictionary<string, string> headers)
        {
            _innerResult = innerResult;
            _headers = headers;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            // Add headers to the response
            foreach (var (key, value) in _headers)
            {
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
        /// <returns>An IResult representing either a successful or failed result mapped to an appropriate HTTP response.</returns>
        public IHttpResult ToHttpResult(
            Func<IHttpResult>? successHttpMapper = null,
            IErrorHttpMapper? errorHttpMapper = null)
        {
            return successHttpMapper is not null
                ? result.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorHttpMapper))
                : result.Match(HttpResults.NoContent, error => MapErrorToHttpResult(error, errorHttpMapper));
        }
    }

    extension(ResultTask resultTask)
    {
        /// <summary>
        ///     Converts a <see cref="ResultTask" /> into an <see cref="Microsoft.AspNetCore.Http.IResult" /> for use with minimal
        ///     API endpoints, mapping success and failure to corresponding HTTP responses.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     An optional function to define the HTTP response when the operation is successful. Defaults to a NoContent response
        ///     if not provided.
        /// </param>
        /// <param name="errorHttpMapper">
        ///     An optional custom error mapper to translate failure results into specific HTTP responses. Uses the default error
        ///     mapper if not provided.
        /// </param>
        /// <returns>
        ///     A <see cref="Microsoft.AspNetCore.Http.IResult" /> representing either a successful result or a mapped failure
        ///     response.
        /// </returns>
        public ValueTask<IHttpResult> ToHttpResult(
            Func<IHttpResult>? successHttpMapper = null,
            IErrorHttpMapper? errorHttpMapper = null)
        {
            successHttpMapper ??= HttpResults.NoContent;
            return resultTask.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorHttpMapper));
        }
    }

    /// <param name="result">The result to convert.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Converts a Result&lt;T&gt; to a Created IResult for minimal API endpoints.
        ///     Success returns 201 Created with location and value, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="locationFactory">Function to generate the location URI from the value.</param>
        /// <param name="errorHttpMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToCreatedHttpResult(
            Func<TValue, string> locationFactory,
            IErrorHttpMapper? errorHttpMapper = null)
        {
            return result.ToHttpResult(v => HttpResults.Created(locationFactory(v), v), errorHttpMapper);
        }

        /// <summary>
        ///     Converts a <see cref="Result{TValue}" /> into an <see cref="IHttpResult" /> for minimal API responses.
        ///     Maps the result's success value to an HTTP response using the provided DTO and HTTP mappers, or maps errors to HTTP
        ///     responses using an error mapper.
        /// </summary>
        /// <param name="successHttpMapper">A function to create an HTTP result from the result value and its mapped DTO.</param>
        /// <param name="errorHttpMapper">
        ///     An optional mapper to handle errors and convert them into HTTP responses. Defaults to a
        ///     predefined implementation if not provided.
        /// </param>
        /// <returns>An <see cref="IHttpResult" /> representing the HTTP response for the result.</returns>
        public IHttpResult ToHttpResult(
            Func<TValue, IHttpResult> successHttpMapper,
            IErrorHttpMapper? errorHttpMapper = null)
        {
            return result.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorHttpMapper));
        }
    }

    extension<TValue>(ResultTask<TValue> resultTask) where TValue : notnull
    {
        /// <summary>
        ///     Converts a ResultTask to an HTTP 201 Created response, including a "Location" header derived from the provided
        ///     location factory.
        /// </summary>
        /// <param name="locationFactory">
        ///     A function that generates the URI for the "Location" header from the successful result value.
        /// </param>
        /// <param name="mapper">
        ///     An optional error mapper to convert failure results into appropriate HTTP responses. If not provided, a default
        ///     error handler is used.
        /// </param>
        /// <returns>
        ///     A ValueTask containing an IHttpResult, which is an HTTP 201 Created response on success, or an error-specific
        ///     response on failure.
        /// </returns>
        public ValueTask<IHttpResult> ToCreatedHttpResult(
            Func<TValue, string> locationFactory,
            IErrorHttpMapper? mapper = null)
        {
            return resultTask.ToHttpResult(v => HttpResults.Created(locationFactory(v), v), mapper);
        }


        /// <summary>
        ///     Converts a <see cref="ResultTask{TValue}" /> into an <see cref="IHttpResult" /> for minimal API endpoints,
        ///     allowing customization of success and failure HTTP responses.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     A function to map successful result values to an <see cref="IHttpResult" />.
        /// </param>
        /// <param name="errorMapper">
        ///     Optional custom error mapper to translate failures into HTTP responses. If not provided, the default mapper is
        ///     used.
        /// </param>
        /// <returns>
        ///     An asynchronous <see cref="ValueTask{TResult}" /> resolving to an <see cref="IHttpResult" /> representing the
        ///     result.
        /// </returns>
        public ValueTask<IHttpResult> ToHttpResult(
            Func<TValue, IHttpResult>? successHttpMapper = null,
            IErrorHttpMapper? errorMapper = null)
        {
            successHttpMapper ??= HttpResults.Ok;
            return resultTask.Match(successHttpMapper, error => MapErrorToHttpResult(error, errorMapper));
        }
    }
}