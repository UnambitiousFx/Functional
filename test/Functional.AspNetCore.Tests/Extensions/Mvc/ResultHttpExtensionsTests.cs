using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc;

public class ResultHttpExtensionsTests
{
    [Fact(DisplayName = "ToActionResult returns NoContentResult for success")]
    public void ToActionResult_Success_ReturnsNoContentResult()
    {
        // Given
        var result = Result.Success();

        // When
        var actionResult = result.ToActionResult();

        // Then
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult returns ObjectResult with 500 for ExceptionalError")]
    public void ToActionResult_Failure_ReturnsObjectResultWith500()
    {
        // Given
        var result = Result.Failure("Something went wrong"); // Creates ExceptionalError

        // When
        var actionResult = result.ToActionResult();

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult<T> returns OkObjectResult with value for success")]
    public void ToActionResult_Generic_Success_ReturnsOkObjectResult()
    {
        // Given
        var result = Result.Success(42);

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v));

        // Then
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ToActionResult<T> returns ObjectResult with 404 for NotFoundError")]
    public void ToActionResult_Generic_NotFoundError_Returns404()
    {
        // Given
        var result = Result.Failure<int>(new NotFoundFailure("Item", "123"));

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v));

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom success mapper transforms value")]
    public void ToActionResult_WithCustomSuccessMapper_TransformsValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var actionResult = result.ToActionResult(x => new OkObjectResult(new { Value = x.ToString() }));

