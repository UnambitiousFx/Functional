using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mvc;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Extensions.Mvc;

public class MaybeMvcBuilderTests {
    [Fact]
    public async Task AsActionResultBuilder_WithSomeMaybe_ReturnsOkObjectResultWithValue() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder();

        // Assert (Then)
        var okObjectResult = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(42, okObjectResult.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithNoneMaybe_ReturnsNotFoundStatusCode() {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder();

        // Assert (Then)
        Assert.IsType<NotFoundResult>(UnwrapWrappedResult(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithSomeMaybeAndFormatter_ReturnsFormattedOkObjectResult() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .WithResponseFormatter(v => new { Value = v.ToString() });

        // Assert (Then)
        var okObjectResult = Assert.IsType<OkObjectResult>(UnwrapWrappedResult(actionResult));
        Assert.NotNull(okObjectResult.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithSomeMaybeAsCreated_ReturnsCreatedResultWithLocationAndValue() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .AsCreated(v => $"/items/{v}");

        // Assert (Then)
        var createdResult = Assert.IsType<CreatedResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal("/items/42", createdResult.Location);
        Assert.Equal(42,          createdResult.Value);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithNoneMapper_ReturnsCustomNoContentResult() {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .AsNone(() => new NoContentResult());

        // Assert (Then)
        Assert.IsType<NoContentResult>(UnwrapWrappedResult(actionResult));
    }

    [Fact]
    public async Task AsActionResultBuilder_WithHeader_StoresHeaderMetadata() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .WithHeader("X-Trace-Id", "abc");
        var headers = GetWrappedHeaders(actionResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("abc", headers!["X-Trace-Id"]);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithHeader_CopiesExistingHeadersWhenChaining() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .WithHeader("X-First", "one")
                                      .WithHeader("X-Second", "two");
        var headers = GetWrappedHeaders(actionResult);

        // Assert (Then)
        Assert.NotNull(headers);
        Assert.Equal("one", headers!["X-First"]);
        Assert.Equal("two", headers["X-Second"]);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithStatusCode_OverridesDefaultStatusCode() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .WithStatusCode(202);

        // Assert (Then)
        var accepted = Assert.IsType<AcceptedResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(202, accepted.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithCreatedStatusCodeAndNoLocationFactory_ReturnsCreatedResult() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .WithStatusCode(201);

        // Assert (Then)
        var created = Assert.IsType<CreatedResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(201, created.StatusCode);
    }

    [Fact]
    public async Task AsActionResultBuilder_WithDefaultCustomStatusCode_ReturnsStatusCodeResult() {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var actionResult = await maybe.AsActionResultBuilder()
                                      .WithStatusCode(418);

        // Assert (Then)
        var statusResult = Assert.IsType<StatusCodeResult>(UnwrapWrappedResult(actionResult));
        Assert.Equal(418, statusResult.StatusCode);
    }

    private static IActionResult UnwrapWrappedResult(IActionResult result) {
        var innerField = result.GetType()
                               .GetField("_inner", BindingFlags.NonPublic | BindingFlags.Instance);
        return innerField?.GetValue(result) as IActionResult ?? result;
    }

    private static IReadOnlyDictionary<string, string>? GetWrappedHeaders(IActionResult result) {
        var headersField = result.GetType()
                                 .GetField("_headers", BindingFlags.NonPublic | BindingFlags.Instance);
        return headersField?.GetValue(result) as IReadOnlyDictionary<string, string>;
    }
}
