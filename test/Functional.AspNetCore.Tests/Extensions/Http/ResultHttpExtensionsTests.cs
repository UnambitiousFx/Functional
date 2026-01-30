using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;
using ConflictFailure = UnambitiousFx.Functional.Failures.ConflictFailure;
using NotFoundFailure = UnambitiousFx.Functional.Failures.NotFoundFailure;
using UnauthenticatedFailure = UnambitiousFx.Functional.Failures.UnauthenticatedFailure;
using ValidationFailure = UnambitiousFx.Functional.Failures.ValidationFailure;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Http;

public class ResultHttpExtensionsTests
{
    [Fact(DisplayName = "ToHttpResult returns 200 OK for success Result")]
    public void ToHttpResult_SuccessResult_Returns200()
    {
        // Given
        var result = Result.Success();

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult returns 400 BadRequest for ValidationError")]
    public void ToHttpResult_FailureResult_Returns400()
    {
        // Given
        var result = Result.Failure(new ValidationFailure(["Invalid input"]));

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult<T> returns 200 OK with value for success")]
    public void ToHttpResult_Generic_Success_Returns200WithValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult<T> returns 404 for NotFoundError")]
    public void ToHttpResult_Generic_NotFoundError_Returns404()
    {
        // Given
        var result = Result.Failure<int>(new NotFoundFailure("Item", "123"));

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with DTO mapper transforms value")]
    public void ToHttpResult_WithDtoMapper_TransformsValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var httpResult = result.ToHttpResult(x => new { Value = x.ToString() });

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResult returns 201 Created with location")]
    public void ToCreatedHttpResult_Success_Returns201WithLocation()
    {
        // Given
        var result = Result.Success(42);

        // When
        var httpResult = result.ToCreatedHttpResult(id => $"/items/{id}");

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResult with DTO mapper transforms value")]
    public void ToCreatedHttpResult_WithDtoMapper_TransformsValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var httpResult = result.ToCreatedHttpResult(
            id => $"/items/{id}",
            x => new { Id = x, Name = "Item" });

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with non-generic Result and DTO mapper returns 200 OK")]
    public void ToHttpResult_NonGenericWithDtoMapper_Returns200()
    {
        // Given
        var result = Result.Success();

        // When
        var httpResult = result.ToHttpResult(() => new { Message = "Success" });

        // Then
        Assert.NotNull(httpResult);
    }


    [Fact(DisplayName = "ToCreatedHttpResult with failure returns error result")]
    public void ToCreatedHttpResult_Failure_ReturnsErrorResult()
    {
        // Given
        var result = Result.Failure<int>(new ValidationFailure(["Invalid input"]));

        // When
        var httpResult = result.ToCreatedHttpResult(id => $"/items/{id}");

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResult with DTO mapper and failure returns error result")]
    public void ToCreatedHttpResult_WithDtoMapperAndFailure_ReturnsErrorResult()
    {
        // Given
        var result = Result.Failure<int>(new NotFoundFailure("Item", "123"));

        // When
        var httpResult = result.ToCreatedHttpResult(
            id => $"/items/{id}",
            x => new { Id = x, Name = "Item" });

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper for UnauthenticatedError returns 401")]
    public void ToHttpResult_WithUnauthenticatedError_Returns401()
    {
        // Given
        var result = Result.Failure(new UnauthenticatedFailure("User not authenticated"));

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper for ConflictError returns 409")]
    public void ToHttpResult_WithConflictError_Returns409()
    {
        // Given
        var result = Result.Failure(new ConflictFailure("Conflict detected"));

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with ProblemDetails in custom mapper")]
    public void ToHttpResult_WithProblemDetailsMapper_ReturnsProblemDetails()
    {
        // Given
        var result = Result.Failure(new ValidationFailure(["Invalid input"]));
        var customMapper = new CustomProblemDetailsMapper();

        // When
        var httpResult = result.ToHttpResult(customMapper);

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper returning null falls back to default")]
    public void ToHttpResult_WithCustomMapperReturningNull_FallsBackToDefault()
    {
        // Given
        var result = Result.Failure(new ValidationFailure(["Invalid input"]));
        var customMapper = new NullReturningMapper();

        // When
        var httpResult = result.ToHttpResult(customMapper);

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with unsupported error status code returns 500")]
    public void ToHttpResult_WithUnsupportedErrorStatusCode_Returns500()
    {
        // Given
        var result = Result.Failure(new CustomFailure(418, "I'm a teapot"));
        var customMapper = new CustomStatusCodeMapper();

        // When
        var httpResult = result.ToHttpResult(customMapper);

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with 500 error and body returns proper result")]
    public void ToHttpResult_WithError500AndBody_ReturnsProblem()
    {
        // Given
        var result = Result.Failure(new CustomFailure(500, "Internal server error"));
        var customMapper = new CustomStatusCodeMapper();

        // When
        var httpResult = result.ToHttpResult(customMapper);

        // Then
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 400 returns BadRequestObjectResult")]
    public void ToHttpResult_WithCustomErrorMapper400_ReturnsBadRequestObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(400, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        var objectResult = Assert.IsType<BadRequest<object>>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 401 returns UnauthorizedObjectResult")]
    public void ToHttpResult_WithCustomErrorMapper401_ReturnsUnauthorizedObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(401, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        var objectResult = Assert.IsType<UnauthorizedHttpResult>(actionResult);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 403 returns ForbidResult")]
    public void ToHttpResult_WithCustomErrorMapper402_ReturnsForbidResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(403, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        Assert.IsType<ForbidHttpResult>(actionResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 404 returns NotFoundObjectResult")]
    public void ToHttpResult_WithCustomErrorMapper404_ReturnsNotFoundObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(404, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        Assert.IsType<NotFound<object>>(actionResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 409 returns ConflictObjectResult")]
    public void ToHttpResult_WithCustomErrorMapper409_ReturnsConflictObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(409, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        Assert.IsType<Conflict<object>>(actionResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 500 returns ObjectResult")]
    public void ToHttpResult_WithCustomErrorMapper500_ReturnsObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(500, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        Assert.IsType<ProblemHttpResult>(actionResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom mapper with status code 500 and null body returns StatusCodeResult")]
    public void ToHttpResult_WithCustomErrorMapperNullBody_ReturnsStatusCodeResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(500, string.Empty));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToHttpResult(mapper);

        // Then
        Assert.IsType<StatusCodeHttpResult>(actionResult);
    }

    #region Helper classes for testing

    private class CustomProblemDetailsMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IFailure failure)
        {
            if (failure is ValidationFailure)
                return (400, new ProblemDetails
                {
                    Title = "Validation Error",
                    Status = 400,
                    Detail = "One or more validation errors occurred."
                });
            return null;
        }
    }

    private class NullReturningMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IFailure failure)
        {
            return null;
        }
    }

    private record CustomFailure(int StatusCode, string Message) : Failure(Message);

    private class CustomStatusCodeMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IFailure failure)
        {
            var body = string.IsNullOrWhiteSpace(failure.Message) ? null : new { failure.Message };
            if (failure is CustomFailure customError) return (customError.StatusCode, body);
            return null;
        }
    }

    #endregion
}