        // Then
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResult returns CreatedAtActionResult")]
    public void ToCreatedActionResult_Success_ReturnsCreatedAtActionResult()
    {
        // Given
        var result = Result.Success(42);

        // When
        var actionResult = result.ToCreatedActionResult("GetItem", null, new { id = 42 });

        // Then
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetItem", createdResult.ActionName);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResult with controller name")]
    public void ToCreatedActionResult_WithControllerName_ReturnsCreatedAtActionResult()
    {
        // Given
        var result = Result.Success(42);

        // When
        var actionResult = result.ToCreatedActionResult("GetItem", "Items", new { id = 42 });

        // Then
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetItem", createdResult.ActionName);
        Assert.Equal("Items", createdResult.ControllerName);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResult with custom mapper returns custom status code")]
    public void ToCreatedActionResult_WithCustomStatusCodeMapper_ReturnsCustomStatusCode()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(418, "I'm a teapot"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToCreatedActionResult("GetItem", null, null, mapper);

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(418, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 400 returns BadRequestObjectResult")]
    public void ToActionResult_WithCustomErrorMapper400_ReturnsBadRequestObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(400, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        var objectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 401 returns UnauthorizedObjectResult")]
    public void ToActionResult_WithCustomErrorMapper401_ReturnsUnauthorizedObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(401, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        var objectResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 403 returns ForbidResult")]
    public void ToActionResult_WithCustomErrorMapper402_ReturnsForbidResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(403, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        Assert.IsType<ForbidResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 404 returns NotFoundObjectResult")]
    public void ToActionResult_WithCustomErrorMapper404_ReturnsNotFoundObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(404, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 409 returns ConflictObjectResult")]
    public void ToActionResult_WithCustomErrorMapper409_ReturnsConflictObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(409, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        Assert.IsType<ConflictObjectResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 500 returns ObjectResult")]
    public void ToActionResult_WithCustomErrorMapper409_ReturnsObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(500, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        Assert.IsType<ObjectResult>(actionResult);
    }

    [Fact(DisplayName =
        "ToActionResult with custom mapper with status code 500 and null body returns StatusCodeResult")]
    public void ToActionResult_WithCustomErrorMapperNullBody_ReturnsStatusCodeResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomFailure(500, string.Empty));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(v => new OkObjectResult(v), mapper);

        // Then
        Assert.IsType<StatusCodeResult>(actionResult);
    }

    [Fact(DisplayName = "ToCreatedActionResult returns error result on failure")]
    public void ToCreatedActionResult_Failure_ReturnsErrorResult()
    {
        // Given
        var result = Result.Failure<int>(new ValidationFailure(["Invalid input"]));

        // When
        var actionResult = result.ToCreatedActionResult("GetItem", null);

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with non-generic Result and custom success mapper returns custom result")]
    public void ToActionResult_NonGenericWithCustomSuccessMapper_ReturnsCustomResult()
    {
        // Given
        var result = Result.Success();

        // When
        var actionResult = result.ToActionResult(() => new OkObjectResult(new { Message = "Success" }));

        // Then
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    #region Async (ResultTask) Tests

    [Fact(DisplayName = "ResultTask ToActionResult returns NoContentResult for success")]
    public async Task ResultTask_ToActionResult_Success_ReturnsNoContentResult()
    {
        // Given
        ResultTask resultTask = Result.Success();

        // When
        var actionResult = await resultTask.ToActionResult();

        // Then
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact(DisplayName = "ResultTask ToActionResult with custom success mapper returns custom result")]
    public async Task ResultTask_ToActionResult_WithCustomSuccessMapper_ReturnsCustomResult()
    {
        // Given
        ResultTask resultTask = Result.Success();

        // When
        var actionResult = await resultTask.ToActionResult(() => new OkObjectResult(new { Message = "Success" }));

        // Then
        Assert.IsType<OkObjectResult>(actionResult);
    }

    [Fact(DisplayName = "ResultTask ToActionResult with failure returns error result")]
    public async Task ResultTask_ToActionResult_Failure_ReturnsErrorResult()
    {
        // Given
        ResultTask resultTask = Result.Failure(new ValidationFailure(["Invalid input"]));

        // When
        var actionResult = await resultTask.ToActionResult();

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ResultTask<T> ToActionResult returns OkObjectResult with value for success")]
    public async Task ResultTaskGeneric_ToActionResult_Success_ReturnsOkObjectResult()
    {
        // Given
        ResultTask<int> resultTask = Result.Success(42);

        // When
        var actionResult = await resultTask.ToActionResult();

        // Then
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ResultTask<T> ToActionResult with custom success mapper transforms value")]
    public async Task ResultTaskGeneric_ToActionResult_WithCustomSuccessMapper_TransformsValue()
    {
        // Given
        ResultTask<int> resultTask = Result.Success(42);

        // When
        var actionResult = await resultTask.ToActionResult(x => new OkObjectResult(new { Value = x.ToString() }));

        // Then
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    [Fact(DisplayName = "ResultTask<T> ToActionResult with failure returns error result")]
    public async Task ResultTaskGeneric_ToActionResult_Failure_ReturnsErrorResult()
    {
        // Given
        ResultTask<int> resultTask = Result.Failure<int>(new NotFoundFailure("Item", "123"));

        // When
        var actionResult = await resultTask.ToActionResult();

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ResultTask<T> ToCreatedActionResult returns CreatedAtActionResult")]
    public async Task ResultTaskGeneric_ToCreatedActionResult_Success_ReturnsCreatedAtActionResult()
    {
        // Given
        ResultTask<int> resultTask = Result.Success(42);

        // When
        var actionResult = await resultTask.ToCreatedActionResult("GetItem", null, new { id = 42 });

        // Then
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetItem", createdResult.ActionName);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ResultTask<T> ToCreatedActionResult with failure returns error result")]
    public async Task ResultTaskGeneric_ToCreatedActionResult_Failure_ReturnsErrorResult()
    {
        // Given
        ResultTask<int> resultTask = Result.Failure<int>(new ValidationFailure(["Invalid input"]));

        // When
        var actionResult = await resultTask.ToCreatedActionResult("GetItem", null);

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ResultTask<T> ToCreatedActionResult with custom error mapper")]
    public async Task ResultTaskGeneric_ToCreatedActionResult_WithCustomErrorMapper_ReturnsCustomError()
    {
        // Given
        ResultTask<int> resultTask = Result.Failure<int>(new CustomFailure(409, "Conflict"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = await resultTask.ToCreatedActionResult("GetItem", null, null, mapper);

        // Then
        var objectResult = Assert.IsType<ConflictObjectResult>(actionResult);
        Assert.Equal(409, objectResult.StatusCode);
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