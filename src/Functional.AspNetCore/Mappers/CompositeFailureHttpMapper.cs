using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Composite mapper that chains multiple error mappers using chain-of-responsibility pattern.
///     Tries each mapper in order until one returns a non-null result.
/// </summary>
public sealed class CompositeFailureHttpMapper : IFailureHttpMapper
{
    private readonly IReadOnlyList<IFailureHttpMapper> _mappers;

    /// <summary>
    ///     Creates a composite mapper with the specified mappers.
    /// </summary>
    /// <param name="mappers">Ordered list of mappers to try.</param>
    public CompositeFailureHttpMapper(IReadOnlyList<IFailureHttpMapper> mappers)
    {
        ArgumentNullException.ThrowIfNull(mappers);
        _mappers = mappers;
    }

    /// <summary>
    ///     Creates a composite mapper with the specified mappers.
    /// </summary>
    /// <param name="mappers">Mappers to try in order.</param>
    public CompositeFailureHttpMapper(params IFailureHttpMapper[] mappers)
    {
        ArgumentNullException.ThrowIfNull(mappers);
        _mappers = mappers;
    }

    /// <inheritdoc />
    public FailureHttpResponse? GetFailureResponse(IFailure failure)
    {
        foreach (var mapper in _mappers) {
            var mappedResponse = mapper.GetFailureResponse(failure);
            if (mappedResponse is not null) {
                return mappedResponse;
            }
        }

        return null;
    }
}
