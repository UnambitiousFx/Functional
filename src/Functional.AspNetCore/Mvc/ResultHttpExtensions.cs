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
        if (body is null) return new StatusCodeResult(statusCode);
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

    private static IActionResult MapErrorToActionResult(Failure failure, IErrorHttpMapper? customMapper)
    {
        var mappedResponse = customMapper?.GetResponse(failure);
        if (mappedResponse != null)
            return ResponseToActionResult(mappedResponse.Value.StatusCode, mappedResponse.Value.Body);

        var defaultResponse = DefaultErrorHttpMapper.Instance.GetResponse(failure);
        if (defaultResponse is not null)
            return ResponseToActionResult(defaultResponse.Value.StatusCode, defaultResponse.Value.Body);

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    /// <param name="result">The result to convert.</param>
    extension(Result result)
    {
        /// <summary>
        ///     Converts a Result to an IResult for use in minimal API endpoints.
        ///     Maps success to the provided success result and failure to an appropriate status code or response body.
        /// </summary>
        /// <param name="httpMapper">A function mapping a successful Result to an IActionResult.</param>
        /// <param name="errorMapper">
        ///     Optional custom mapper for handling error cases. Uses a default implementation if not
        ///     supplied.
        /// </param>
        /// <returns>An IActionResult representing either success or failure.</returns>
        public IActionResult ToActionResult(Func<IActionResult> httpMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.Match(httpMapper, error => MapErrorToActionResult(error, errorMapper));
        }

        /// <summary>
        ///     Converts a Result to an IResult for minimal API endpoints.
        ///     Success returns 200 OK, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IActionResult ToActionResult(IErrorHttpMapper? errorMapper = null)
        {
            return result.ToActionResult(httpMapper: () => new NoContentResult(), errorMapper);
        }

        /// <summary>
        ///     Converts a non-generic Result to an IResult, allowing a DTO to be mapped on success.
        ///     On success, returns 200 OK with the mapped DTO. On failure, maps the error to an appropriate HTTP status code.
        /// </summary>
        /// <param name="dtoMapper">A function that maps the Result to a DTO on success.</param>
        /// <param name="errorMapper">Optional custom error mapper. If not provided, a default mapper is used.</param>
        /// <returns>An IResult representing the converted HTTP response.</returns>
        public IActionResult ToActionResult<TDto>(Func<TDto> dtoMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.ToActionResult(dtoMapper, x => new OkObjectResult(x), errorMapper);
        }

        /// <summary>
        ///     Converts a Result into an IActionResult for use in minimal API endpoints.
        ///     On success, it maps the result to a DTO using the provided mapping function and converts it into an IActionResult
        ///     using the success response mapper.
        ///     On failure, it utilizes an optional error mapper to map the error to an appropriate IActionResult.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to map from the result.</typeparam>
        /// <param name="dtoMapper">A function that produces a DTO from the result when it is successful.</param>
        /// <param name="httpMapper">A function that converts the DTO to an IActionResult when the result is successful.</param>
        /// <param name="errorMapper">
        ///     An optional mapper for customizing the error-to-IActionResult mapping. If not provided, a
        ///     default mapping is used.
        /// </param>
        /// <returns>An IActionResult representing either the success or failure of the provided result.</returns>
        public IActionResult ToActionResult<TValue>(Func<TValue> dtoMapper,
            Func<TValue, IActionResult> httpMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.Match(() =>
            {
                var value = dtoMapper();
                return httpMapper(value);
            }, error => MapErrorToActionResult(error, errorMapper));
        }
    }

    /// <param name="result">The result to convert.</param>
    /// <typeparam name="TValue">The type of the success value.</typeparam>
    extension<TValue>(Result<TValue> result) where TValue : notnull
    {
        /// <summary>
        ///     Converts a Result&lt;T&gt; to a CreatedAtAction IActionResult for MVC controllers.
        ///     Success returns 201 Created with location and value, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="actionName">Name of the action to link to.</param>
        /// <param name="routeValues">Route values for the action.</param>
        /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IActionResult representing the result.</returns>
        public IActionResult ToCreatedActionResult(string actionName,
            object? routeValues = null,
            IErrorHttpMapper? mapper = null)
        {
            return result.ToActionResult(v => v, (_, v) => new CreatedAtActionResult(actionName, null, routeValues, v),
                mapper);
        }

        /// <summary>
        ///     Converts a Result&lt;T&gt; to a CreatedAtAction IActionResult for MVC controllers with DTO mapping.
        ///     Success returns 201 Created with location and mapped DTO, failure maps error to appropriate status code.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
        /// <param name="actionName">Name of the action to link to.</param>
        /// <param name="dtoMapper">Function to map the value to a DTO.</param>
        /// <param name="routeValues">Route values for the action.</param>
        /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IActionResult representing the result.</returns>
        public IActionResult ToCreatedActionResult<TDto>(string actionName,
            Func<TValue, TDto> dtoMapper,
            object? routeValues = null,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.ToActionResult(dtoMapper,
                (_, d) => new CreatedAtActionResult(actionName, null, routeValues, d), errorMapper);
        }

        /// <summary>
        ///     Converts a Result&lt;T&gt; to an IResult for minimal API endpoints with DTO mapping.
        ///     Success returns 200 OK with mapped DTO, failure maps error to appropriate status code.
        /// </summary>
        /// <typeparam name="TDto">The type of the DTO to map to.</typeparam>
        /// <param name="dtoMapper">Function to map the value to a DTO.</param>
        /// <param name="errorMapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IActionResult ToActionResult<TDto>(Func<TValue, TDto> dtoMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.ToActionResult(dtoMapper, (_, v) => new OkObjectResult(v), errorMapper);
        }

        /// <summary>
        ///     Converts a Result&lt;T&gt; to an IResult for minimal API endpoints.
        ///     Success returns 200 OK with value, failure maps error to appropriate status code.
        /// </summary>
        /// <param name="mapper">Optional custom error mapper. Uses default if not provided.</param>
        /// <returns>An IResult representing the result.</returns>
        public IActionResult ToActionResult(IErrorHttpMapper? mapper = null)
        {
            return result.ToActionResult(v => v, (_, v) => new OkObjectResult(v), mapper);
        }

        /// <summary>
        ///     Converts a <see cref="Result{TValue}" /> into an <see cref="IActionResult" /> for minimal API responses.
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
        /// <returns>An <see cref="IActionResult" /> representing the HTTP response for the result.</returns>
        public IActionResult ToActionResult<TDto>(Func<TValue, TDto> dtoMapper,
            Func<TValue, TDto, IActionResult> httpMapper,
            IErrorHttpMapper? errorMapper = null)
        {
            return result.Match(resultValue =>
            {
                var value = dtoMapper(resultValue);
                return httpMapper(resultValue, value);
            }, error => MapErrorToActionResult(error, errorMapper));
        }
    }
}