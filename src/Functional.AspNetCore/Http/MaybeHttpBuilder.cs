using System.Runtime.CompilerServices;
using IHttpResult = Microsoft.AspNetCore.Http.IResult;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace UnambitiousFx.Functional.AspNetCore.Http;

/// <summary>
///     Fluent HTTP response builder for <see cref="Maybe{TValue}" /> with a response formatter.
/// </summary>
/// <typeparam name="TValue">The type of the Maybe value.</typeparam>
/// <typeparam name="TResponse">The type of the HTTP response body.</typeparam>
public class MaybeHttpBuilder<TValue, TResponse>
    where TValue : notnull
{
    private readonly ValueTask<Maybe<TValue>> _awaitableMaybe;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly Func<TValue, string>? _locationFactory;
    private readonly Func<IHttpResult>? _noneMapper;
    private readonly Func<TValue, TResponse>? _responseFormatter;
    private readonly int _successStatusCode;

    /// <summary>
    ///     Initializes a new <see cref="MaybeHttpBuilder{TValue, TResponse}" /> from a synchronous
    ///     <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <param name="maybe">The Maybe value to convert.</param>
    public MaybeHttpBuilder(Maybe<TValue> maybe)
        : this(new ValueTask<Maybe<TValue>>(maybe))
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="MaybeHttpBuilder{TValue, TResponse}" /> from an asynchronous
    ///     <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <param name="awaitableResult">The asynchronous Maybe value to convert.</param>
    public MaybeHttpBuilder(ValueTask<Maybe<TValue>> awaitableResult)
        : this(awaitableResult, null, null, 200, null, null)
    {
    }

    private MaybeHttpBuilder(ValueTask<Maybe<TValue>> awaitableMaybe,
                              Func<TValue, TResponse>? responseFormatter,
                              IReadOnlyDictionary<string, string>? headers,
                              int successStatusCode,
                              Func<TValue, string>? locationFactory,
                              Func<IHttpResult>? noneMapper)
    {
        _awaitableMaybe = awaitableMaybe;
        _responseFormatter = responseFormatter;
        _headers = headers;
        _successStatusCode = successStatusCode;
        _locationFactory = locationFactory;
        _noneMapper = noneMapper;
    }

    /// <summary>
    ///     Gets the awaiter for this builder, allowing it to be used directly in <c>await</c> expressions.
    /// </summary>
    public ValueTaskAwaiter<IHttpResult> GetAwaiter()
    {
        return Build()
           .GetAwaiter();
    }

    /// <summary>
    ///     Adds a response header to the HTTP response.
    /// </summary>
    /// <param name="key">The header name.</param>
    /// <param name="value">The header value.</param>
    /// <returns>A new builder with the header added.</returns>
    public MaybeHttpBuilder<TValue, TResponse> WithHeader(string key,
                                                          string value)
    {
        var newHeaders = new Dictionary<string, string>();
        if (_headers is not null)
        {
            foreach (var (k, v) in _headers)
            {
                newHeaders[k] = v;
            }
        }

        newHeaders[key] = value;
        return new MaybeHttpBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            newHeaders,
            _successStatusCode,
            _locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Overrides the success HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to use on Some.</param>
    /// <returns>A new builder with the updated status code.</returns>
    public MaybeHttpBuilder<TValue, TResponse> WithStatusCode(int statusCode)
    {
        return new MaybeHttpBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            _headers,
            statusCode,
            _locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Applies a transformation to the Some value before serialization.
    /// </summary>
    /// <param name="formatter">
    ///     A function that maps <typeparamref name="TValue" /> to <typeparamref name="TNewResponse" />.
    /// </param>
    /// <typeparam name="TNewResponse">The new response body type.</typeparam>
    /// <returns>A new builder configured with the given formatter.</returns>
    public MaybeHttpBuilder<TValue, TNewResponse> WithResponseFormatter<TNewResponse>(Func<TValue, TNewResponse> formatter)
    {
        return new MaybeHttpBuilder<TValue, TNewResponse>(
            _awaitableMaybe,
            formatter,
            _headers,
            _successStatusCode,
            _locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Configures the builder to return a 201 Created response with a <c>Location</c> header on Some.
    /// </summary>
    /// <param name="locationFactory">A function that produces the Location URI from the Some value.</param>
    /// <returns>A new builder configured for 201 Created.</returns>
    public MaybeHttpBuilder<TValue, TResponse> AsCreated(Func<TValue, string> locationFactory)
    {
        return new MaybeHttpBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            _headers,
            201,
            locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Configures the HTTP response returned when the Maybe is None.
    ///     Defaults to <c>404 Not Found</c> when not specified.
    /// </summary>
    /// <param name="noneMapper">A factory that produces an <see cref="IHttpResult" /> for the None case.</param>
    /// <returns>A new builder with the given None handler.</returns>
    public MaybeHttpBuilder<TValue, TResponse> AsNone(Func<IHttpResult> noneMapper)
    {
        return new MaybeHttpBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            _headers,
            _successStatusCode,
            _locationFactory,
            noneMapper
        );
    }

    /// <summary>
    ///     Builds the HTTP response based on the Maybe value.
    /// </summary>
    public async ValueTask<IHttpResult> Build()
    {
        var maybe = await _awaitableMaybe;
        return maybe.Match(OnSome, OnNone);
    }

    private IHttpResult OnSome(TValue value)
    {
        var responseValue = _responseFormatter is not null
                                ? _responseFormatter(value)
                                : (TResponse)(object)value;

        IHttpResult httpResult;
        if (_successStatusCode == 201 &&
            _locationFactory is not null)
        {
            var location = _locationFactory(value);
            httpResult = HttpResults.Created(location, responseValue);
        }
        else
        {
            httpResult = _successStatusCode switch
            {
                200 => HttpResults.Ok(responseValue),
                201 => HttpResults.Created(string.Empty, responseValue),
                202 => HttpResults.Accepted(null, responseValue),
                204 => HttpResults.NoContent(),
                _ => HttpResults.StatusCode(_successStatusCode)
            };
        }

        return new WrapperHttpResult(httpResult, _headers);
    }

    private IHttpResult OnNone()
    {
        return _noneMapper is not null
                   ? _noneMapper()
                   : HttpResults.NotFound();
    }
}

/// <summary>
///     Fluent HTTP response builder for <see cref="Maybe{TValue}" />.
/// </summary>
/// <typeparam name="TValue">The type of the Maybe value.</typeparam>
public sealed class MaybeHttpBuilder<TValue> : MaybeHttpBuilder<TValue, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Initializes a new <see cref="MaybeHttpBuilder{TValue}" /> from a synchronous <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <param name="maybe">The Maybe value to convert.</param>
    public MaybeHttpBuilder(Maybe<TValue> maybe)
        : base(maybe)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="MaybeHttpBuilder{TValue}" /> from an asynchronous <see cref="Maybe{TValue}" />.
    /// </summary>
    /// <param name="awaitableResult">The asynchronous Maybe value to convert.</param>
    public MaybeHttpBuilder(ValueTask<Maybe<TValue>> awaitableResult)
        : base(awaitableResult)
    {
    }
}
