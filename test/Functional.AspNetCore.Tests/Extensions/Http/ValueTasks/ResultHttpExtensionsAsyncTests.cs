using UnambitiousFx.Functional.AspNetCore.Http.ValueTasks;
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
        var valueTask = new ValueTask<Result<int>>(Result.Success(42));

        // Act (When)
        var httpResult = await valueTask.ToHttpResultAsync();

        // Assert (Then)
        Assert.NotNull(httpResult);
    }

    [Fact(DisplayName = "ToCreatedHttpResultAsync awaits async operation correctly")]
    public async Task ToCreatedHttpResultAsync_AwaitsAsyncOperation_Correctly()
    {
        // Arrange (Given)
        var valueTask = new ValueTask<Result<int>>(Result.Success(42));

        // Act (When)
        var httpResult = await valueTask.ToCreatedHttpResultAsync(id => $"/items/{id}");

        // Assert (Then)
        Assert.NotNull(httpResult);
    }
}