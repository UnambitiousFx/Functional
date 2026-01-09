using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Provides extension methods for handling Result objects in the context of ASP.NET Core MVC.
///     These methods convert Result and Result&lt;T&gt; instances to IActionResult, supporting both
///     success and error scenarios with mapping to HTTP status codes.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    ///     Converts a Result to an IActionResult for MVC controllers.
    ///     Success returns 200 OK, failure maps error to appropriate status code.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IActionResult representing the result.</returns>
    public static IActionResult ToActionResult(
        this Result result,
        IErrorHttpMapper? mapper = null)
    {
        return result.Match(
            () => new NoContentResult(),
            error => MapErrorToActionResult(error, mapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to an IActionResult for MVC controllers.
    ///     Success returns 200 OK with value, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IActionResult representing the result.</returns>
    public static IActionResult ToActionResult<TValue>(
        this Result<TValue> result,
        IErrorHttpMapper? mapper = null)
        where TValue : notnull
    {
        return result.Match(
            value => new OkObjectResult(value),
            error => MapErrorToActionResult(error, mapper));
    }

    /// <summary>
    ///     Converts a Result instance to an IActionResult for use in MVC controllers.
    ///     On success, the specified DTO mapper is used to project the result to a response payload.
    ///     On failure, the error is mapped to an appropriate HTTP response using the provided or default error mapper.
    /// </summary>
    /// <typeparam name="TDto">The type of the output DTO for successful results.</typeparam>
    /// <param name="result">The Result instance to convert.</param>
    /// <param name="dtoMapper">A function to map the successful result to a DTO.</param>
    /// <param name="errorMapper">
    ///     An optional custom error-to-HTTP mapping strategy. Defaults to the standard mapper if not
    ///     provided.
    /// </param>
    /// <returns>An IActionResult representing either a successful HTTP 200 response or a failure based on the error mapper.</returns>
    public static IActionResult ToActionResult<TDto>(
        this Result result,
        Func<TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
    {
        ArgumentNullException.ThrowIfNull(dtoMapper);

        return result.Match(
            () => new OkObjectResult(dtoMapper()),
            error => MapErrorToActionResult(error, errorMapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to an IActionResult for MVC controllers with DTO mapping.
    ///     Success returns 200 OK with mapped DTO, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="dtoMapper">Function to map the value to a DTO.</param>
    /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IActionResult representing the result.</returns>
    public static IActionResult ToActionResult<TValue, TDto>(
        this Result<TValue> result,
        Func<TValue, TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(dtoMapper);

        return result.Match(
            value => new OkObjectResult(dtoMapper(value)),
            error => MapErrorToActionResult(error, errorMapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to a CreatedAtAction IActionResult for MVC controllers.
    ///     Success returns 201 Created with location and value, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="actionName">Name of the action to link to.</param>
    /// <param name="routeValues">Route values for the action.</param>
    /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IActionResult representing the result.</returns>
    public static IActionResult ToCreatedActionResult<TValue>(
        this Result<TValue> result,
        string actionName,
        object? routeValues = null,
        IErrorHttpMapper? mapper = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(actionName);

        return result.Match(
            value => new CreatedAtActionResult(actionName, null, routeValues, value),
            error => MapErrorToActionResult(error, mapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to a CreatedAtAction IActionResult for MVC controllers with DTO mapping.
    ///     Success returns 201 Created with location and mapped DTO, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="actionName">Name of the action to link to.</param>
    /// <param name="dtoMapper">Function to map the value to a DTO.</param>
    /// <param name="routeValues">Route values for the action.</param>
    /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IActionResult representing the result.</returns>
    public static IActionResult ToCreatedActionResult<TValue, TDto>(
        this Result<TValue> result,
        string actionName,
        Func<TValue, TDto> dtoMapper,
        object? routeValues = null,
        IErrorHttpMapper? errorMapper = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(actionName);
        ArgumentNullException.ThrowIfNull(dtoMapper);

        return result.Match(
            value => new CreatedAtActionResult(actionName, null, routeValues, dtoMapper(value)),
            error => MapErrorToActionResult(error, errorMapper));
    }

    private static ObjectResult ProblemDetailToActionResult(ProblemDetails details)
    {
        return new ObjectResult(details)
        {
            StatusCode = details.Status ?? StatusCodes.Status500InternalServerError
        };
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
            _ => new StatusCodeResult(statusCode)
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

    private static IActionResult MapErrorToActionResult(Error error, IErrorHttpMapper? customMapper)
    {
        var mappedResponse = customMapper?.GetResponse(error);
        if (mappedResponse != null)
            return ResponseToActionResult(mappedResponse.Value.StatusCode, mappedResponse.Value.Body);

        var defaultResponse = DefaultErrorHttpMapper.Instance.GetResponse(error);
        if (defaultResponse is not null)
            return ResponseToActionResult(defaultResponse.Value.StatusCode, defaultResponse.Value.Body);

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}