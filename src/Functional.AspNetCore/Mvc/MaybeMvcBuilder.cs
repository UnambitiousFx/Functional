using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Mvc;

/// <summary>
///     Fluent MVC action result builder for <see cref="Maybe{TValue}" /> with a response formatter.
/// </summary>
/// <typeparam name="TValue">The type of the Maybe value.</typeparam>
/// <typeparam name="TResponse">The type of the response body.</typeparam>
public class MaybeMvcBuilder<TValue, TResponse>
    where TValue : notnull
{
    private readonly ValueTask<Maybe<TValue>> _awaitableMaybe;
    private readonly IReadOnlyDictionary<string, string>? _headers;
    private readonly Func<TValue, string>? _locationFactory;
    private readonly Func<IActionResult>? _noneMapper;
    private readonly Func<TValue, TResponse>? _responseFormatter;
    private readonly int _successStatusCode;

    /// <summary>
    ///     Initializes a new <see cref="MaybeMvcBuilder{TValue, TResponse}" /> from a synchronous Maybe value.
    /// </summary>
    public MaybeMvcBuilder(Maybe<TValue> maybe)
        : this(new ValueTask<Maybe<TValue>>(maybe))
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="MaybeMvcBuilder{TValue, TResponse}" /> from an asynchronous Maybe value.
    /// </summary>
    public MaybeMvcBuilder(ValueTask<Maybe<TValue>> awaitableResult)
        : this(awaitableResult, null, null, 200, null, null)
    {
    }

    private MaybeMvcBuilder(ValueTask<Maybe<TValue>> awaitableMaybe,
                            Func<TValue, TResponse>? responseFormatter,
                            IReadOnlyDictionary<string, string>? headers,
                            int successStatusCode,
                            Func<TValue, string>? locationFactory,
                            Func<IActionResult>? noneMapper)
    {
        _awaitableMaybe = awaitableMaybe;
        _responseFormatter = responseFormatter;
        _headers = headers;
        _successStatusCode = successStatusCode;
        _locationFactory = locationFactory;
        _noneMapper = noneMapper;
    }

    /// <summary>
    ///     Gets the awaiter for this builder, allowing direct use in <c>await</c> expressions.
    /// </summary>
    public ValueTaskAwaiter<IActionResult> GetAwaiter()
    {
        return Build()
           .GetAwaiter();
    }

    /// <summary>
    ///     Adds or updates a response header.
    /// </summary>
    public MaybeMvcBuilder<TValue, TResponse> WithHeader(string key,
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
        return new MaybeMvcBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            newHeaders,
            _successStatusCode,
            _locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Overrides the success status code used for Some responses.
    /// </summary>
    public MaybeMvcBuilder<TValue, TResponse> WithStatusCode(int statusCode)
    {
        return new MaybeMvcBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            _headers,
            statusCode,
            _locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Applies a transformation to the Some value before creating the response body.
    /// </summary>
    public MaybeMvcBuilder<TValue, TNewResponse> WithResponseFormatter<TNewResponse>(Func<TValue, TNewResponse> formatter)
    {
        return new MaybeMvcBuilder<TValue, TNewResponse>(
            _awaitableMaybe,
            formatter,
            _headers,
            _successStatusCode,
            _locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Configures the builder to return 201 Created with a Location value for Some.
    /// </summary>
    public MaybeMvcBuilder<TValue, TResponse> AsCreated(Func<TValue, string> locationFactory)
    {
        return new MaybeMvcBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            _headers,
            StatusCodes.Status201Created,
            locationFactory,
            _noneMapper
        );
    }

    /// <summary>
    ///     Configures the response returned when the Maybe value is None.
    /// </summary>
    public MaybeMvcBuilder<TValue, TResponse> AsNone(Func<IActionResult> noneMapper)
    {
        return new MaybeMvcBuilder<TValue, TResponse>(
            _awaitableMaybe,
            _responseFormatter,
            _headers,
            _successStatusCode,
            _locationFactory,
            noneMapper
        );
    }

    /// <summary>
    ///     Builds the MVC action result from the Maybe value.
    /// </summary>
    public async ValueTask<IActionResult> Build()
    {
        var maybe = await _awaitableMaybe;
        return maybe.Match(OnSome, OnNone);
    }

    private IActionResult OnSome(TValue value)
    {
        var responseValue = _responseFormatter is not null
                                ? _responseFormatter(value)
                                : (TResponse)(object)value;

        IActionResult actionResult;
        if (_successStatusCode == StatusCodes.Status201Created &&
            _locationFactory is not null)
        {
            var location = _locationFactory(value);
            actionResult = new CreatedResult(location, responseValue);
        }
        else
        {
            actionResult = _successStatusCode switch
            {
                200 => new OkObjectResult(responseValue),
                201 => new CreatedResult(string.Empty, responseValue),
                202 => new AcceptedResult((string?)null, responseValue),
                204 => new NoContentResult(),
                _ => new StatusCodeResult(_successStatusCode)
            };
        }

        return new WrapperActionResult(actionResult, _headers);
    }

    private IActionResult OnNone()
    {
        return _noneMapper is not null
                   ? _noneMapper()
                   : new NotFoundResult();
    }
}

/// <summary>
///     Fluent MVC action result builder for <see cref="Maybe{TValue}" />.
/// </summary>
/// <typeparam name="TValue">The type of the Maybe value.</typeparam>
public sealed class MaybeMvcBuilder<TValue> : MaybeMvcBuilder<TValue, TValue>
    where TValue : notnull
{
    /// <summary>
    ///     Initializes a new <see cref="MaybeMvcBuilder{TValue}" /> from a synchronous Maybe value.
    /// </summary>
    public MaybeMvcBuilder(Maybe<TValue> maybe)
        : base(maybe)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="MaybeMvcBuilder{TValue}" /> from an asynchronous Maybe value.
    /// </summary>
    public MaybeMvcBuilder(ValueTask<Maybe<TValue>> awaitableResult)
        : base(awaitableResult)
    {
    }
}
