using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.AspNetCore.Mvc;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc;

public class ResultMvcBuilderTests
{
    [Fact]
    public async Task AsActionResultBuilder_WithSuccessResult_ReturnsNoContentStatusCode()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act (When)
        var actionResult = await result.AsActionResultBuilder();

        // Assert (Then)
        Assert.IsType<NoContentResult>(UnwrapWrappedResult(actionResult));
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
        var statusCodeResult = Assert.IsType<StatusCodeResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(StatusCodes.Status200OK, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithFailureResult_ReturnsObjectResultWithStatusCode404()
    {
        // Arrange (Given)
        var result = Result.Failure(new NotFoundFailure("Item", "42"));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder();

        // Assert (Then)
        var objectResult = Assert.IsType<ObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithGenericSuccessAndFormatter_ReturnsOkObjectResult()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .WithResponseFormatter(v => new { Value = v.ToString() });

        // Assert (Then)
        var okObjectResult = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.NotNull(okObjectResult.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithGenericSuccessAsCreated_ReturnsCreatedResultWithLocationAndValue()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var actionResult = await result.AsActionResultBuilder()
                                       .AsCreated(v => $"/items/{v}");

        // Assert (Then)
        var createdResult = Assert.IsType<CreatedResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal("/items/42", createdResult.Location);
        Assert.Equal(42, createdResult.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithCustomFailureMapperAndHeader_StoresHeaderMetadata()
    {
        // Arrange (Given)
        var mapper = new HeaderFailureMapper();
        var result = Result.Failure(new ExceptionalFailure(new Exception("boom")));

        // Act (When)
        var actionResult = await result.AsActionResultBuilder(mapper);
        var headers = GetWrappedHeaders(actionResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("abc", headers!["X-Trace-Id"]);
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

    private sealed class HeaderFailureMapper : IFailureHttpMapper
    {
        public FailureHttpResponse? GetFailureResponse(IFailure failure)
        {
            return new FailureHttpResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Body = new { Message = "bad" },
                Headers = new Dictionary<string, string>
                {
                    ["X-Trace-Id"] = "abc"
                }
            };
        }
    }
}
