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
            {
                return (400, new ProblemDetails
                {
                    Title = "Validation Error",
                    Status = 400,
                    Detail = "One or more validation errors occurred."
                });
            }
            return null;
        }
    }

    private class NullReturningMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IError error) => null;
    }

    private record CustomError(int StatusCode, string Message) : Error(Message);

    private class CustomStatusCodeMapper : IErrorHttpMapper
    {
        public (int StatusCode, object? Body)? GetResponse(IError error)
        {
            if (error is CustomError customError)
            {
                return (customError.StatusCode, new { Message = error.Message });
            }
            return null;
        }
    }

    #endregion

}
