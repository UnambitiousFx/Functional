using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Options;

public class ResultHttpOptionsTests
{
    [Fact(DisplayName = "IncludeExceptionDetails is false by default")]
    public void IncludeExceptionDetails_DefaultValue_IsFalse()
    {
        // Arrange (Given) & Act (When)
        var options = new ResultHttpOptions();

        // Assert (Then)
        Assert.False(options.IncludeExceptionDetails);
    }

    [Fact(DisplayName = "CustomMappers is empty by default")]
    public void CustomMappers_DefaultValue_IsEmpty()
    {
        // Arrange (Given) & Act (When)
        var options = new ResultHttpOptions();

        // Assert (Then)
        Assert.Empty(options.CustomMappers);
    }

    [Fact(DisplayName = "AddMapper adds custom mapper to collection")]
    public void AddMapper_AddsMapperToCustomMappers()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper = Substitute.For<IErrorHttpMapper>();

        // Act (When)
        options.AddMapper(customMapper);

        // Assert (Then)
        Assert.Single(options.CustomMappers);
        Assert.Contains(customMapper, options.CustomMappers);
    }

    [Fact(DisplayName = "AddMapper returns options for fluent chaining")]
    public void AddMapper_ReturnsOptionsForFluentChaining()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper = Substitute.For<IErrorHttpMapper>();

        // Act (When)
        var result = options.AddMapper(customMapper);

        // Assert (Then)
        Assert.Same(options, result);
    }

    [Fact(DisplayName = "AddMapper allows multiple mappers to be added")]
    public void AddMapper_AllowsMultipleMappers()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var mapper1 = Substitute.For<IErrorHttpMapper>();
        var mapper2 = Substitute.For<IErrorHttpMapper>();

        // Act (When)
        options.AddMapper(mapper1).AddMapper(mapper2);

        // Assert (Then)
        Assert.Equal(2, options.CustomMappers.Count);
        Assert.Contains(mapper1, options.CustomMappers);
        Assert.Contains(mapper2, options.CustomMappers);
    }

    [Fact(DisplayName = "AddMapper throws ArgumentNullException when mapper is null")]
    public void AddMapper_MapperIsNull_ThrowsArgumentNullException()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => options.AddMapper(null!));
    }

    [Fact(DisplayName = "BuildMapper returns DefaultErrorHttpMapper when no options configured")]
    public void BuildMapper_NoOptionsConfigured_ReturnsDefaultMapper()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<DefaultErrorHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper returns CompositeMapper when custom mapper is added with default")]
    public void BuildMapper_SingleCustomMapper_ReturnsCompositeMapper()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper = Substitute.For<IErrorHttpMapper>();
        options.AddMapper(customMapper);

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<CompositeErrorHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper returns CompositeErrorHttpMapper when custom mappers added")]
    public void BuildMapper_WithCustomMappers_ReturnsCompositeMapper()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper1 = Substitute.For<IErrorHttpMapper>();
        var customMapper2 = Substitute.For<IErrorHttpMapper>();
        options.AddMapper(customMapper1).AddMapper(customMapper2);

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<CompositeErrorHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper prioritizes custom mappers over default mapper")]
    public void BuildMapper_WithCustomMappers_CustomMappersHavePriority()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper = Substitute.For<IErrorHttpMapper>();
        var error = new ValidationError(["Test"]);

        customMapper.GetResponse(error).Returns((999, null));
        options.AddMapper(customMapper);

        // Act (When)
        var compositeMapper = options.BuildMapper();
        var response = compositeMapper.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(999, response.Value.StatusCode);
    }

    [Fact(DisplayName = "BuildMapper with UseProblemDetails and custom mapper returns CompositeMapper")]
    public void BuildMapper_UseProblemDetailsAndCustomMapper_ReturnsCompositeMapper()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper = Substitute.For<IErrorHttpMapper>();
        options.AddMapper(customMapper);

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<CompositeErrorHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper passes IncludeExceptionDetails to ProblemDetailsErrorMapper")]
    public void BuildMapper_PassesIncludeExceptionDetailsToProblemDetailsMapper()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions
        {
            IncludeExceptionDetails = true
        };

        // Act (When)
        var mapper = options.BuildMapper();
        var error = new ExceptionalError(new Exception("Test"));
        var body = mapper.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(body);
    }

    [Fact(DisplayName = "CustomMappers returns read-only collection")]
    public void CustomMappers_ReturnsReadOnlyCollection()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var customMapper = Substitute.For<IErrorHttpMapper>();
        options.AddMapper(customMapper);

        // Act (When)
        var mappers = options.CustomMappers;

        // Assert (Then)
        Assert.IsAssignableFrom<IReadOnlyList<IErrorHttpMapper>>(mappers);
    }
}