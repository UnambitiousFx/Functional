using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;
using IHttpResult = Microsoft.AspNetCore.Http.IResult;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Acceptance;

public class AdapterAcceptanceTests
{
    public static IEnumerable<object[]> CommonFailures()
    {
        yield return [new ValidationFailure(["Invalid input"]), StatusCodes.Status400BadRequest];
        yield return [new NotFoundFailure("User", "123"), StatusCodes.Status404NotFound];
        yield return [new ConflictFailure("Conflict"), StatusCodes.Status409Conflict];
        yield return [new UnauthorizedFailure("Unauthorized"), StatusCodes.Status401Unauthorized];
        yield return [new UnauthenticatedFailure("Forbidden"), StatusCodes.Status403Forbidden];
        yield return
            [new ExceptionalFailure(new InvalidOperationException("Boom")), StatusCodes.Status500InternalServerError];
    }

    [Theory(DisplayName = "Same failure maps to equivalent status code in Minimal API and MVC")]
    [MemberData(nameof(CommonFailures))]
    public async Task SameFailure_MinimalAndMvc_MapToEquivalentStatusCode(Failure failure,
                                                                          int expectedStatusCode)
    {
        // Arrange (Given)
        var result = Result.Fail(failure);

        // Act (When)
        var minimalStatusCode = GetMinimalStatusCode(await result.AsHttpBuilder());
        var mvcStatusCode = GetMvcStatusCode(await result.AsActionResultBuilder());

        // Assert (Then)
        Assert.Equal(expectedStatusCode, minimalStatusCode);
        Assert.Equal(expectedStatusCode, mvcStatusCode);
    }

    [Fact(DisplayName = "Maybe.None handling is available and policy-based in Minimal API and MVC")]
    public async Task MaybeNoneHandling_IsPolicyBased_AcrossMinimalAndMvc()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var minimalDefault = GetMinimalStatusCode(await maybe.AsHttpBuilder());
        var mvcDefault = GetMvcStatusCode(await maybe.AsActionResultBuilder());
        var minimalNoContent = GetMinimalStatusCode(await maybe.AsHttpBuilder().AsNone(() => Results.NoContent()));
        var mvcNoContent = GetMvcStatusCode(await maybe.AsActionResultBuilder().AsNone(() => new NoContentResult()));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status404NotFound, minimalDefault);
        Assert.Equal(StatusCodes.Status404NotFound, mvcDefault);
        Assert.Equal(StatusCodes.Status204NoContent, minimalNoContent);
        Assert.Equal(StatusCodes.Status204NoContent, mvcNoContent);
    }

    private static int GetMinimalStatusCode(IHttpResult result)
    {
        var inner = UnwrapMinimalResult(result);

        return inner switch
        {
            IStatusCodeHttpResult { StatusCode: { } statusCode } => statusCode,
            ForbidHttpResult => StatusCodes.Status403Forbidden,
            _ => throw new InvalidOperationException(
                     $"Unable to resolve status code from minimal result type: {inner.GetType().Name}")
        };
    }

    private static int GetMvcStatusCode(IActionResult result)
    {
        var inner = UnwrapActionResult(result);

        return inner switch
        {
            ObjectResult { StatusCode: { } statusCode } => statusCode,
            StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
            ForbidResult => StatusCodes.Status403Forbidden,
            _ => throw new InvalidOperationException(
                     $"Unable to resolve status code from MVC result type: {inner.GetType().Name}")
        };
    }

    private static IHttpResult UnwrapMinimalResult(IHttpResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as IHttpResult ?? result;
    }

    private static IActionResult UnwrapActionResult(IActionResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as IActionResult ?? result;
    }
}
