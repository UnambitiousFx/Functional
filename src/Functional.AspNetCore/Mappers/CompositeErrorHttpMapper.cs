using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     Composite mapper that chains multiple error mappers using chain-of-responsibility pattern.
///     Tries each mapper in order until one returns a non-null result.
/// </summary>
public sealed class CompositeErrorHttpMapper : IErrorHttpMapper
{
    private readonly IReadOnlyList<IErrorHttpMapper> _mappers;

    /// <summary>
    ///     Creates a composite mapper with the specified mappers.
    /// </summary>
    /// <param name="mappers">Ordered list of mappers to try.</param>
    public CompositeErrorHttpMapper(IReadOnlyList<IErrorHttpMapper> mappers)
    {
        ArgumentNullException.ThrowIfNull(mappers);
        _mappers = mappers;
    }

    /// <summary>
    ///     Creates a composite mapper with the specified mappers.
    /// </summary>
    /// <param name="mappers">Mappers to try in order.</param>
    public CompositeErrorHttpMapper(params IErrorHttpMapper[] mappers)
    {
        ArgumentNullException.ThrowIfNull(mappers);
        _mappers = mappers;
    }


    /// <inheritdoc />
    public (int StatusCode, object? Body)? GetResponse(IError error)
    {
        foreach (var mapper in _mappers)
        {
            var mappedResponse = mapper.GetResponse(error);
            if (mappedResponse is not null) return mappedResponse;
        }

        return null;
    }
}