using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

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

    [Fact(DisplayName = "Policy defaults to Result=NoContent and MaybeNone=NotFound")]
    public void Policy_DefaultValue_IsExpected()
    {
        // Arrange (Given) & Act (When)
        var options = new ResultHttpOptions();

        // Assert (Then)
        Assert.Equal(ResultSuccessHttpBehavior.NoContent, options.Policy.ResultSuccessBehavior);
        Assert.Equal(MaybeNoneHttpBehavior.NotFound,      options.Policy.MaybeNoneBehavior);
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
        var options      = new ResultHttpOptions();
        var customMapper = Substitute.For<IFailureHttpMapper>();

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
        var options      = new ResultHttpOptions();
        var customMapper = Substitute.For<IFailureHttpMapper>();

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
        var mapper1 = Substitute.For<IFailureHttpMapper>();
        var mapper2 = Substitute.For<IFailureHttpMapper>();

        // Act (When)
        options.AddMapper(mapper1)
               .AddMapper(mapper2);

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

    [Fact(DisplayName = "BuildMapper returns DefaultFailureHttpMapper when no options configured")]
    public void BuildMapper_NoOptionsConfigured_ReturnsDefaultMapper()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<DefaultFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper returns CompositeMapper when custom mapper is added with default")]
    public void BuildMapper_SingleCustomMapper_ReturnsCompositeMapper()
    {
        // Arrange (Given)
        var options      = new ResultHttpOptions();
        var customMapper = Substitute.For<IFailureHttpMapper>();
        options.AddMapper(customMapper);

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<CompositeFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper returns CompositeFailureHttpMapper when custom mappers added")]
    public void BuildMapper_WithCustomMappers_ReturnsCompositeMapper()
    {
        // Arrange (Given)
        var options       = new ResultHttpOptions();
        var customMapper1 = Substitute.For<IFailureHttpMapper>();
        var customMapper2 = Substitute.For<IFailureHttpMapper>();
        options.AddMapper(customMapper1)
               .AddMapper(customMapper2);

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<CompositeFailureHttpMapper>(mapper);
    }

    [Fact(DisplayName = "BuildMapper prioritizes custom mappers over default mapper")]
    public void BuildMapper_WithCustomMappers_CustomMappersHavePriority()
    {
        // Arrange (Given)
        var options      = new ResultHttpOptions();
        var customMapper = Substitute.For<IFailureHttpMapper>();
        var error        = new ValidationFailure(["Test"]);

        customMapper.GetFailureResponse(error)
                    .Returns(new FailureHttpResponse { StatusCode = 999 });
        options.AddMapper(customMapper);

        // Act (When)
        var compositeMapper = options.BuildMapper();
        var response        = compositeMapper.GetFailureResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(999, response.StatusCode);
    }

    [Fact(DisplayName = "BuildMapper with UseProblemDetails and custom mapper returns CompositeMapper")]
    public void BuildMapper_UseProblemDetailsAndCustomMapper_ReturnsCompositeMapper()
    {
        // Arrange (Given)
        var options      = new ResultHttpOptions();
        var customMapper = Substitute.For<IFailureHttpMapper>();
        options.AddMapper(customMapper);

        // Act (When)
        var mapper = options.BuildMapper();

        // Assert (Then)
        Assert.IsType<CompositeFailureHttpMapper>(mapper);
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
        var mapper   = options.BuildMapper();
        var error    = new ExceptionalFailure(new Exception("Test"));
        var response = mapper.GetFailureResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
    }

    [Fact(DisplayName = "CustomMappers returns read-only collection")]
    public void CustomMappers_ReturnsReadOnlyCollection()
    {
        // Arrange (Given)
        var options      = new ResultHttpOptions();
        var customMapper = Substitute.For<IFailureHttpMapper>();
        options.AddMapper(customMapper);

        // Act (When)
        var mappers = options.CustomMappers;

        // Assert (Then)
        Assert.IsAssignableFrom<IReadOnlyList<IFailureHttpMapper>>(mappers);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(statusCode) adds typed mapper to collection")]
    public void AddMapper_TypedWithStatusCode_AddsMapperToCustomMappers()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When)
        options.AddMapper<ValidationFailure>(400);

        // Assert (Then)
        Assert.Single(options.CustomMappers);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(statusCode) returns options for fluent chaining")]
    public void AddMapper_TypedWithStatusCode_ReturnsOptionsForFluentChaining()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When)
        var result = options.AddMapper<ValidationFailure>(400);

        // Assert (Then)
        Assert.Same(options, result);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(statusCode) produces correct status code for matching failure")]
    public void AddMapper_TypedWithStatusCode_ProducesCorrectStatusCodeForMatchingFailure()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        options.AddMapper<ValidationFailure>(422);
        var failure = new ValidationFailure(["Bad input"]);

        // Act (When)
        var mapper   = options.BuildMapper();
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(422, response.StatusCode);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(statusCode) falls through to default for non-matching failure")]
    public void AddMapper_TypedWithStatusCode_FallsThroughForNonMatchingFailure()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        options.AddMapper<ConflictFailure>(409);
        var failure = new NotFoundFailure("User", "1");

        // Act (When)
        var mapper   = options.BuildMapper();
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        // Default mapper produces 404 for NotFoundFailure
        Assert.NotNull(response);
        Assert.Equal(404, response.StatusCode);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(factory) adds typed mapper to collection")]
    public void AddMapper_TypedWithFactory_AddsMapperToCustomMappers()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When)
        options.AddMapper<ValidationFailure>(f => new FailureHttpResponse { StatusCode = 400, Body = new { f.Message } });

        // Assert (Then)
        Assert.Single(options.CustomMappers);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(factory) invokes factory with typed failure")]
    public void AddMapper_TypedWithFactory_InvokesFactoryWithTypedFailure()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        var failure = new ConflictFailure("Already taken");
        options.AddMapper<ConflictFailure>(f => new FailureHttpResponse
        {
            StatusCode = 409,
            Body       = new { message = f.Message }
        });

        // Act (When)
        var mapper   = options.BuildMapper();
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(409, response.StatusCode);
    }

    [Fact(DisplayName = "AddMapper<TFailure>(factory) throws when factory is null")]
    public void AddMapper_TypedWithFactory_ThrowsWhenFactoryIsNull()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => options.AddMapper((Func<ValidationFailure, FailureHttpResponse>)null!));
    }

    [Fact(DisplayName = "Multiple typed mappers are evaluated in add order")]
    public void AddMapper_MultipleTypedMappers_EvaluatedInAddOrder()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        options.AddMapper<ValidationFailure>(400);
        options.AddMapper<ValidationFailure>(422); // second should never be reached for ValidationFailure

        var failure = new ValidationFailure(["Bad"]);

        // Act (When)
        var mapper   = options.BuildMapper();
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode); // first match wins
    }
}
