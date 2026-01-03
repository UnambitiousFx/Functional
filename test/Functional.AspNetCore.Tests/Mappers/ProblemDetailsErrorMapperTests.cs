using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class ProblemDetailsErrorMapperTests
{
    [Fact(DisplayName = "GetStatusCode returns 400 for ValidationError")]
    public void GetStatusCode_ValidationError_Returns400()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new ValidationError(["Field is required"]);

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 404 for NotFoundError")]
    public void GetStatusCode_NotFoundError_Returns404()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new NotFoundError("User", "123");

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status404NotFound, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 401 for UnauthorizedError")]
    public void GetStatusCode_UnauthorizedError_Returns401()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new UnauthorizedError();

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status401Unauthorized, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 409 for ConflictError")]
    public void GetStatusCode_ConflictError_Returns409()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new ConflictError("Resource already exists");

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status409Conflict, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 500 for ExceptionalError")]
    public void GetStatusCode_ExceptionalError_Returns500()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new ExceptionalError(new Exception("Internal error"));

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCode);
    }

    [Fact(DisplayName = "GetStatusCode returns 400 for custom Error")]
    public void GetStatusCode_CustomError_Returns400()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new Error("CUSTOM", "Custom error message");

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
    }

    [Fact(DisplayName = "GetResponseBody returns ProblemDetails with correct structure")]
    public void GetResponseBody_ReturnsValidProblemDetails()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new ValidationError(["Field is required"]);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        Assert.NotNull(body);
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Validation Failed", problemDetails.Title);
        Assert.NotNull(problemDetails.Type);
        Assert.NotNull(problemDetails.Detail);
    }

    [Fact(DisplayName = "GetResponseBody includes error code in extensions")]
    public void GetResponseBody_IncludesErrorCodeInExtensions()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new ValidationError(["Field is required"]);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.True(problemDetails.Extensions.ContainsKey("code"));
        Assert.Equal("VALIDATION", problemDetails.Extensions["code"]);
    }

    [Fact(DisplayName = "GetResponseBody includes metadata in extensions")]
    public void GetResponseBody_IncludesMetadataInExtensions()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var metadata = new Dictionary<string, object?>
        {
            ["key1"] = "value1",
            ["key2"] = 42
        };
        var error = new Error("CUSTOM", "Custom message", metadata);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.True(problemDetails.Extensions.ContainsKey("key1"));
        Assert.Equal("value1", problemDetails.Extensions["key1"]);
        Assert.True(problemDetails.Extensions.ContainsKey("key2"));
        Assert.Equal(42, problemDetails.Extensions["key2"]);
    }

    [Fact(DisplayName = "GetResponseBody includes validation failures for ValidationError")]
    public void GetResponseBody_ValidationError_IncludesFailures()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var failures = new[] { "Field is required", "Email is invalid" };
        var error = new ValidationError(failures);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.True(problemDetails.Extensions.ContainsKey("failures"));
        var returnedFailures = Assert.IsAssignableFrom<IEnumerable<string>>(problemDetails.Extensions["failures"]);
        Assert.Equal(failures, returnedFailures);
    }

    [Fact(DisplayName = "GetResponseBody includes resource and identifier for NotFoundError")]
    public void GetResponseBody_NotFoundError_IncludesResourceAndIdentifier()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new NotFoundError("User", "123");

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.True(problemDetails.Extensions.ContainsKey("resource"));
        Assert.Equal("User", problemDetails.Extensions["resource"]);
        Assert.True(problemDetails.Extensions.ContainsKey("identifier"));
        Assert.Equal("123", problemDetails.Extensions["identifier"]);
    }

    [Fact(DisplayName = "GetResponseBody excludes exception details by default")]
    public void GetResponseBody_ExceptionalError_ExcludesExceptionDetailsByDefault()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper(includeExceptionDetails: false);
        var error = new ExceptionalError(new InvalidOperationException("Test exception"));

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.False(problemDetails.Extensions.ContainsKey("exception"));
    }

    [Fact(DisplayName = "GetResponseBody includes exception details when configured")]
    public void GetResponseBody_ExceptionalError_IncludesExceptionDetailsWhenConfigured()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper(includeExceptionDetails: true);
        var exception = new InvalidOperationException("Test exception");
        var error = new ExceptionalError(exception);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.True(problemDetails.Extensions.ContainsKey("exception"));
    }

    [Theory(DisplayName = "GetResponseBody sets correct title for error types")]
    [InlineData(typeof(ValidationError), "Validation Failed")]
    [InlineData(typeof(UnauthorizedError), "Unauthorized")]
    [InlineData(typeof(ConflictError), "Conflict")]
    public void GetResponseBody_SetsCorrectTitleForErrorType(Type errorType, string expectedTitle)
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = errorType switch
        {
            _ when errorType == typeof(ValidationError) => (IError)new ValidationError(["Test"]),
            _ when errorType == typeof(UnauthorizedError) => new UnauthorizedError(),
            _ when errorType == typeof(ConflictError) => new ConflictError("Test"),
            _ => throw new ArgumentException("Unsupported error type")
        };

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.Equal(expectedTitle, problemDetails.Title);
    }

    [Fact(DisplayName = "GetResponseBody sets RFC 7807 type URI for ValidationError")]
    public void GetResponseBody_ValidationError_SetsRfc7807TypeUri()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new ValidationError(["Field is required"]);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.Equal("https://tools.ietf.org/html/rfc7231#section-6.5.1", problemDetails.Type);
    }

    [Fact(DisplayName = "GetResponseBody sets RFC 7807 type URI for NotFoundError")]
    public void GetResponseBody_NotFoundError_SetsRfc7807TypeUri()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        var error = new NotFoundError("User", "123");

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.Equal("https://tools.ietf.org/html/rfc7231#section-6.5.4", problemDetails.Type);
    }

    [Fact(DisplayName = "GetResponseBody sets detail to error message")]
    public void GetResponseBody_SetsDetailToErrorMessage()
    {
        // Arrange (Given)
        var sut = new ProblemDetailsErrorMapper();
        const string expectedMessage = "This is a test error message";
        var error = new Error("TEST", expectedMessage);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        var problemDetails = Assert.IsType<ProblemDetails>(body);
        Assert.Equal(expectedMessage, problemDetails.Detail);
    }
}
