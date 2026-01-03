using UnambitiousFx.Functional.AspNetCore.Mappers;

namespace UnambitiousFx.Functional.AspNetCore;

/// <summary>
///     Configuration options for Result-to-HTTP conversion.
/// </summary>
public sealed class ResultHttpOptions
{
    private readonly List<IErrorHttpMapper> _customMappers = [];

    /// <summary>
    ///     Whether to use Problem Details (RFC 7807) format for error responses.
    ///     Default is false (uses simple JSON error responses).
    /// </summary>
    public bool UseProblemDetails { get; set; }

    /// <summary>
    ///     Whether to include exception details (stack traces) in error responses.
    ///     Should only be enabled in development environments.
    ///     Default is false.
    /// </summary>
    public bool IncludeExceptionDetails { get; set; }

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
    ///     Builds the final error mapper based on the configured options.
    /// </summary>
    /// <returns>The configured error mapper.</returns>
    internal IErrorHttpMapper BuildMapper()
    {
        var mappers = new List<IErrorHttpMapper>();

        // Add custom mappers first (they have priority)
        mappers.AddRange(_customMappers);

        // Add the appropriate default mapper
        if (UseProblemDetails)
        {
            mappers.Add(new ProblemDetailsErrorMapper(IncludeExceptionDetails));
        }
        else
        {
            mappers.Add(new DefaultErrorHttpMapper());
        }

        // If we only have one mapper, return it directly
        if (mappers.Count == 1)
        {
            return mappers[0];
        }

        // Otherwise, wrap in a composite mapper
        return new CompositeErrorHttpMapper(mappers);
    }
}
