using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact(DisplayName = "AddResultHttp registers IErrorHttpMapper as singleton")]
    public void AddResultHttp_RegistersErrorHttpMapperAsSingleton()
    {
        // Arrange (Given)
        var services = new ServiceCollection();

        // Act (When)
        services.AddResultHttp();
        var provider = services.BuildServiceProvider();

        // Assert (Then)
        var mapper = provider.GetService<IErrorHttpMapper>();
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
        var mapper = provider.GetRequiredService<IErrorHttpMapper>();

        // Assert (Then)
        Assert.IsType<DefaultErrorHttpMapper>(mapper);
    }


    [Fact(DisplayName = "AddResultHttp with custom mapper registers composite mapper")]
    public void AddResultHttp_WithCustomMapper_RegistersCompositeMapper()
    {
        // Arrange (Given)
        var services = new ServiceCollection();
        var customMapper = Substitute.For<IErrorHttpMapper>();

        // Act (When)
        services.AddResultHttp(options => { options.AddMapper(customMapper); });
        var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IErrorHttpMapper>();

        // Assert (Then)
        Assert.IsType<CompositeErrorHttpMapper>(mapper);
    }

    [Fact(DisplayName = "AddResultHttp with multiple custom mappers registers composite mapper")]
    public void AddResultHttp_WithMultipleCustomMappers_RegistersCompositeMapper()
    {
        // Arrange (Given)
        var services = new ServiceCollection();
        var customMapper1 = Substitute.For<IErrorHttpMapper>();
        var customMapper2 = Substitute.For<IErrorHttpMapper>();

        // Act (When)
        services.AddResultHttp(options =>
        {
            options.AddMapper(customMapper1);
            options.AddMapper(customMapper2);
        });
        var provider = services.BuildServiceProvider();
        var mapper = provider.GetRequiredService<IErrorHttpMapper>();

        // Assert (Then)
        Assert.IsType<CompositeErrorHttpMapper>(mapper);
    }

    [Fact(DisplayName = "AddResultHttp registers mapper as singleton across multiple resolves")]
    public void AddResultHttp_RegistersMapperAsSingleton_SameInstanceAcrossResolves()
    {
        // Arrange (Given)
        var services = new ServiceCollection();
        services.AddResultHttp();
        var provider = services.BuildServiceProvider();

        // Act (When)
        var mapper1 = provider.GetRequiredService<IErrorHttpMapper>();
        var mapper2 = provider.GetRequiredService<IErrorHttpMapper>();

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
        var mapper = provider.GetRequiredService<IErrorHttpMapper>();

        // Act (When)
        var error = new ExceptionalError(new Exception("Test exception"));
        var body = mapper.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(body);
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
        var mappers = provider.GetServices<IErrorHttpMapper>().ToList();
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
        var mapper = provider.GetRequiredService<IErrorHttpMapper>();

        // Assert (Then)
        Assert.IsType<DefaultErrorHttpMapper>(mapper);
    }
}