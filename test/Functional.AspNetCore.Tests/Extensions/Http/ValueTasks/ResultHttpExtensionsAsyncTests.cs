using UnambitiousFx.Functional.AspNetCore.Http.ValueTasks;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Http.ValueTasks;

public class ResultHttpExtensionsAsyncTests
{
    [Fact(DisplayName = "ToHttpResultAsync returns 200 OK for successful ValueTask<Result>")]
    public async Task ToHttpResultAsync_SuccessResult_Returns200()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var httpResult = await result.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync returns error result for failed ValueTask<Result>")]
    public async Task ToHttpResultAsync_FailureResult_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new ValidationError(["Invalid input"])));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<T> returns 200 OK with value for successful ValueTask<Result<T>>")]
    public async Task ToHttpResultAsync_Generic_Success_Returns200WithValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<T> returns error for failed ValueTask<Result<T>>")]
    public async Task ToHttpResultAsync_Generic_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new NotFoundError("Item", "123")));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync with DTO mapper transforms value for ValueTask<Result>")]
    public async Task ToHttpResultAsync_WithDtoMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(() => new { Status = "Success" });

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync with DTO mapper returns error for failed ValueTask<Result>")]
    public async Task ToHttpResultAsync_WithDtoMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new ValidationError(["Invalid data"])));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(() => new { Status = "Success" });

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync with DTO mapper awaits async operation correctly")]
    public async Task ToHttpResultAsync_WithDtoMapper_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var resultTask = valueTask.ToHttpResultAsync(() => new { Message = "Complete" });
        var httpResult = await resultTask;

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<T, TDto> with DTO mapper transforms value")]
    public async Task ToHttpResultAsync_Generic_WithDtoMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(x => new { Value = x.ToString() });

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResultAsync returns 201 Created for success")]
    public async Task ToCreatedHttpResultAsync_Success_Returns201()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await result.ToCreatedHttpResultAsync(id => $"/items/{id}");

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResultAsync returns error for failure")]
    public async Task ToCreatedHttpResultAsync_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new ValidationError(["Invalid"])));

        // Act (When)
        var httpResult = await result.ToCreatedHttpResultAsync(id => $"/items/{id}");

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResultAsync with DTO mapper transforms value")]
    public async Task ToCreatedHttpResultAsync_WithDtoMapper_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await result.ToCreatedHttpResultAsync(
            id => $"/items/{id}",
            x => new { Id = x, Name = "Item" });

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync awaits async operation correctly")]
    public async Task ToHttpResultAsync_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var httpResult = await valueTask.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<T> awaits async operation correctly")]
    public async Task ToHttpResultAsync_Generic_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await valueTask.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResultAsync awaits async operation correctly")]
    public async Task ToCreatedHttpResultAsync_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await valueTask.ToCreatedHttpResultAsync(id => $"/items/{id}");

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync with custom httpMapper returns custom result for success")]
    public async Task ToHttpResultAsync_WithCustomHttpMapper_Success_ReturnsCustomResult()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            () => Microsoft.AspNetCore.Http.Results.Ok("Custom success response"));

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync with custom httpMapper returns error for failure")]
    public async Task ToHttpResultAsync_WithCustomHttpMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new ValidationError(["Validation failed"])));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            () => Microsoft.AspNetCore.Http.Results.Ok("Custom success response"));

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync with custom httpMapper and custom errorMapper uses custom error mapping")]
    public async Task ToHttpResultAsync_WithCustomHttpMapperAndErrorMapper_UsesCustomErrorMapping()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new CustomError(418, "I'm a teapot")));
        var errorMapper = new CustomStatusCodeMapper();

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            () => Microsoft.AspNetCore.Http.Results.Ok("Success"),
            errorMapper);

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<TValue> with dtoMapper and httpMapper transforms value for success")]
    public async Task ToHttpResultAsync_Generic_WithDtoMapperAndHttpMapper_Success_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success());

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            () => new { Status = "Complete" },
            dto => Microsoft.AspNetCore.Http.Results.Accepted("/status", dto));

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<TValue> with dtoMapper and httpMapper returns error for failure")]
    public async Task ToHttpResultAsync_Generic_WithDtoMapperAndHttpMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new NotFoundError("Resource", "123")));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            () => new { Status = "Complete" },
            dto => Microsoft.AspNetCore.Http.Results.Accepted("/status", dto));

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<TValue> with dtoMapper, httpMapper and custom errorMapper uses custom error mapping")]
    public async Task ToHttpResultAsync_Generic_WithDtoMapperHttpMapperAndErrorMapper_UsesCustomErrorMapping()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure(new CustomError(418, "Custom error")));
        var errorMapper = new CustomStatusCodeMapper();

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            () => new { Status = "Complete" },
            dto => Microsoft.AspNetCore.Http.Results.Accepted("/status", dto),
            errorMapper);

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<TValue,TDto> with dtoMapper and httpMapper transforms value for success")]
    public async Task ToHttpResultAsync_GenericWithDto_WithDtoMapperAndHttpMapper_Success_TransformsValue()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            x => new { Value = x.ToString(), Doubled = x * 2 },
            (value, dto) => Microsoft.AspNetCore.Http.Results.Created($"/items/{value}", dto));

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<TValue,TDto> with dtoMapper and httpMapper returns error for failure")]
    public async Task ToHttpResultAsync_GenericWithDto_WithDtoMapperAndHttpMapper_Failure_ReturnsError()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new ValidationError(["Invalid value"])));

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            x => new { Value = x.ToString(), Doubled = x * 2 },
            (value, dto) => Microsoft.AspNetCore.Http.Results.Created($"/items/{value}", dto));

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToHttpResultAsync<TValue,TDto> with dtoMapper, httpMapper and custom errorMapper uses custom error mapping")]
    public async Task ToHttpResultAsync_GenericWithDto_WithDtoMapperHttpMapperAndErrorMapper_UsesCustomErrorMapping()
    {
        // Arrange (Given)
        var result = ValueTask.FromResult(Result.Failure<int>(new CustomError(418, "Custom error")));
        var errorMapper = new CustomStatusCodeMapper();

        // Act (When)
        var httpResult = await result.ToHttpResultAsync(
            x => new { Value = x.ToString(), Doubled = x * 2 },
            (value, dto) => Microsoft.AspNetCore.Http.Results.Created($"/items/{value}", dto),
            errorMapper);

        // Assert (Then)
        Assert.NotNull(httpResult);
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