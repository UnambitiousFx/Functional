using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;
using IHttpResult = Microsoft.AspNetCore.Http.IResult;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Extension methods for converting Result types to HTTP responses (IResult) for minimal APIs.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    ///     Converts a Result to an IResult for minimal API endpoints.
    ///     Success returns 200 OK, failure maps error to appropriate status code.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IResult representing the result.</returns>
    public static IHttpResult ToHttpResult(
        this Result result,
        IErrorHttpMapper? mapper = null)
    {
        return result.Match(
            () => HttpResults.NoContent(),
            error => MapErrorToHttpResult(error, mapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to an IResult for minimal API endpoints.
    ///     Success returns 200 OK with value, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IResult representing the result.</returns>
    public static IHttpResult ToHttpResult<TValue>(
        this Result<TValue> result,
        IErrorHttpMapper? mapper = null)
        where TValue : notnull
    {
        return result.Match(
            value => HttpResults.Ok(value),
            error => MapErrorToHttpResult(error, mapper));
    }

    /// <summary>
    ///     Converts a non-generic Result to an IResult, allowing a DTO to be mapped on success.
    ///     On success, returns 200 OK with the mapped DTO. On failure, maps the error to an appropriate HTTP status code.
    /// </summary>
    /// <param name="result">The Result to convert.</param>
    /// <param name="dtoMapper">A function that maps the Result to a DTO on success.</param>
    /// <param name="errorMapper">Optional custom error mapper. If not provided, a default mapper is used.</param>
    /// <returns>An IResult representing the converted HTTP response.</returns>
    public static IHttpResult ToHttpResult<TDto>(
        this Result result,
        Func<TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
    {
        ArgumentNullException.ThrowIfNull(dtoMapper);

        return result.Match(
            () => HttpResults.Ok(dtoMapper()),
            error => MapErrorToHttpResult(error, errorMapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to an IResult for minimal API endpoints with DTO mapping.
    ///     Success returns 200 OK with mapped DTO, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="dtoMapper">Function to map the value to a DTO.</param>
    /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IResult representing the result.</returns>
    public static IHttpResult ToHttpResult<TValue, TDto>(
        this Result<TValue> result,
        Func<TValue, TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(dtoMapper);

        return result.Match(
            value => HttpResults.Ok(dtoMapper(value)),
            error => MapErrorToHttpResult(error, errorMapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to a Created IResult for minimal API endpoints.
    ///     Success returns 201 Created with location and value, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="locationFactory">Function to generate the location URI from the value.</param>
    /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IResult representing the result.</returns>
    public static IHttpResult ToCreatedHttpResult<TValue>(
        this Result<TValue> result,
        Func<TValue, string> locationFactory,
        IErrorHttpMapper? mapper = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(locationFactory);

        return result.Match(
            value => HttpResults.Created(locationFactory(value), value),
            error => MapErrorToHttpResult(error, mapper));
    }

    /// <summary>
    ///     Converts a Result&lt;T&gt; to a Created IResult for minimal API endpoints with DTO mapping.
    ///     Success returns 201 Created with location and mapped DTO, failure maps error to appropriate status code.
    /// </summary>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="locationFactory">Function to generate the location URI from the value.</param>
    /// <param name="dtoMapper">Function to map the value to a DTO.</param>
    /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
    /// <returns>An IResult representing the result.</returns>
    public static IHttpResult ToCreatedHttpResult<TValue, TDto>(
        this Result<TValue> result,
        Func<TValue, string> locationFactory,
        Func<TValue, TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(locationFactory);
        ArgumentNullException.ThrowIfNull(dtoMapper);

        return result.Match(
            value => HttpResults.Created(locationFactory(value), dtoMapper(value)),
            error => MapErrorToHttpResult(error, errorMapper));
    }

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

    private static IHttpResult MapErrorToHttpResult(Error error, IErrorHttpMapper? customMapper)
    {
        var mappedResponse = customMapper?.GetResponse(error);
        if (mappedResponse != null)
            return ResponseToHttpResult(mappedResponse.Value.StatusCode, mappedResponse.Value.Body);

        var defaultResponse = DefaultErrorHttpMapper.Instance.GetResponse(error);
        if (defaultResponse is not null)
            return ResponseToHttpResult(defaultResponse.Value.StatusCode, defaultResponse.Value.Body);

        return HttpResults.StatusCode(StatusCodes.Status500InternalServerError);
    }
}