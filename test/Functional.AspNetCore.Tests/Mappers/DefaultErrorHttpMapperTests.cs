using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class DefaultErrorHttpMapperTests
{
    private readonly DefaultErrorHttpMapper _sut = new();

    [Fact(DisplayName = "GetErrorResponse returns 400 for ValidationError")]
    public void GetErrorResponse_ValidationError_Returns400()
    {
        // Given
        var error = new ValidationFailure(["Field is required"]);

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns 404 for NotFoundError")]
    public void GetErrorResponse_NotFoundError_Returns404()
    {
        // Given
        var error = new NotFoundFailure("User", "123");

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(404, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns 401 for UnauthorizedError")]
    public void GetErrorResponse_UnauthorizedError_Returns401()
    {
        // Given
        var error = new UnauthorizedFailure();

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(401, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns 403 for UnauthenticatedError")]
    public void GetErrorResponse_UnauthenticatedError_Returns401()
    {
        // Given
        var error = new UnauthenticatedFailure();

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(403, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns 409 for ConflictError")]
    public void GetErrorResponse_ConflictError_Returns409()
    {
        // Given
        var error = new ConflictFailure("Resource already exists");

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(409, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns 500 for ExceptionalError")]
    public void GetErrorResponse_ExceptionalError_Returns500()
    {
        // Given
        var error = new ExceptionalFailure(new Exception("Internal error"));

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(500, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns 500 for custom Error")]
    public void GetErrorResponse_CustomError_Returns500()
    {
        // Given
        var error = new Failure("CUSTOM", "Custom error message");

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.Equal(500, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns validation failures for ValidationError")]
    public void GetErrorResponse_ValidationError_ReturnsFailures()
    {
        // Given
        var failures = new[] { "Field is required", "Email is invalid" };
        var error = new ValidationFailure(failures);

        // When
        var response = _sut.GetErrorResponse(error);

        // Then
        Assert.NotNull(response);
        Assert.NotNull(response.Body);
        var problemDetail = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal("Validation Error", problemDetail.Title);
        Assert.Equal(400, problemDetail.Status);
        Assert.Contains("code", problemDetail.Extensions);
    }

    [Fact(DisplayName = "GetErrorResponse returns resource and identifier for NotFoundError")]
    public void GetErrorResponse_NotFoundError_ReturnsResourceAndIdentifier()
    {
        // Given
        var error = new NotFoundFailure("User", "123");

        // When
        var response = _sut.GetErrorResponse(error);


        // Then
        Assert.NotNull(response);
        Assert.NotNull(response.Body);
        var problemDetail = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal("Not Found", problemDetail.Title);
        Assert.Equal(404, problemDetail.Status);
        Assert.Equal("123", problemDetail.Extensions["identifier"]);
    }
}