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

    [Fact]
    public async Task ValueTaskResult_AsActionResultBuilder_WithSuccessResult_ReturnsNoContentStatusCode()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success());

        // Act (When)
        var actionResult = await resultTask.AsActionResultBuilder();

        // Assert (Then)
        Assert.Equal(StatusCodes.Status204NoContent, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithHeader_CopiesExistingHeadersWhenChaining()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithHeader("X-First", "one")
                                       .WithHeader("X-Second", "two");
        var headers = GetWrappedHeaders(actionResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("one", headers!["X-First"]);
        Assert.Equal("two", headers["X-Second"]);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailure_ReturnsUnauthorizedStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("unauthorized"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(new FixedBodyStatusMapper(401));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status401Unauthorized, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailure_ReturnsForbiddenStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("forbidden"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(new FixedBodyStatusMapper(403));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status403Forbidden, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailure_ReturnsConflictStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("conflict"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(new FixedBodyStatusMapper(409));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status409Conflict, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailure_ReturnsInternalServerErrorStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("server error"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(new FixedBodyStatusMapper(500));

        // Assert (Then)
        Assert.Equal(StatusCodes.Status500InternalServerError, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailure_ReturnsDefaultCustomStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("teapot"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(new FixedBodyStatusMapper(418));

        // Assert (Then)
        Assert.Equal(418, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_GenericResult_WithCreatedStatusCodeAndNoLocationFactory_ReturnsCreatedStatusCode()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithStatusCode(StatusCodes.Status201Created);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status201Created, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_GenericResult_WithAcceptedStatusCode_ReturnsAcceptedStatusCode()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithStatusCode(StatusCodes.Status202Accepted);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status202Accepted, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_GenericResult_WithDefaultCustomStatusCode_ReturnsCustomStatusCode()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithStatusCode(418);

        // Assert (Then)
        Assert.Equal(418, GetStatusCode(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_GenericResult_WithHeader_CopiesExistingHeadersWhenChaining()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithHeader("X-First", "one")
                                       .WithHeader("X-Second", "two");
        var headers = GetWrappedHeaders(actionResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("one", headers!["X-First"]);
        Assert.Equal("two", headers["X-Second"]);
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

    private sealed class FixedBodyStatusMapper(int statusCode) : IFailureHttpMapper
    {
        public FailureHttpResponse? GetFailureResponse(IFailure failure)
        {
            return new FailureHttpResponse
            {
                StatusCode = statusCode,
                Body       = new { Error = "error" }
            };
        }
    }
}
