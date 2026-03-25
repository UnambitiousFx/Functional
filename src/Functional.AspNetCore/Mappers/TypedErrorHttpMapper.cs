using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Mappers;

/// <summary>
///     An error mapper that handles a specific failure type using a delegate factory.
///     Returns null for failures that do not match <typeparamref name="TFailure" />.
/// </summary>
/// <typeparam name="TFailure">The failure type handled by this mapper.</typeparam>
internal sealed class TypedErrorHttpMapper<TFailure> : IErrorHttpMapper
    where TFailure : IFailure
{
    private readonly Func<TFailure, ErrorHttpResponse?> _factory;

    /// <summary>
    ///     Creates a typed mapper backed by a factory delegate.
    /// </summary>
    /// <param name="factory">The factory invoked when a matching failure is encountered.</param>
    internal TypedErrorHttpMapper(Func<TFailure, ErrorHttpResponse?> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _factory = factory;
    }

    /// <inheritdoc />
    public ErrorHttpResponse? GetErrorResponse(IFailure failure)
    {
        if (failure is TFailure typed)
        {
            return _factory(typed);
        }

        return null;
    }
}
