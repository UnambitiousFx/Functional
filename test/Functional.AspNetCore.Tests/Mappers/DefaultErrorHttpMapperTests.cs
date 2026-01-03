using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class DefaultErrorHttpMapperTests
{
    private readonly DefaultErrorHttpMapper _sut = new();

    [Fact(DisplayName = "GetStatusCode returns 400 for ValidationError")]
    public void GetStatusCode_ValidationError_Returns400()
    {
        // Given
        var error = new ValidationError(["Field is required"]);

        // When
        var statusCode = _sut.GetStatusCode(error);

        // Then
        Assert.Equal(400, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 404 for NotFoundError")]
    public void GetStatusCode_NotFoundError_Returns404()
    {
        // Given
        var error = new NotFoundError("User", "123");

        // When
        var statusCode = _sut.GetStatusCode(error);

        // Then
        Assert.Equal(404, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 401 for UnauthorizedError")]
    public void GetStatusCode_UnauthorizedError_Returns401()
    {
        // Given
        var error = new UnauthorizedError();

        // When
        var statusCode = _sut.GetStatusCode(error);

        // Then
        Assert.Equal(401, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 409 for ConflictError")]
    public void GetStatusCode_ConflictError_Returns409()
    {
        // Given
        var error = new ConflictError("Resource already exists");

        // When
        var statusCode = _sut.GetStatusCode(error);

        // Then
        Assert.Equal(409, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 500 for ExceptionalError")]
    public void GetStatusCode_ExceptionalError_Returns500()
    {
        // Given
        var error = new ExceptionalError(new Exception("Internal error"));

        // When
        var statusCode = _sut.GetStatusCode(error);

        // Then
        Assert.Equal(500, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 400 for custom Error")]
    public void GetStatusCode_CustomError_Returns400()
    {
        // Given
        var error = new Error("CUSTOM", "Custom error message");

        // When
        var statusCode = _sut.GetStatusCode(error);

        // Then
        Assert.Equal(400, statusCode);
    }

    [Fact(DisplayName = "GetResponseBody returns validation failures for ValidationError")]
    public void GetResponseBody_ValidationError_ReturnsFailures()
    {
        // Given
        var failures = new[] { "Field is required", "Email is invalid" };
        var error = new ValidationError(failures);

        // When
        var body = _sut.GetResponseBody(error);

        // Then
        Assert.NotNull(body);
        var anonymous = body as dynamic;
        Assert.NotNull(anonymous);
        Assert.Equal("VALIDATION", anonymous!.error);
        Assert.NotNull(anonymous.failures);
    }

    [Fact(DisplayName = "GetResponseBody returns resource and identifier for NotFoundError")]
    public void GetResponseBody_NotFoundError_ReturnsResourceAndIdentifier()
    {
        // Given
        var error = new NotFoundError("User", "123");

        // When
        var body = _sut.GetResponseBody(error);

        // Then
        Assert.NotNull(body);
        var anonymous = body as dynamic;
        Assert.NotNull(anonymous);
        Assert.Equal("NOT_FOUND", anonymous!.error);
        Assert.Equal("User", anonymous.resource);
        Assert.Equal("123", anonymous.identifier);
    }
}
