using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions;

public class RegisteredServicesTests
{
    [Fact]
    public async Task AsHttpBuilder_WithoutMapper_UsesMapperRegisteredByAddResultHttp()
    {
        // Arrange (Given)
        var context = CreateContext(o => o.AddMapper<BusinessFailure>(422));
        var result  = Result.Failure<int>(new BusinessFailure());

        // Act (When)
        await (await result.AsHttpBuilder()).ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal(422, context.Response.StatusCode);
    }

    [Fact]
    public async Task AsHttpBuilder_WithExplicitMapper_IgnoresRegisteredMapper()
    {
        // Arrange (Given)
        var context = CreateContext(o => o.AddMapper<BusinessFailure>(422));
        var result  = Result.Failure(new BusinessFailure());

        // Act (When)
        await (await result.AsHttpBuilder(new DefaultFailureHttpMapper())).ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task AsHttpBuilder_WithoutRegistration_UsesDefaultMapper()
    {
        // Arrange (Given)
        var context = CreateContext(null, addResultHttp: false);
        var result  = Result.Failure(new ConflictFailure("taken"));

        // Act (When)
        await (await result.AsHttpBuilder()).ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
    }

    [Theory]
    [InlineData(ResultSuccessHttpBehavior.NoContent, StatusCodes.Status204NoContent)]
    [InlineData(ResultSuccessHttpBehavior.Ok, StatusCodes.Status200OK)]
    public async Task AsHttpBuilder_NonGenericSuccess_UsesRegisteredPolicy(ResultSuccessHttpBehavior behavior,
                                                                          int                       expected)
    {
        // Arrange (Given)
        var context = CreateContext(o => o.Policy = new ResultHttpAdapterPolicy { ResultSuccessBehavior = behavior });

        // Act (When)
        await (await Result.Success().AsHttpBuilder()).ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal(expected, context.Response.StatusCode);
    }

    [Theory]
    [InlineData(MaybeNoneHttpBehavior.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(MaybeNoneHttpBehavior.NoContent, StatusCodes.Status204NoContent)]
    public async Task AsHttpBuilder_MaybeNone_UsesRegisteredPolicy(MaybeNoneHttpBehavior behavior,
                                                                  int                   expected)
    {
        // Arrange (Given)
        var context = CreateContext(o => o.Policy = new ResultHttpAdapterPolicy { MaybeNoneBehavior = behavior });

        // Act (When)
        await (await Maybe.None<int>().AsHttpBuilder()).ExecuteAsync(context);

        // Assert (Then)
        Assert.Equal(expected, context.Response.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithoutMapper_UsesMapperRegisteredByAddResultHttp()
    {
        // Arrange (Given)
        var context = CreateContext(o => o.AddMapper<BusinessFailure>(422));
        var result  = Result.Failure<int>(new BusinessFailure());

        // Act (When)
        await (await result.AsActionResultBuilder()).ExecuteResultAsync(ToActionContext(context));

        // Assert (Then)
        Assert.Equal(422, context.Response.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_NonGenericSuccess_UsesRegisteredPolicy()
    {
        // Arrange (Given)
        var context = CreateContext(o => o.Policy = new ResultHttpAdapterPolicy
        {
            ResultSuccessBehavior = ResultSuccessHttpBehavior.Ok
        });

        // Act (When)
        await (await Result.Success().AsActionResultBuilder()).ExecuteResultAsync(ToActionContext(context));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_MaybeNone_UsesRegisteredPolicy()
    {
        // Arrange (Given)
        var context = CreateContext(o => o.Policy = new ResultHttpAdapterPolicy
        {
            MaybeNoneBehavior = MaybeNoneHttpBehavior.NoContent
        });

        // Act (When)
        await (await Maybe.None<int>().AsActionResultBuilder()).ExecuteResultAsync(ToActionContext(context));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateContext(Action<ResultHttpOptions>? configure,
                                                    bool                       addResultHttp = true)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMvcCore();
        if (addResultHttp) {
            services.AddResultHttp(configure);
        }

        return new DefaultHttpContext { RequestServices = services.BuildServiceProvider() };
    }

    private static ActionContext ToActionContext(HttpContext context)
    {
        return new ActionContext(context, new RouteData(), new ActionDescriptor());
    }

    private sealed record BusinessFailure() : Failure("Business rule refused.");
}
