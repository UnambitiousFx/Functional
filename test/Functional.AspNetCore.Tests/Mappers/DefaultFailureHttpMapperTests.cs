using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class DefaultFailureHttpMapperTests
{
    private readonly DefaultFailureHttpMapper _sut = new();

    [Theory]
    [InlineData(typeof(ValidationFailure),      400)]
    [InlineData(typeof(NotFoundFailure),         404)]
    [InlineData(typeof(UnauthorizedFailure),     401)]
    [InlineData(typeof(UnauthenticatedFailure),  403)]
    [InlineData(typeof(ConflictFailure),         409)]
    [InlineData(typeof(ExceptionalFailure),      500)]
    [InlineData(typeof(Failure),                 500)]
    public void GetFailureResponse_WithFailureType_ReturnsExpectedStatusCode(Type failureType, int expectedStatusCode)
    {
        // Arrange (Given)
        var error = CreateFailure(failureType);

        // Act (When)
        var response = _sut.GetFailureResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }

    [Fact]
    public void GetFailureResponse_ValidationError_ReturnsValidationProblemDetails()
    {
        // Arrange (Given)
        var failures = new[] { "Field is required", "Email is invalid" };
        var error    = new ValidationFailure(failures);

        // Act (When)
        var response = _sut.GetFailureResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.NotNull(response.Body);
        var problemDetail = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal("Validation Error", problemDetail.Title);
        Assert.Equal(400,                problemDetail.Status);
        Assert.Contains("code", problemDetail.Extensions);
    }

    [Fact]
    public void GetFailureResponse_NotFoundError_ReturnsResourceAndIdentifier()
    {
        // Arrange (Given)
        var error = new NotFoundFailure("User", "123");

        // Act (When)
        var response = _sut.GetFailureResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.NotNull(response.Body);
        var problemDetail = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal("Not Found", problemDetail.Title);
        Assert.Equal(404,         problemDetail.Status);
        Assert.Equal("123",       problemDetail.Extensions["identifier"]);
    }

    private static IFailure CreateFailure(Type failureType)
    {
        if (failureType == typeof(ValidationFailure)) {
            return new ValidationFailure(["Field is required"]);
        }

        if (failureType == typeof(NotFoundFailure)) {
            return new NotFoundFailure("User", "123");
        }

        if (failureType == typeof(UnauthorizedFailure)) {
            return new UnauthorizedFailure();
        }

        if (failureType == typeof(UnauthenticatedFailure)) {
            return new UnauthenticatedFailure();
        }

        if (failureType == typeof(ConflictFailure)) {
            return new ConflictFailure("Resource already exists");
        }

        if (failureType == typeof(ExceptionalFailure)) {
            return new ExceptionalFailure(new Exception("Internal error"));
        }

        return new Failure("CUSTOM", "Custom error message");
    }
}
