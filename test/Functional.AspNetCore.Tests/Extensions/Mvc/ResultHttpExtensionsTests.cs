using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc;

public class ResultHttpExtensionsTests
{
    [Fact]
    public async Task AsActionResultBuilder_WithSuccessResult_ReturnsNoContentStatusCode()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var actionResult = await result.AsActionResultBuilder();

        // Assert (Then)
        Assert.Equal(StatusCodes.Status204NoContent, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithSuccessResultAndStatusCode200_ReturnsStatusCode200()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithStatusCode(StatusCodes.Status200OK);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status200OK, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailureResult_ReturnsMappedStatusCode404()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundFailure("Item", "42"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder();

        // Assert (Then)
        Assert.Equal(StatusCodes.Status404NotFound, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithGenericSuccessResult_ReturnsOkObjectResultWithValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder();

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFormatter_FormatsResponseBodyValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithResponseFormatter(v => new ResponseDto(v.ToString()));

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        var dto = Assert.IsType<ResponseDto>(ok.Value);
        Assert.Equal("42", dto.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_AsCreated_WithGenericSuccessResult_ReturnsCreatedResultWithLocationAndValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .AsCreated(v => $"/items/{v}");

        // Assert (Then)
        var created = Assert.IsType<CreatedResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal("/items/42", created.Location);
        Assert.Equal(42, created.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithCustomMapper_AppliesMappedStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure<int>(new CustomFailure(418, "Teapot"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(new CustomStatusCodeMapper());

        // Assert (Then)
        Assert.Equal(418, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithHeader_StoresHeaderMetadata()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithHeader("X-Trace-Id", "abc");
        var headers = GetWrappedHeaders(actionResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("abc", headers!["X-Trace-Id"]);
    }

    [Fact]
    public async Task ValueTaskResult_AsActionResultBuilder_WithGenericSuccessResult_ReturnsOkObjectResultWithValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var actionResult = await resultTask.AsActionResultBuilder();

        // Assert (Then)
        var ok = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(42, ok.Value);
    }

    private static int GetStatusCode(IActionResult result)
    {
        var inner = UnwrapWrappedResult(result);

        return inner switch
        {
            ObjectResult { StatusCode: { } statusCode } => statusCode,
            StatusCodeResult statusCodeResult => statusCodeResult.StatusCode,
            ForbidResult => StatusCodes.Status403Forbidden,
            _ => throw new InvalidOperationException($"Unable to resolve status code for {inner.GetType().Name}")
        };
    }

    private static IActionResult UnwrapWrappedResult(IActionResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as IActionResult ?? result;
    }

    private static IReadOnlyDictionary<string, string>? GetWrappedHeaders(IActionResult result)
    {
        var headersField = result.GetType().GetField("_headers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return headersField?.GetValue(result) as IReadOnlyDictionary<string, string>;
    }

    private sealed record ResponseDto(string Value);

    private sealed record CustomFailure(int StatusCode, string Message) : Failure(Message);

    private sealed class CustomStatusCodeMapper : IFailureHttpMapper
    {
        public FailureHttpResponse? GetFailureResponse(IFailure failure)
        {
            if (failure is not CustomFailure customFailure)
            {
                return null;
            }

            return new FailureHttpResponse
            {
                StatusCode = customFailure.StatusCode,
                Body = new ProblemDetails { Status = customFailure.StatusCode, Detail = customFailure.Message }
            };
        }
    }
}
