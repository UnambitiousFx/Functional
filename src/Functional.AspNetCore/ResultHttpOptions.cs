using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore;

/// <summary>
///     Configuration options for Result-to-HTTP conversion.
/// </summary>
public sealed class ResultHttpOptions
{
    private readonly List<IFailureHttpMapper> _customMappers = [];

    /// <summary>
    ///     Whether the default mapper includes failure details on 500 responses: the message, and for an
    ///     <see cref="ExceptionalFailure" /> the exception type and stack trace.
    ///     When false, 500 responses carry only title, status and type.
    ///     Should only be enabled in development environments.
    ///     Default is false.
    /// </summary>
    public bool IncludeExceptionDetails { get; set; }

    /// <summary>
    ///     Adapter policy defaults used by HTTP conversion extensions.
    /// </summary>
    public ResultHttpAdapterPolicy Policy { get; set; } = ResultHttpAdapterPolicy.Default;

    internal Dictionary<string, int> CodeStatuses { get; } = new(StringComparer.Ordinal);

    internal List<KeyValuePair<string, int>> PrefixStatuses { get; } = [];

    internal List<string> ExposedMetadata { get; } = [];

    internal int DefaultStatusCode { get; private set; } = StatusCodes.Status500InternalServerError;

    /// <summary>
    ///     Gets the collection of custom error mappers.
    ///     Custom mappers are evaluated before the default mapper.
    /// </summary>
    public IReadOnlyList<IFailureHttpMapper> CustomMappers => _customMappers.AsReadOnly();

    /// <summary>
    ///     Adds a custom error mapper.
    ///     Custom mappers are tried in the order they are added, before the default mapper.
    /// </summary>
    /// <param name="mapper">The custom mapper to add.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions AddMapper(IFailureHttpMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        _customMappers.Add(mapper);
        return this;
    }

    /// <summary>
    ///     Adds a typed mapper for <typeparamref name="TFailure" /> that returns a response with the given status code
    ///     and a default <see cref="ProblemDetails" /> body containing the failure message.
    ///     Custom mappers are tried before the default mapper, in the order they are added.
    /// </summary>
    /// <typeparam name="TFailure">The failure type to handle.</typeparam>
    /// <param name="statusCode">The HTTP status code to return for this failure type.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions AddMapper<TFailure>(int statusCode)
        where TFailure : IFailure
    {
        return AddMapper(new TypedFailureHttpMapper<TFailure>(f => new FailureHttpResponse
        {
            StatusCode = statusCode,
            Body = new ProblemDetails
            {
                Title  = "An error occurred.",
                Detail = f.Message,
                Status = statusCode
            }
        }));
    }

    /// <summary>
    ///     Adds a typed mapper for <typeparamref name="TFailure" /> using a custom factory delegate.
    ///     Custom mappers are tried before the default mapper, in the order they are added.
    /// </summary>
    /// <typeparam name="TFailure">The failure type to handle.</typeparam>
    /// <param name="factory">The factory that produces an <see cref="FailureHttpResponse" /> for the failure.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions AddMapper<TFailure>(Func<TFailure, FailureHttpResponse> factory)
        where TFailure : IFailure
    {
        ArgumentNullException.ThrowIfNull(factory);
        return AddMapper(new TypedFailureHttpMapper<TFailure>(factory));
    }

    /// <summary>
    ///     Maps failures whose <see cref="IFailure.Code" /> equals <paramref name="code" /> to <paramref name="statusCode" />.
    ///     Code mappings are checked after the custom mappers and before the built-in mapping by failure type.
    ///     The response body stays the one built for the failure type, with the mapped status.
    /// </summary>
    /// <param name="code">The failure code, compared ordinally.</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions MapCode(string code,
                                     int    statusCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(code);
        CodeStatuses[code] = ValidStatus(statusCode);
        return this;
    }

    /// <summary>
    ///     Maps failures whose <see cref="IFailure.Code" /> starts with <paramref name="prefix" /> to
    ///     <paramref name="statusCode" />. Exact <see cref="MapCode" /> mappings win; prefixes are tried in the order
    ///     they are added.
    /// </summary>
    /// <param name="prefix">The code prefix, compared ordinally (e.g. <c>documents.</c>).</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions MapCodePrefix(string prefix,
                                           int    statusCode)
    {
        ArgumentException.ThrowIfNullOrEmpty(prefix);
        PrefixStatuses.Add(new KeyValuePair<string, int>(prefix, ValidStatus(statusCode)));
        return this;
    }

    /// <summary>
    ///     Sets the status code of failures with no built-in mapping and no code mapping, such as a plain
    ///     <see cref="Failure" />. Default is 500. Below 500, the failure message is returned as <c>detail</c>.
    /// </summary>
    /// <param name="statusCode">The HTTP status code to return (e.g. 422 for business refusals).</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions DefaultStatus(int statusCode)
    {
        DefaultStatusCode = ValidStatus(statusCode);
        return this;
    }

    /// <summary>
    ///     Copies these keys of <see cref="IFailure.Metadata" />, when present, into the <see cref="ProblemDetails" />
    ///     extensions written by the default mapper. Metadata is never exposed unless listed here.
    ///     A key never replaces an extension the mapper already wrote (such as <c>code</c> or <c>errors</c>).
    /// </summary>
    /// <param name="keys">The metadata keys to expose.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions ExposeMetadata(params string[] keys)
    {
        ArgumentNullException.ThrowIfNull(keys);
        foreach (var key in keys) {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ExposedMetadata.Add(key);
        }

        return this;
    }

    private static int ValidStatus(int statusCode)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(statusCode, 100);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(statusCode, 599);
        return statusCode;
    }

    /// <summary>
    ///     Builds the final error mapper based on the configured options.
    /// </summary>
    /// <returns>The configured error mapper.</returns>
    internal IFailureHttpMapper BuildMapper()
    {
        var mappers = new List<IFailureHttpMapper>();

        // Add custom mappers first (they have priority)
        mappers.AddRange(_customMappers);

        mappers.Add(new DefaultFailureHttpMapper(this));

        // If we only have one mapper, return it directly
        if (mappers.Count == 1) {
            return mappers[0];
        }

        // Otherwise, wrap in a composite mapper
        return new CompositeFailureHttpMapper(mappers);
    }
}
