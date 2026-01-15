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

    /// <param name="result">The result to convert.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Converts a Result to an IResult for use in minimal API endpoints.
        ///     Maps success to the provided success result and failure to an appropriate status code or response body.
        /// </summary>
        /// <param name="httpMapper">A function mapping a successful Result to an IHttpResult.</param>
        /// <param name="errorMapper">
        ///     Optional custom mapper for handling error cases. Uses a default implementation if not
        ///     supplied.
        /// </param>
        /// <returns>An IHttpResult representing either success or failure.</returns>
        public IHttpResult ToHttpResult(Func<IHttpResult> httpMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.Match(httpMapper, error => MapErrorToHttpResult(error, errorMapper));
        }

        /// <summary>
        ///     Converts a Result to an IResult for minimal API endpoints.
        ///     Success returns 200 OK, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToHttpResult(IErrorHttpMapper? mapper = null)
        {
            return result.ToHttpResult(HttpResults.NoContent, mapper);
        }

        /// <summary>
        ///     Converts a non-generic Result to an IResult, allowing a DTO to be mapped on success.
        ///     On success, returns 200 OK with the mapped DTO. On failure, maps the error to an appropriate HTTP status code.
        /// </summary>
        /// <param name="dtoMapper">A function that maps the Result to a DTO on success.</param>
        /// <param name="errorMapper">Optional custom error mapper. If not provided, a default mapper is used.</param>
        /// <returns>An IResult representing the converted HTTP response.</returns>
        public IHttpResult ToHttpResult<TDto>(Func<TDto> dtoMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.ToHttpResult(dtoMapper, HttpResults.Ok, errorMapper);
        }

        /// <summary>
        ///     Converts a Result into an IHttpResult for use in minimal API endpoints.
        ///     On success, it maps the result to a DTO using the provided mapping function and converts it into an IHttpResult
        ///     using the success response mapper.
        ///     On failure, it utilizes an optional error mapper to map the error to an appropriate IHttpResult.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to map from the result.</typeparam>
        /// <param name="dtoMapper">A function that produces a DTO from the result when it is successful.</param>
        /// <param name="httpMapper">A function that converts the DTO to an IHttpResult when the result is successful.</param>
        /// <param name="errorMapper">
        ///     An optional mapper for customizing the error-to-IHttpResult mapping. If not provided, a
        ///     default mapping is used.
        /// </param>
        /// <returns>An IHttpResult representing either the success or failure of the provided result.</returns>
        public IHttpResult ToHttpResult<TValue>(Func<TValue> dtoMapper,
            Func<TValue, IHttpResult> httpMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.Match(() =>
            {
                var value = dtoMapper();
                return httpMapper(value);
            }, error => MapErrorToHttpResult(error, errorMapper));
        }
    }

    /// <param name="result">The result to convert.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Converts a Result&lt;T&gt; to an IResult for minimal API endpoints with DTO mapping.
        ///     Success returns 200 OK with mapped DTO, failure maps error to appropriate status code.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
        /// <param name="dtoMapper">Function to map the value to a DTO.</param>
        /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToHttpResult<TDto>(Func<TValue, TDto> dtoMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.ToHttpResult(dtoMapper, (_, v) => Results.Ok((object?)v), errorMapper);
        }

        /// <summary>
        ///     Converts a Result&lt;T&gt; to a Created IResult for minimal API endpoints.
        ///     Success returns 201 Created with location and value, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="locationFactory">Function to generate the location URI from the value.</param>
        /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToCreatedHttpResult(Func<TValue, string> locationFactory,
            IErrorHttpMapper? mapper = null)
        {
            return result.ToHttpResult(v => v, (v, _) => HttpResults.Created(locationFactory(v), v), mapper);
        }

        /// <summary>
        ///     Converts a Result&lt;T&gt; to a Created IResult for minimal API endpoints with DTO mapping.
        ///     Success returns 201 Created with location and mapped DTO, failure maps error to appropriate status code.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
        /// <param name="locationFactory">Function to generate the location URI from the value.</param>
        /// <param name="dtoMapper">Function to map the value to a DTO.</param>
        /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToCreatedHttpResult<TDto>(Func<TValue, string> locationFactory,
            Func<TValue, TDto> dtoMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.ToHttpResult(dtoMapper, (v, d) => HttpResults.Created(locationFactory(v), d), errorMapper);
        }

        /// <summary>
        ///     Converts a Result&lt;T&gt; to an IResult for minimal API endpoints.
        ///     Success returns 200 OK with value, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IHttpResult ToHttpResult(IErrorHttpMapper? mapper = null)
        {
            return result.ToHttpResult(HttpResults.Ok, mapper);
        }

        /// <summary>
        ///     Converts a <see cref="Result{TValue}" /> into an <see cref="IHttpResult" /> for minimal API responses.
        ///     Maps the result's success value to an HTTP response using the provided DTO and HTTP mappers, or maps errors to HTTP
        ///     responses using an error mapper.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO produced from the result value.</typeparam>
        /// <param name="dtoMapper">A function to transform the result's value into a DTO.</param>
        /// <param name="httpMapper">A function to create an HTTP result from the result value and its mapped DTO.</param>
        /// <param name="errorMapper">
        ///     An optional mapper to handle errors and convert them into HTTP responses. Defaults to a
        ///     predefined implementation if not provided.
        /// </param>
        /// <returns>An <see cref="IHttpResult" /> representing the HTTP response for the result.</returns>
        public IHttpResult ToHttpResult<TDto>(Func<TValue, TDto> dtoMapper,
            Func<TValue, TDto, IHttpResult> httpMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.Match(resultValue =>
            {
                var value = dtoMapper(resultValue);
                return httpMapper(resultValue, value);
            }, error => MapErrorToHttpResult(error, errorMapper));
        }
    }
}