using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.AspNetCore.Mvc.ValueTasks;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc.ValueTasks;

public class ResultHttpExtensionsAsyncTests
{
    [Fact(DisplayName = "ToActionResultAsync returns NoContentResult for successful ValueTask<Result>")]
    public async Task ToActionResultAsync_SuccessResult_ReturnsNoContentResult()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var actionResult = await result.ToActionResultAsync();

        // Assert (Then)
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResultAsync returns error result for failed ValueTask<Result>")]
    public async Task ToActionResultAsync_FailureResult_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new ValidationError(["Invalid input"])));

        // Act (When)
        var actionResult = await result.ToActionResultAsync();

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResultAsync<T> returns OkObjectResult with value for successful ValueTask<Result<T>>")]
    public async Task ToActionResultAsync_Generic_Success_ReturnsOkObjectResult()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await result.ToActionResultAsync();

        // Assert (Then)
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync<T> returns error for failed ValueTask<Result<T>>")]
    public async Task ToActionResultAsync_Generic_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new NotFoundError("Item", "123")));

        // Act (When)
        var actionResult = await result.ToActionResultAsync();

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToHttpResultAsync with DTO mapper transforms value for ValueTask<Result>")]
    public async Task ToHttpResultAsync_WithDtoMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var actionResult = await result.ToActionResultAsync(() => new { Status = "Success" });

        // Assert (Then)
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync<T, TDto> with DTO mapper transforms value")]
    public async Task ToActionResultAsync_Generic_WithDtoMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await result.ToActionResultAsync(x => new { Value = x.ToString() });

        // Assert (Then)
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.NotNull(okResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResultAsync returns CreatedAtActionResult for success")]
    public async Task ToCreatedActionResultAsync_Success_ReturnsCreatedAtActionResult()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await result.ToCreatedActionResultAsync("GetItem", new { id = 42 });

        // Assert (Then)
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal("GetItem", createdResult.ActionName);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResultAsync returns error for failure")]
    public async Task ToCreatedActionResultAsync_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new ValidationError(["Invalid"])));

        // Act (When)
        var actionResult = await result.ToCreatedActionResultAsync("GetItem");

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToCreatedActionResultAsync with DTO mapper transforms value")]
    public async Task ToCreatedActionResultAsync_WithDtoMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await result.ToCreatedActionResultAsync(
            "GetItem",
            x => new { Id = x, Name = "Item" },
            new { id = 42 });

        // Assert (Then)
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.NotNull(createdResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync awaits async operation correctly")]
    public async Task ToActionResultAsync_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var actionResult = await valueTask.ToActionResultAsync();

        // Assert (Then)
        Assert.IsType<NoContentResult>(actionResult);
    }

    [Fact(DisplayName = "ToActionResultAsync<T> awaits async operation correctly")]
    public async Task ToActionResultAsync_Generic_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await valueTask.ToActionResultAsync();

        // Assert (Then)
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(42, okResult.Value);
    }

    [Fact(DisplayName = "ToCreatedActionResultAsync awaits async operation correctly")]
    public async Task ToCreatedActionResultAsync_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await valueTask.ToCreatedActionResultAsync("GetItem");

        // Assert (Then)
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync with custom httpMapper returns custom result for success")]
    public async Task ToActionResultAsync_WithCustomHttpMapper_Success_ReturnsCustomResult()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            httpMapper: () => new OkObjectResult("Custom success response"));

        // Assert (Then)
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal("Custom success response", okResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync with custom httpMapper returns error for failure")]
    public async Task ToActionResultAsync_WithCustomHttpMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new ValidationError(["Validation failed"])));

        // Act (When)
        var actionResult = await result.ToActionResultAsync(() => new OkObjectResult("Custom success response"));

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResultAsync with custom httpMapper and custom errorMapper uses custom error mapping")]
    public async Task ToActionResultAsync_WithCustomHttpMapperAndErrorMapper_UsesCustomErrorMapping()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new CustomError(418, "I'm a teapot")));
        var errorMapper = new CustomStatusCodeMapper();

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            httpMapper: () => new OkObjectResult("Success"),
            errorMapper);

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(418, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResultAsync<TValue> with dtoMapper and httpMapper transforms value for success")]
    public async Task ToActionResultAsync_Generic_WithDtoMapperAndHttpMapper_Success_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            () => new { Status = "Complete" },
            dto => new AcceptedResult("/status", dto));

        // Assert (Then)
        var acceptedResult = Assert.IsType<AcceptedResult>(actionResult);
        Assert.NotNull(acceptedResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync<TValue> with dtoMapper and httpMapper returns error for failure")]
    public async Task ToActionResultAsync_Generic_WithDtoMapperAndHttpMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new NotFoundError("Resource", "123")));

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            () => new { Status = "Complete" },
            dto => new AcceptedResult("/status", dto));

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(404, objectResult.StatusCode);
    }

    [Fact(DisplayName =
        "ToActionResultAsync<TValue> with dtoMapper, httpMapper and custom errorMapper uses custom error mapping")]
    public async Task ToActionResultAsync_Generic_WithDtoMapperHttpMapperAndErrorMapper_UsesCustomErrorMapping()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new CustomError(418, "Custom error")));
        var errorMapper = new CustomStatusCodeMapper();

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            () => new { Status = "Complete" },
            dto => new AcceptedResult("/status", dto),
            errorMapper);

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(418, objectResult.StatusCode);
    }

    [Fact(DisplayName = "ToActionResultAsync<TValue,TDto> with dtoMapper and httpMapper transforms value for success")]
    public async Task ToActionResultAsync_GenericWithDto_WithDtoMapperAndHttpMapper_Success_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            x => new { Value = x.ToString(), Doubled = x * 2 },
            (value, dto) => new CreatedAtActionResult("GetItem", null, new { id = value }, dto));

        // Assert (Then)
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult);
        Assert.NotNull(createdResult.Value);
    }

    [Fact(DisplayName = "ToActionResultAsync<TValue,TDto> with dtoMapper and httpMapper returns error for failure")]
    public async Task ToActionResultAsync_GenericWithDto_WithDtoMapperAndHttpMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new ValidationError(["Invalid value"])));

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            x => new { Value = x.ToString(), Doubled = x * 2 },
            (value, dto) => new CreatedAtActionResult("GetItem", null, new { id = value }, dto));

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact(DisplayName =
        "ToActionResultAsync<TValue,TDto> with dtoMapper, httpMapper and custom errorMapper uses custom error mapping")]
    public async Task ToActionResultAsync_GenericWithDto_WithDtoMapperHttpMapperAndErrorMapper_UsesCustomErrorMapping()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new CustomError(418, "Custom error")));
        var errorMapper = new CustomStatusCodeMapper();

        // Act (When)
        var actionResult = await result.ToActionResultAsync(
            x => new { Value = x.ToString(), Doubled = x * 2 },
            (value, dto) => new CreatedAtActionResult("GetItem", null, new { id = value }, dto),
            errorMapper);

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(418, objectResult.StatusCode);
    }

    #region Helper classes for testing

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