using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class DefaultErrorHttpMapperTests
{
    private readonly DefaultErrorHttpMapper _sut = new();

    [Fact(DisplayName = "GetResponse returns 400 for ValidationError")]
    public void GetResponse_ValidationError_Returns400()
    {
        // Given
        var error = new ValidationError(["Field is required"]);

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(400, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponse returns 404 for NotFoundError")]
    public void GetResponse_NotFoundError_Returns404()
    {
        // Given
        var error = new NotFoundError("User", "123");

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(404, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponse returns 401 for UnauthorizedError")]
    public void GetResponse_UnauthorizedError_Returns401()
    {
        // Given
        var error = new UnauthorizedError();

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(401, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponse returns 403 for UnauthenticatedError")]
    public void GetResponse_UnauthenticatedError_Returns401()
    {
        // Given
        var error = new UnauthenticatedError();

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(403, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponse returns 409 for ConflictError")]
    public void GetResponse_ConflictError_Returns409()
    {
        // Given
        var error = new ConflictError("Resource already exists");

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(409, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponse returns 500 for ExceptionalError")]
    public void GetResponse_ExceptionalError_Returns500()
    {
        // Given
        var error = new ExceptionalError(new Exception("Internal error"));

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(500, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponse returns 500 for custom Error")]
    public void GetResponse_CustomError_Returns500()
    {
        // Given
        var error = new Error("CUSTOM", "Custom error message");

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(500, response.Value.StatusCode);
    }

    [Fact(DisplayName = "GetResponseBody returns validation failures for ValidationError")]
    public void GetResponseBody_ValidationError_ReturnsFailures()
    {
        // Given
        var failures = new[] { "Field is required", "Email is invalid" };
        var error = new ValidationError(failures);

        // When
        var response = _sut.GetResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.NotNull(response.Value.Body);
        var problemDetail = Assert.IsType<ProblemDetails>(response.Value.Body);
        Assert.Equal("Validation Error", problemDetail.Title);
        Assert.Equal(400, problemDetail.Status);
        Assert.Contains("code", problemDetail.Extensions);
    }

    [Fact(DisplayName = "GetResponseBody returns resource and identifier for NotFoundError")]
    public void GetResponseBody_NotFoundError_ReturnsResourceAndIdentifier()
    {
        // Given
        var error = new NotFoundError("User", "123");

        // When
        var response = _sut.GetResponse(error);


        // Then
        Assert.NotNull(response);
        Assert.NotNull(response.Value.Body);
        var problemDetail = Assert.IsType<ProblemDetails>(response.Value.Body);
        Assert.Equal("Not Found", problemDetail.Title);
        Assert.Equal(404, problemDetail.Status);
        Assert.Equal("123", problemDetail.Extensions["identifier"]);
    }
}