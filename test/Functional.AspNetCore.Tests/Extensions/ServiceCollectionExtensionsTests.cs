using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact(DisplayName = "AddResultHttp registers IFailureHttpMapper as singleton")]
    public void AddResultHttp_RegistersErrorHttpMapperAsSingleton()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        services.AddResultHttp();
        var provider = services.BuildServiceProvider();

        // Assert (Then)
        var mapper = provider.GetService<IFailureHttpMapper>();
        Assert.NotNull(mapper);
    }

    [Fact(DisplayName = "AddResultHttp returns same service collection for fluent chaining")]
    public void AddResultHttp_ReturnsServiceCollectionForFluentChaining()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        var result = services.AddResultHttp();

        // Assert (Then)
        Assert.Same(services, result);
    }

    [Fact(DisplayName = "AddResultHttp throws ArgumentNullException when services is null")]
    public void AddResultHttp_ServicesIsNull_ThrowsArgumentNullException()
    {
        // Arrange (Given)
        IServiceCollection services = null!;

        // Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => services.AddResultHttp());
    }

    [Fact(DisplayName = "AddResultHttp without configuration uses default mapper")]
    public void AddResultHttp_WithoutConfiguration_UsesDefaultMapper()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        services.AddResultHttp();
        var provider = services.BuildServiceProvider();
        var mapper   = provider.GetRequiredService<IFailureHttpMapper>();

        // Assert (Then)
        Assert.IsType<DefaultFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "AddResultHttp with custom mapper registers composite mapper")]
    public void AddResultHttp_WithCustomMapper_RegistersCompositeMapper()
    {
        // Arrange (Given)
        var services     = new ServiceCollection();
        var customMapper = Substitute.For<IFailureHttpMapper>();

        // Act (When)
        services.AddResultHttp(options => { options.AddMapper(customMapper); });
        var provider = services.BuildServiceProvider();
        var mapper   = provider.GetRequiredService<IFailureHttpMapper>();

        // Assert (Then)
        Assert.IsType<CompositeFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "AddResultHttp with multiple custom mappers registers composite mapper")]
    public void AddResultHttp_WithMultipleCustomMappers_RegistersCompositeMapper()
    {
        // Arrange (Given)
        var services      = new ServiceCollection();
        var customMapper1 = Substitute.For<IFailureHttpMapper>();
        var customMapper2 = Substitute.For<IFailureHttpMapper>();

        // Act (When)
        services.AddResultHttp(options =>
        {
            options.AddMapper(customMapper1);
            options.AddMapper(customMapper2);
        });
        var provider = services.BuildServiceProvider();
        var mapper   = provider.GetRequiredService<IFailureHttpMapper>();

        // Assert (Then)
        Assert.IsType<CompositeFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "AddResultHttp registers mapper as singleton across multiple resolves")]
    public void AddResultHttp_RegistersMapperAsSingleton_SameInstanceAcrossResolves()
    {
        // Arrange (Given)
        var services = new ServiceCollection();
        services.AddResultHttp();
        var provider = services.BuildServiceProvider();

        // Act (When)
        var mapper1 = provider.GetRequiredService<IFailureHttpMapper>();
        var mapper2 = provider.GetRequiredService<IFailureHttpMapper>();

        // Assert (Then)
        Assert.Same(mapper1, mapper2);
    }

    [Fact(DisplayName = "AddResultHttp with IncludeExceptionDetails configures mapper correctly")]
    public void AddResultHttp_WithIncludeExceptionDetails_ConfiguresMapperCorrectly()
    {
        // Arrange (Given)
        var services = new ServiceCollection();
        services.AddResultHttp(options => { options.IncludeExceptionDetails = true; });
        var provider = services.BuildServiceProvider();
        var mapper   = provider.GetRequiredService<IFailureHttpMapper>();

        // Act (When)
        var error    = new ExceptionalFailure(new Exception("Test exception"));
        var response = mapper.GetFailureResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
    }

    [Fact(DisplayName = "AddResultHttp can be called multiple times without error")]
    public void AddResultHttp_CanBeCalledMultipleTimes_WithoutError()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        services.AddResultHttp();
        services.AddResultHttp();

        // Assert (Then)
        var provider = services.BuildServiceProvider();
        var mappers = provider.GetServices<IFailureHttpMapper>()
                              .ToList();
        Assert.Equal(2, mappers.Count);
    }

    [Fact(DisplayName = "AddResultHttp with null configure action uses default options")]
    public void AddResultHttp_WithNullConfigureAction_UsesDefaultOptions()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        services.AddResultHttp();
        var provider = services.BuildServiceProvider();
        var mapper   = provider.GetRequiredService<IFailureHttpMapper>();

        // Assert (Then)
        Assert.IsType<DefaultFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "AddResultHttp registers configurable ResultHttpAdapterPolicy as singleton")]
    public void AddResultHttp_RegistersPolicyAsSingleton()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        services.AddResultHttp(options =>
        {
            options.Policy = options.Policy with
            {
                ResultSuccessBehavior = ResultSuccessHttpBehavior.Ok,
                MaybeNoneBehavior = MaybeNoneHttpBehavior.NoContent
            };
        });

        var provider = services.BuildServiceProvider();
        var policy1  = provider.GetRequiredService<ResultHttpAdapterPolicy>();
        var policy2  = provider.GetRequiredService<ResultHttpAdapterPolicy>();

        // Assert (Then)
        Assert.Same(policy1, policy2);
        Assert.Equal(ResultSuccessHttpBehavior.Ok,    policy1.ResultSuccessBehavior);
        Assert.Equal(MaybeNoneHttpBehavior.NoContent, policy1.MaybeNoneBehavior);
    }
}
