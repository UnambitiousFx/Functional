using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Errors;

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
        var actionResult = result.ToActionResult();

        // Then
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ToActionResult<T> returns ObjectResult with 404 for NotFoundError")]
    public void ToActionResult_Generic_NotFoundError_Returns404()
    {
        // Given
        var result = Result.Failure<int>(new NotFoundError("Item", "123"));

        // When
        var actionResult = result.ToActionResult();

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with DTO mapper transforms value")]
    public void ToActionResult_WithDtoMapper_TransformsValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var actionResult = result.ToActionResult(x => new { Value = x.ToString() });

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
        var actionResult = result.ToCreatedActionResult("GetItem", new { id = 42 });

        // Then
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetItem", createdResult.ActionName);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResult with DTO mapper transforms value")]
    public void ToCreatedActionResult_WithDtoMapper_TransformsValue()
    {
        // Given
        var result = Result.Success(42);

        // When
        var actionResult = result.ToCreatedActionResult(
            "GetItem",
            x => new { Id = x, Name = "Item" },
            new { id = 42 });

        // Then
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.NotNull(createdResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResult with custom mapper returns custom status code")]
    public void ToCreatedActionResult_WithCustomStatusCodeMapper_ReturnsCustomStatusCode()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(418, "I'm a teapot"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToCreatedActionResult("GetItem", mapper: mapper);

        // Then
        var objectResult = Assert.IsType<StatusCodeResult>(actionResult);
        Assert.Equal(418, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 400 returns BadRequestObjectResult")]
    public void ToActionResult_WithCustomErrorMapper400_ReturnsBadRequestObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(400, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        var objectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 401 returns UnauthorizedObjectResult")]
    public void ToActionResult_WithCustomErrorMapper401_ReturnsUnauthorizedObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(401, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        var objectResult = Assert.IsType<UnauthorizedObjectResult>(actionResult);
        Assert.Equal(401, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 403 returns ForbidResult")]
    public void ToActionResult_WithCustomErrorMapper402_ReturnsForbidResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(403, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        Assert.IsType<ForbidResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 404 returns NotFoundObjectResult")]
    public void ToActionResult_WithCustomErrorMapper404_ReturnsNotFoundObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(404, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        Assert.IsType<NotFoundObjectResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 409 returns ConflictObjectResult")]
    public void ToActionResult_WithCustomErrorMapper409_ReturnsConflictObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(409, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        Assert.IsType<ConflictObjectResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResult with custom mapper with status code 500 returns ObjectResult")]
    public void ToActionResult_WithCustomErrorMapper409_ReturnsObjectResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(500, "Invalid input"));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        Assert.IsType<ObjectResult>(actionResult);
    }

    [Fact(DisplayName =
        "ToActionResult with custom mapper with status code 500 and null body returns StatusCodeResult")]
    public void ToActionResult_WithCustomErrorMapperNullBody_ReturnsStatusCodeResult()
    {
        // Given
        var result = Result.Failure<int>(new CustomError(500, string.Empty));
        var mapper = new CustomStatusCodeMapper();

        // When
        var actionResult = result.ToActionResult(mapper);

        // Then
        Assert.IsType<StatusCodeResult>(actionResult);
    }

    [Fact(DisplayName = "ToCreatedActionResult returns error result on failure")]
    public void ToCreatedActionResult_Failure_ReturnsErrorResult()
    {
        // Given
        var result = Result.Failure<int>(new ValidationError(["Invalid input"]));

        // When
        var actionResult = result.ToCreatedActionResult("GetItem");

        // Then
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }


    #region Helper classes for testing

    private class CustomProblemDetailsMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IError error)
        {
            if (error is ValidationError)
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
        public (int StatusCode, object? Body)? GetResponse(IError error)
        {
            return null;
        }
    }

    private record CustomError(int StatusCode, string Message) : Error(Message);

    private class CustomStatusCodeMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IError error)
        {
            var body = string.IsNullOrWhiteSpace(error.Message) ? null : new { error.Message };
            if (error is CustomError customError) return (customError.StatusCode, body);
            return null;
        }
    }

    #endregion
}