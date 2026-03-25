using Microsoft.AspNetCore.Http;
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
        yield return [new ExceptionalFailure(new InvalidOperationException("Boom")), StatusCodes.Status500InternalServerError];
    }

    [Theory(DisplayName = "Same failure maps to equivalent status code in Minimal API and MVC")]
    [MemberData(nameof(CommonFailures))]
    public void SameFailure_MinimalAndMvc_MapToEquivalentStatusCode(Failure failure, int expectedStatusCode)
    {
        // Arrange (Given)
        var result = Result.Fail(failure);

        // Act (When)
        var minimalStatusCode = GetMinimalStatusCode(result.ToHttpResult());
        var mvcStatusCode = GetMvcStatusCode(result.ToActionResult());

        // Assert (Then)
        Assert.Equal(expectedStatusCode, minimalStatusCode);
        Assert.Equal(expectedStatusCode, mvcStatusCode);
    }

    [Fact(DisplayName = "Maybe.None handling is available and policy-based in Minimal API and MVC")]
    public void MaybeNoneHandling_IsPolicyBased_AcrossMinimalAndMvc()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var policy = new ResultHttpAdapterPolicy
        {
            MaybeNoneBehavior = MaybeNoneHttpBehavior.NoContent
        };

        // Act (When)
        var minimalDefault = GetMinimalStatusCode(maybe.ToHttpResult());
        var mvcDefault = GetMvcStatusCode(maybe.ToActionResult());
        var minimalNoContent = GetMinimalStatusCode(maybe.ToHttpResult(policy: policy));
        var mvcNoContent = GetMvcStatusCode(maybe.ToActionResult(policy: policy));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status404NotFound, minimalDefault);
        Assert.Equal(StatusCodes.Status404NotFound, mvcDefault);
        Assert.Equal(StatusCodes.Status204NoContent, minimalNoContent);
        Assert.Equal(StatusCodes.Status204NoContent, mvcNoContent);
    }

    private static int GetMinimalStatusCode(IHttpResult result)
    {
        if (result is IStatusCodeHttpResult statusCodeHttpResult && statusCodeHttpResult.StatusCode is int statusCode)
        {
            return statusCode;
        }

        throw new InvalidOperationException($"Unable to resolve status code from minimal result type: {result.GetType().Name}");
    }

    private static int GetMvcStatusCode(IActionResult result)
    {
        return result switch
        {
            ObjectResult objectResult when objectResult.StatusCode is int statusCode => statusCode,
            StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
            _ => throw new InvalidOperationException($"Unable to resolve status code from MVC result type: {result.GetType().Name}")
        };
    }
}
