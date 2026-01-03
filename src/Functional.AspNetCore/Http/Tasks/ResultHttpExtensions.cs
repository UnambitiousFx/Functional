using UnambitiousFx.Functional.AspNetCore.Mappers;
using HttpResult = Microsoft.AspNetCore.Http.IResult;

namespace UnambitiousFx.Functional.AspNetCore.Http.Tasks;

/// <summary>
///     Provides extension methods for converting <see cref="Result" /> and <see cref="Task" /> of
///     <see cref="Result" />
///     into HTTP-compatible results using <see cref="Microsoft.AspNetCore.Http.IResult" />.
/// </summary>
public static class ResultHttpExtensions
{
    /// <summary>
    ///     Converts a <see cref="Task" /> of <see cref="Result" /> to an HTTP-compatible result asynchronously.
    /// </summary>
    /// <param name="awaitableResult">
    ///     The <see cref="Task" /> containing a <see cref="Result" /> to be converted to an HTTP result.
    /// </param>
    /// <param name="mapper">
    ///     An optional <see cref="IErrorHttpMapper" /> instance used to map errors in the <see cref="Result" /> to HTTP status
    ///     codes and response bodies.
    ///     If null, a default mapper is used.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> of <see cref="Microsoft.AspNetCore.Http.IResult" /> representing the HTTP response
    ///     derived from the result.
    /// </returns>
    public static async Task<HttpResult> ToHttpResultAsync(
        this Task<Result> awaitableResult,
        IErrorHttpMapper? mapper = null)
    {
        var result = await awaitableResult;
        return result.ToHttpResult(mapper);
    }

    /// <summary>
    ///     Converts an awaitable <see cref="Task{TResult}" /> of type <see cref="Result{TValue}" />
    ///     to an HTTP result by utilizing an optional error mapper.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained within the result.</typeparam>
    /// <param name="awaitableResult">The awaitable result instance to be converted.</param>
    /// <param name="mapper">
    ///     An optional instance of <see cref="IErrorHttpMapper" /> to customize the mapping
    ///     of errors to HTTP responses. If not provided, a default mapper is used.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation, containing an <see cref="Microsoft.AspNetCore.Http.IResult" />
    ///     that represents the mapped HTTP response.
    /// </returns>
    public static async Task<HttpResult> ToHttpResultAsync<TValue>(
        this Task<Result<TValue>> awaitableResult,
        IErrorHttpMapper? mapper = null)
        where TValue : notnull
    {
        var result = await awaitableResult;
        return result.ToHttpResult(mapper);
    }

    /// <summary>
    /// Converts a <see cref="ValueTask" /> of <see cref="Result" /> to an HTTP-compatible result asynchronously.
    /// </summary>
    /// <param name="awaitableResult">
    /// The <see cref="ValueTask" /> containing a <see cref="Result" /> to be converted to an HTTP result.
    /// </param>
    /// <param name="dtoMapper">
    /// A function that produces a DTO object for representing successful results in the HTTP response.
    /// </param>
    /// <param name="errorMapper">
    /// An optional <see cref="IErrorHttpMapper" /> instance used to map errors in the <see cref="Result" /> to HTTP status
    /// codes and response bodies. If null, a default mapper is used.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask" /> of <see cref="Microsoft.AspNetCore.Http.IResult" /> representing the HTTP response
    /// derived from the result.
    /// </returns>
    public static async ValueTask<HttpResult> ToHttpResultAsync<TDto>(this ValueTask<Result> awaitableResult,
        Func<TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
    {
        var result = await awaitableResult;
        return result.ToHttpResult(dtoMapper, errorMapper);
    }

    /// <summary>
    ///     Converts a <see cref="Task{TResult}" /> representing an asynchronous operation result
    ///     to an HTTP result using a mapping function for the successful value type
    ///     and an optional mapper for errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the successful result value.</typeparam>
    /// <typeparam name="TDto">The type to which the successful value will be mapped in the response.</typeparam>
    /// <param name="awaitableResult">The asynchronous result to be converted to an HTTP result.</param>
    /// <param name="dtoMapper">A function that maps the successful result value to a DTO type for the response.</param>
    /// <param name="errorMapper">
    ///     An optional error mapper used to map errors to HTTP status codes and response bodies.
    ///     If null, a default mapper is used.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that resolves to an <see cref="Microsoft.AspNetCore.Http.IResult" />
    ///     representing the HTTP response.
    /// </returns>
    public static async Task<HttpResult> ToHttpResultAsync<TValue, TDto>(
        this Task<Result<TValue>> awaitableResult,
        Func<TValue, TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
        where TValue : notnull
    {
        var result = await awaitableResult;
        return result.ToHttpResult(dtoMapper, errorMapper);
    }

    /// <summary>
    ///     Converts an asynchronous <see cref="Result{TValue}" /> operation to an HTTP 201 Created result,
    ///     including the location of the created resource and optionally mapping errors to HTTP responses.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
    /// <param name="awaitableResult">The asynchronous result of the operation to be converted.</param>
    /// <param name="locationFactory">
    ///     A function that generates the location URI for the created resource based on the result's value.
    /// </param>
    /// <param name="mapper">
    ///     An optional mapper for converting errors into HTTP status codes and response bodies.
    ///     If null, a default error mapping will be used.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous conversion operation.
    ///     The result is an HTTP 201 Created response if the operation was successful,
    ///     or an appropriate HTTP error response if the operation failed.
    /// </returns>
    public static async Task<HttpResult> ToCreatedHttpResultAsync<TValue>(
        this Task<Result<TValue>> awaitableResult,
        Func<TValue, string> locationFactory,
        IErrorHttpMapper? mapper = null)
        where TValue : notnull
    {
        var result = await awaitableResult;
        return result.ToCreatedHttpResult(locationFactory, mapper);
    }

    /// <summary>
    ///     Converts a Task of Result into an HTTP 201 Created response with a DTO payload and
    ///     location header.
    ///     This method processes the result asynchronously, applying a provided location factory, DTO mapping function,
    ///     and an optional HTTP-specific error mapper to generate an appropriate response. The resulting HTTP response will
    ///     include a "Location" header set by the <paramref name="locationFactory" /> and a payload transformed by the
    ///     <paramref name="dtoMapper" />.
    ///     If the result is a failure, the <paramref name="errorMapper" /> is used to generate an appropriate error response.
    /// </summary>
    /// <typeparam name="TValue">The type of the value contained in the result.</typeparam>
    /// <typeparam name="TDto">The type of the DTO to which the result is mapped.</typeparam>
    /// <param name="awaitableResult">The asynchronous result to be converted into an HTTP response.</param>
    /// <param name="locationFactory">
    ///     A function that generates the location URL for the "Location" header from the result
    ///     value.
    /// </param>
    /// <param name="dtoMapper">A function that maps the result value into a DTO.</param>
    /// <param name="errorMapper">An optional error mapper for generating appropriate error responses from failures.</param>
    /// <returns>A task that represents the asynchronous operation and results in an HTTP response.</returns>
    public static async Task<HttpResult> ToCreatedHttpResultAsync<TValue, TDto>(
        this Task<Result<TValue>> awaitableResult,
        Func<TValue, string> locationFactory,
        Func<TValue, TDto> dtoMapper,
        IErrorHttpMapper? errorMapper = null)
        where TValue : notnull
    {
        var result = await awaitableResult;
        return result.ToCreatedHttpResult(locationFactory, dtoMapper, errorMapper);
    }
}
