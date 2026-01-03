using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions;

public class ResultHttpExtensionsTests
{
    #region ToHttpResult - Minimal API

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
        var result = Result.Failure(new ValidationError(["Invalid input"]));

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
        var result = Result.Failure<int>(new NotFoundError("Item", "123"));

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

    #endregion

    #region ToActionResult - MVC Controllers

    [Fact(DisplayName = "ToActionResult returns OkResult for success")]
    public void ToActionResult_Success_ReturnsOkResult()
    {
        // Given
        var result = Result.Success();

        // When
        var actionResult = result.ToActionResult();

        // Then
        Assert.IsType<OkResult>(actionResult);
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

    #endregion
}
