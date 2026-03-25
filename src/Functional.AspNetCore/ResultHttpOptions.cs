using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore;

/// <summary>
///     Configuration options for Result-to-HTTP conversion.
/// </summary>
public sealed class ResultHttpOptions
{
    private readonly List<IErrorHttpMapper> _customMappers = [];

    /// <summary>
    ///     Whether to include exception details (stack traces) in error responses.
    ///     Should only be enabled in development environments.
    ///     Default is false.
    /// </summary>
    public bool IncludeExceptionDetails { get; set; }

    /// <summary>
    ///     Adapter policy defaults used by HTTP conversion extensions.
    /// </summary>
    public ResultHttpAdapterPolicy Policy { get; set; } = ResultHttpAdapterPolicy.Default;

    /// <summary>
    ///     Gets the collection of custom error mappers.
    ///     Custom mappers are evaluated before the default mapper.
    /// </summary>
    public IReadOnlyList<IErrorHttpMapper> CustomMappers => _customMappers.AsReadOnly();

    /// <summary>
    ///     Adds a custom error mapper.
    ///     Custom mappers are tried in the order they are added, before the default mapper.
    /// </summary>
    /// <param name="mapper">The custom mapper to add.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions AddMapper(IErrorHttpMapper mapper)
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
        return AddMapper(new TypedErrorHttpMapper<TFailure>(f => new ErrorHttpResponse
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
    /// <param name="factory">The factory that produces an <see cref="ErrorHttpResponse" /> for the failure.</param>
    /// <returns>This options instance for fluent configuration.</returns>
    public ResultHttpOptions AddMapper<TFailure>(Func<TFailure, ErrorHttpResponse> factory)
        where TFailure : IFailure
    {
        ArgumentNullException.ThrowIfNull(factory);
        return AddMapper(new TypedErrorHttpMapper<TFailure>(factory));
    }

    /// <summary>
    ///     Builds the final error mapper based on the configured options.
    /// </summary>
    /// <returns>The configured error mapper.</returns>
    internal IErrorHttpMapper BuildMapper()
    {
        var mappers = new List<IErrorHttpMapper>();

        // Add custom mappers first (they have priority)
        mappers.AddRange(_customMappers);

        mappers.Add(new DefaultErrorHttpMapper());

        // If we only have one mapper, return it directly
        if (mappers.Count == 1) {
            return mappers[0];
        }

        // Otherwise, wrap in a composite mapper
        return new CompositeErrorHttpMapper(mappers);
    }
}
