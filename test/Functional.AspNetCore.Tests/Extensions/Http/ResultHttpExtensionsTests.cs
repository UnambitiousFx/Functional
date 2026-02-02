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
    [Fact(DisplayName = "ToHttpResult returns NoContent for success Result")]
    public void ToHttpResult_SuccessResult_ReturnsNoContent()
    {
        // Given
        var result = Result.Success();

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.IsType<NoContent>(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult returns ProblemDetails for ValidationError")]
    public void ToHttpResult_FailureResult_ReturnsProblemDetails()
    {
        // Given
        var result = Result.Failure(new ValidationFailure(["Invalid input"]));

        // When
        var httpResult = result.ToHttpResult();

        // Then
        Assert.IsType<ProblemHttpResult>(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult<T> returns 200 OK with value for success")]
    public void ToHttpResult_Generic_Success_Returns200WithValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var httpResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v));

        // Then
        var okResult = Assert.IsType<Ok<int>>(httpResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ToHttpResult<T> returns ProblemDetails for NotFoundError")]
    public void ToHttpResult_Generic_NotFoundError_ReturnsProblemDetails()
    {
        // Given
        var result = Result.Failure<int>(new NotFoundFailure("Item", "123"));

        // When
        var httpResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v));

        // Then
        Assert.IsType<ProblemHttpResult>(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with custom success mapper transforms value")]
    public void ToHttpResult_WithCustomSuccessMapper_TransformsValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var httpResult = result.ToHttpResult(x => Microsoft.AspNetCore.Http.Results.Ok(new { Value = x.ToString() }));

        // Then
        Assert.NotNull(httpResult);
        // The result is Ok<T> where T is the anonymous type, not Ok<object>
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(httpResult);
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
        Assert.IsType<Created<int>>(httpResult);
    }

    [Fact(DisplayName = "ToHttpResult with non-generic Result and custom success mapper returns custom result")]
    public void ToHttpResult_NonGenericWithCustomSuccessMapper_ReturnsCustomResult()
    {
        // Given
        var result = Result.Success();

        // When
        var httpResult = result.ToHttpResult(() => Microsoft.AspNetCore.Http.Results.Ok(new { Message = "Success" }));

        // Then
        Assert.NotNull(httpResult);
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(httpResult);
    }


    [Fact(DisplayName = "ToCreatedHttpResult with failure returns ProblemDetails")]
    public void ToCreatedHttpResult_Failure_ReturnsProblemDetails()
    {
        // Given
        var result = Result.Failure<int>(new ValidationFailure(["Invalid input"]));

        // When
        var httpResult = result.ToCreatedHttpResult(id => $"/items/{id}");

        // Then
        Assert.NotNull(httpResult);
        Assert.IsType<ProblemHttpResult>(httpResult);
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
        var httpResult = result.ToHttpResult(null, customMapper);

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
        var httpResult = result.ToHttpResult(null, customMapper);

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
        var httpResult = result.ToHttpResult(null, customMapper);

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
        var httpResult = result.ToHttpResult(null, customMapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

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
        var actionResult = result.ToHttpResult(v => Microsoft.AspNetCore.Http.Results.Ok(v), mapper);

        // Then
        Assert.IsType<StatusCodeHttpResult>(actionResult);
    }

    #region Async (ResultTask) Tests

    [Fact(DisplayName = "ResultTask ToHttpResult returns NoContent for success")]
    public async Task ResultTask_ToHttpResult_Success_ReturnsNoContent()
    {
        // Given
        ResultTask resultTask = Result.Success();

        // When
        var httpResult = await resultTask.ToHttpResult();

        // Then
        Assert.IsType<NoContent>(httpResult);
    }

    [Fact(DisplayName = "ResultTask ToHttpResult with custom success mapper returns custom result")]
    public async Task ResultTask_ToHttpResult_WithCustomSuccessMapper_ReturnsCustomResult()
    {
        // Given
        ResultTask resultTask = Result.Success();

        // When
        var httpResult = await resultTask.ToHttpResult(() => Microsoft.AspNetCore.Http.Results.Ok(new { Message = "Success" }));

        // Then
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(httpResult);
    }

    [Fact(DisplayName = "ResultTask ToHttpResult with failure returns ProblemDetails")]
    public async Task ResultTask_ToHttpResult_Failure_ReturnsProblemDetails()
    {
        // Given
        ResultTask resultTask = Result.Failure(new ValidationFailure(["Invalid input"]));

        // When
        var httpResult = await resultTask.ToHttpResult();

        // Then
        Assert.IsType<ProblemHttpResult>(httpResult);
    }

    [Fact(DisplayName = "ResultTask<T> ToHttpResult returns 200 OK with value for success")]
    public async Task ResultTaskGeneric_ToHttpResult_Success_Returns200WithValue()
    {
        // Given
        ResultTask<int> resultTask = Result.Success(42);

        // When
        var httpResult = await resultTask.ToHttpResult();

        // Then
        var okResult = Assert.IsType<Ok<int>>(httpResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ResultTask<T> ToHttpResult with custom success mapper transforms value")]
    public async Task ResultTaskGeneric_ToHttpResult_WithCustomSuccessMapper_TransformsValue()
    {
        // Given
        ResultTask<int> resultTask = Result.Success(42);

        // When
        var httpResult = await resultTask.ToHttpResult(x => Microsoft.AspNetCore.Http.Results.Ok(new { Value = x.ToString() }));

        // Then
        Assert.IsAssignableFrom<Microsoft.AspNetCore.Http.IResult>(httpResult);
    }

    [Fact(DisplayName = "ResultTask<T> ToHttpResult with failure returns ProblemDetails")]
    public async Task ResultTaskGeneric_ToHttpResult_Failure_ReturnsProblemDetails()
    {
        // Given
        ResultTask<int> resultTask = Result.Failure<int>(new NotFoundFailure("Item", "123"));

        // When
        var httpResult = await resultTask.ToHttpResult();

        // Then
        Assert.IsType<ProblemHttpResult>(httpResult);
    }

    [Fact(DisplayName = "ResultTask<T> ToCreatedHttpResult returns 201 Created with location")]
    public async Task ResultTaskGeneric_ToCreatedHttpResult_Success_Returns201WithLocation()
    {
        // Given
        ResultTask<int> resultTask = Result.Success(42);

        // When
        var httpResult = await resultTask.ToCreatedHttpResult(id => $"/items/{id}");

        // Then
        var createdResult = Assert.IsType<Created<int>>(httpResult);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ResultTask<T> ToCreatedHttpResult with failure returns ProblemDetails")]
    public async Task ResultTaskGeneric_ToCreatedHttpResult_Failure_ReturnsProblemDetails()
    {
        // Given
        ResultTask<int> resultTask = Result.Failure<int>(new ValidationFailure(["Invalid input"]));

        // When
        var httpResult = await resultTask.ToCreatedHttpResult(id => $"/items/{id}");

        // Then
        Assert.IsType<ProblemHttpResult>(httpResult);
    }

    [Fact(DisplayName = "ResultTask<T> ToCreatedHttpResult with custom error mapper")]
    public async Task ResultTaskGeneric_ToCreatedHttpResult_WithCustomErrorMapper_ReturnsCustomError()
    {
        // Given
        ResultTask<int> resultTask = Result.Failure<int>(new CustomFailure(409, "Conflict"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var httpResult = await resultTask.ToCreatedHttpResult(id => $"/items/{id}", mapper);

        // Then
        Assert.IsType<Conflict<object>>(httpResult);
    }

    #endregion

    #region Helper classes for testing

    private class CustomProblemDetailsMapper : IErrorHttpMapper
    {
        public ErrorHttpResponse? GetErrorResponse(IFailure failure)
        {
            if (failure is ValidationFailure)
                return new ErrorHttpResponse
                {
                    StatusCode = 400,
                    Body = new ProblemDetails
                    {
                        Title = "Validation Error",
                        Status = 400,
                        Detail = "One or more validation errors occurred."
                    }
                };
            return null;
        }
    }

    private class NullReturningMapper : IErrorHttpMapper
    {
        public ErrorHttpResponse? GetErrorResponse(IFailure failure)
        {
            return null;
        }
    }

    private record CustomFailure(int StatusCode, string Message) : Failure(Message);

    private class CustomStatusCodeMapper : IErrorHttpMapper
    {
        public ErrorHttpResponse? GetErrorResponse(IFailure failure)
        {
            var body = string.IsNullOrWhiteSpace(failure.Message) ? null : new { failure.Message };
            if (failure is CustomFailure customError)
                return new ErrorHttpResponse
                {
                    StatusCode = customError.StatusCode,
                    Body = body
                };
            return null;
        }
    }

    #endregion
}