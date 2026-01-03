using Microsoft.Extensions.DependencyInjection;

namespace UnambitiousFx.Functional.AspNetCore;

/// <summary>
///     Dependency injection extensions for Result HTTP handling.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds Result-to-HTTP conversion services to the service collection.
    ///     Registers the configured error mapper as a singleton.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The service collection for fluent chaining.</returns>
    public static IServiceCollection AddResultHttp(
        this IServiceCollection services,
        Action<ResultHttpOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new ResultHttpOptions();
        configure?.Invoke(options);

        var mapper = options.BuildMapper();
        services.AddSingleton(mapper);

        return services;
    }
}
