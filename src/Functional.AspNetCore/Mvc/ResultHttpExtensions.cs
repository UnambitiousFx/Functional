using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    private static ObjectResult ProblemDetailToActionResult(ProblemDetails details)
    {
        return new ObjectResult(details)
        {
            StatusCode = details.Status ?? StatusCodes.Status500InternalServerError
        };
    }

    private static IActionResult DefaultActionResult(int statusCode, object? body)
    {
        if (body is null)
        {
            return new StatusCodeResult(statusCode);
        }

        return new ObjectResult(body) { StatusCode = statusCode };
    }

    private static IActionResult BodyToActionResult(int statusCode, object? body)
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

    private static IActionResult ResponseToActionResult(int statusCode, object? body)
    {
        return body switch
        {
            null => new StatusCodeResult(statusCode),
            ProblemDetails problemDetails => ProblemDetailToActionResult(problemDetails),
            _ => BodyToActionResult(statusCode, body)
        };
    }

    private static IActionResult ErrorHttpResponseToActionResult(ErrorHttpResponse response)
    {
        var result = ResponseToActionResult(response.StatusCode, response.Body);

        // Apply headers if present
        if (response.Headers is not null && response.Headers.Count > 0)
        {
            return new HeaderedActionResult(result, response.Headers);
        }

        return result;
    }

    private static IActionResult MapErrorToActionResult(Failure failure, IErrorHttpMapper? customMapper)
    {
        var mappedResponse = customMapper?.GetErrorResponse(failure);
        if (mappedResponse != null)
        {
            return ErrorHttpResponseToActionResult(mappedResponse);
        }

        var defaultResponse = DefaultErrorHttpMapper.Instance.GetErrorResponse(failure);
        if (defaultResponse is not null)
        {
            return ErrorHttpResponseToActionResult(defaultResponse);
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    ///     Custom IActionResult implementation that wraps another IActionResult and adds HTTP headers.
    /// </summary>
    private sealed class HeaderedActionResult : IActionResult
    {
        private readonly IActionResult _innerResult;
        private readonly IReadOnlyDictionary<string, string> _headers;

        public HeaderedActionResult(IActionResult innerResult, IReadOnlyDictionary<string, string> headers)
        {
            _innerResult = innerResult;
            _headers = headers;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            // Add headers to the response
            foreach (var (key, value) in _headers)
            {
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
        /// <param name="errorMapper">
        ///     Optional custom implementation of IErrorHttpMapper for handling failures. If not provided,
        ///     a default mapper will be used to generate the error response.
        /// </param>
        /// <returns>An IActionResult representing the outcome of a Result, based on either success or failure.</returns>
        public IActionResult ToActionResult(
            Func<IActionResult>? successHttpMapper = null,
            IErrorHttpMapper? errorMapper = null)
        {
            successHttpMapper ??= () => new NoContentResult();
            return result.Match(successHttpMapper, error => MapErrorToActionResult(error, errorMapper));
        }
    }

    extension(ResultTask resultTask)
    {
        /// <summary>
        ///     Converts a ResultTask to an IActionResult, providing appropriate HTTP responses
        ///     for success or failure cases. Allows customizing the success and failure response handling.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     An optional function defining the IActionResult to return for a successful ResultTask.
        ///     If null, a default NoContentResult is used as the success response.
        /// </param>
        /// <param name="errorMapper">
        ///     Optional implementation of IErrorHttpMapper for customizing the behavior of mapping
        ///     failures to IActionResult. If null, the default failure mapper is used.
        /// </param>
        /// <returns>
        ///     A ValueTask resulting in an IActionResult based on the ResultTask's success or failure state.
        /// </returns>
        public ValueTask<IActionResult> ToActionResult(
            Func<IActionResult>? successHttpMapper = null,
            IErrorHttpMapper? errorMapper = null)
        {
            successHttpMapper ??= () => new NoContentResult();
            return resultTask.Match(successHttpMapper, error => MapErrorToActionResult(error, errorMapper));
        }
    }

    /// <param name="result">The result to convert.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Converts a Result&lt;TValue&gt; to a CreatedAtAction IActionResult for MVC controllers.
        ///     On success, returns 201 Created with location and value. On failure, maps error to an appropriate HTTP status code
        ///     and response using the provided error mapper or a default one.
        /// </summary>
        /// <param name="actionName">
        ///     The name of the action to include in the CreatedAtActionResult for generating the URI of the newly created
        ///     resource.
        /// </param>
        /// <param name="controllerName">
        ///     The name of the controller containing the action. Can be null if the action is in the current controller.
        /// </param>
        /// <param name="routeValues">
        ///     An object containing the route values for generating the URI. Can be null if no additional route values are
        ///     required.
        /// </param>
        /// <param name="mapper">
        ///     An optional custom implementation of IErrorHttpMapper for handling failures. If not provided, a default mapper will
        ///     be used to determine the error response.
        /// </param>
        /// <returns>
        ///     An IActionResult representing either a 201 Created result for success or an appropriate error response for
        ///     failure.
        /// </returns>
        public IActionResult ToCreatedActionResult(string actionName,
            string? controllerName,
            object? routeValues = null,
            IErrorHttpMapper? mapper = null)
        {
            return result.ToActionResult(
                v => new CreatedAtActionResult(actionName, controllerName, routeValues, v),
                mapper);
        }

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
        ///     An optional implementation of <see cref="IErrorHttpMapper" /> to handle error result mapping
        ///     into an HTTP response. If not provided, a default error mapper will be used.
        /// </param>
        /// <returns>An <see cref="IActionResult" /> representing the HTTP response for the given result.</returns>
        public IActionResult ToActionResult(
            Func<TValue, IActionResult>? successHttpMapper = null,
            IErrorHttpMapper? errorHttpMapper = null)
        {
            successHttpMapper ??= v => new OkObjectResult(v);
            return result.Match(
                value => successHttpMapper(value),
                error => MapErrorToActionResult(error, errorHttpMapper));
        }
    }

    extension<TValue>(ResultTask<TValue> resultTask) where TValue : notnull
    {
        /// <summary>
        ///     Converts a <see cref="ResultTask{TValue}" /> to an <see cref="IActionResult" /> that represents
        ///     a CreatedAtActionResult. This is used in minimal API endpoints to map success results
        ///     to a "201 Created" response with location headers, and failures to appropriate error responses.
        /// </summary>
        /// <param name="actionName">
        ///     The name of the action method that is called in the CreatedAtActionResult.
        /// </param>
        /// <param name="controllerName">
        ///     The name of the controller containing the action method. May be null to use the current controller.
        /// </param>
        /// <param name="routeValues">
        ///     The route data used to generate the URL for the location header. May be null.
        /// </param>
        /// <param name="mapper">
        ///     An optional custom implementation of <see cref="IErrorHttpMapper" /> for mapping errors
        ///     to HTTP status codes and response bodies. If null, a default mapper is used.
        /// </param>
        /// <returns>
        ///     A <see cref="ValueTask{IActionResult}" /> that resolves to an IActionResult representing
        ///     either a "201 Created" response for success or an appropriate error response for failure.
        /// </returns>
        public ValueTask<IActionResult> ToCreatedActionResult(string actionName,
            string? controllerName,
            object? routeValues = null,
            IErrorHttpMapper? mapper = null)
        {
            return resultTask.ToActionResult(
                v => new CreatedAtActionResult(actionName, controllerName, routeValues, v),
                mapper);
        }

        /// <summary>
        ///     Converts a ResultTask to an IActionResult for use in ASP.NET Core endpoints.
        ///     Maps success to a provided custom success response or to a default response,
        ///     and maps failure to an appropriate status code or response using a mapper.
        /// </summary>
        /// <param name="successHttpMapper">
        ///     An optional function mapping a successful ResultTask to an IActionResult. If null, a default
        ///     response (e.g., OkObjectResult) will be used for success cases.
        /// </param>
        /// <param name="errorHttpMapper">
        ///     Optional custom implementation of IErrorHttpMapper for handling failures. If not provided,
        ///     a default mapper will be used to generate the error response.
        /// </param>
        /// <returns>
        ///     A ValueTask containing an IActionResult representing the outcome of a ResultTask, based on either success or
        ///     failure.
        /// </returns>
        public ValueTask<IActionResult> ToActionResult(
            Func<TValue, IActionResult>? successHttpMapper = null,
            IErrorHttpMapper? errorHttpMapper = null)
        {
            successHttpMapper ??= v => new OkObjectResult(v);
            return resultTask.Match(
                value => successHttpMapper(value),
                error => MapErrorToActionResult(error, errorHttpMapper));
        }
    }
}