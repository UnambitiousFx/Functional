using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;
using IHttpResult = Microsoft.AspNetCore.Http.IResult;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Http;

public class ResultHttpExtensionsTests
{
    [Fact]
    public async Task AsHttpBuilder_WithSuccessResult_ReturnsNoContentStatusCode()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var httpResult = await result.AsHttpBuilder();

        // Assert (Then)
        Assert.Equal(StatusCodes.Status204NoContent, GetStatusCode(httpResult));
    }

    [Fact]
    public async Task AsHttpBuilder_WithSuccessResultAndStatusCode200_ReturnsStatusCode200()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var httpResult = await result.AsHttpBuilder()
                                     .WithStatusCode(StatusCodes.Status200OK);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status200OK, GetStatusCode(httpResult));
    }

    [Fact]
    public async Task AsHttpBuilder_WithFailureResult_ReturnsMappedStatusCode400()
    {
        // Arrange (Given)
        var result = Result.Failure(new ValidationFailure(["Invalid input"]));

        // Act (When)
        var httpResult = await result.AsHttpBuilder();

        // Assert (Then)
        Assert.Equal(StatusCodes.Status400BadRequest, GetStatusCode(httpResult));
    }

    [Fact]
    public async Task AsHttpBuilder_WithGenericSuccessResult_ReturnsOkWithOriginalValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var httpResult = await result.AsHttpBuilder();

        // Assert (Then)
        var ok = Assert.IsType<Ok<int>>(UnwrapWrappedResult(httpResult));
        Assert.Equal(42, ok.Value);
    }

    [Fact]
    public async Task AsHttpBuilder_WithFormatter_FormatsResponseBodyValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var httpResult = await result.AsHttpBuilder()
                                     .WithResponseFormatter(v => new ResponseDto(v.ToString()));

        // Assert (Then)
        var ok = Assert.IsType<Ok<ResponseDto>>(UnwrapWrappedResult(httpResult));
        Assert.NotNull(ok.Value);
        Assert.Equal("42", ok.Value!.Value);
    }

    [Fact]
    public async Task AsHttpBuilder_AsCreated_WithGenericSuccessResult_ReturnsCreatedWithLocationAndValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var httpResult = await result.AsHttpBuilder()
                                     .AsCreated(v => $"/items/{v}");

        // Assert (Then)
        var created = Assert.IsType<Created<int>>(UnwrapWrappedResult(httpResult));
        Assert.Equal("/items/42", created.Location);
        Assert.Equal(42, created.Value);
    }

    [Fact]
    public async Task AsHttpBuilder_WithCustomMapper_AppliesMappedStatusCode()
    {
        // Arrange (Given)
        var result = Result.Failure<int>(new CustomFailure(418, "Teapot"));

        // Act (When)
        var httpResult = await result.AsHttpBuilder(new CustomStatusCodeMapper());

        // Assert (Then)
        Assert.Equal(418, GetStatusCode(httpResult));
    }

    [Fact]
    public async Task AsHttpBuilder_WithHeader_StoresHeaderMetadata()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var httpResult = await result.AsHttpBuilder()
                                     .WithHeader("X-Trace-Id", "abc");
        var headers = GetWrappedHeaders(httpResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("abc", headers!["X-Trace-Id"]);
    }

    [Fact]
    public async Task ValueTaskResult_AsHttpBuilder_WithGenericSuccessResult_ReturnsOkWithOriginalValue()
    {
        // Arrange (Given)
        var resultTask = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var httpResult = await resultTask.AsHttpBuilder();

        // Assert (Then)
        var ok = Assert.IsType<Ok<int>>(UnwrapWrappedResult(httpResult));
        Assert.Equal(42, ok.Value);
    }

    private static int GetStatusCode(IHttpResult result)
    {
        var inner = UnwrapWrappedResult(result);

        return inner switch
        {
            IStatusCodeHttpResult { StatusCode: { } statusCode } => statusCode,
            ForbidHttpResult _ => StatusCodes.Status403Forbidden,
            _ => throw new InvalidOperationException($"Unable to resolve status code for {inner.GetType().Name}")
        };
    }

    private static IHttpResult UnwrapWrappedResult(IHttpResult result)
    {
        var innerField = result.GetType().GetField("_inner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return innerField?.GetValue(result) as IHttpResult ?? result;
    }

    private static IReadOnlyDictionary<string, string>? GetWrappedHeaders(IHttpResult result)
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
