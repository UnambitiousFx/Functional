using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class ValidationStatusTests
{
    private static readonly ValidationFailure Validation = new([new ValidationError("amount", "Must be positive.")]);

    [Fact]
    public void GetFailureResponse_ValidationFailureByDefault_Returns422WithRfc9110Type()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().BuildMapper();

        // Act (When)
        var response = mapper.GetFailureResponse(Validation)!;

        // Assert (Then)
        var problem = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, response.StatusCode);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, problem.Status);
        Assert.Equal("https://www.rfc-editor.org/rfc/rfc9110#section-15.5.21", problem.Type);
        Assert.Contains("errors", problem.Extensions);
    }

    [Fact]
    public void ValidationStatus_400_ReturnsBadRequestWithValidationBody()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().ValidationStatus(StatusCodes.Status400BadRequest).BuildMapper();

        // Act (When)
        var response = mapper.GetFailureResponse(Validation)!;

        // Assert (Then)
        var problem = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Equal("Validation Error",                                     problem.Title);
        Assert.Equal("https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1", problem.Type);
        Assert.Contains("errors", problem.Extensions);
    }

    [Fact]
    public void ValidationStatus_ValidationFailureWithDomainCode_AppliesStatusAndKeepsCode()
    {
        // Arrange (Given)
        var mapper  = new ResultHttpOptions().ValidationStatus(StatusCodes.Status400BadRequest).BuildMapper();
        var failure = new ValidationFailure([new ValidationError("amount", "Must be positive.")]) { Code = "orders.amount_invalid" };

        // Act (When)
        var response = mapper.GetFailureResponse(failure)!;

        // Assert (Then)
        var problem = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
        Assert.Equal("orders.amount_invalid", problem.Extensions["code"]);
        Assert.Contains("errors", problem.Extensions);
    }

    [Theory]
    [InlineData(StatusCodes.Status302Found)]
    [InlineData(StatusCodes.Status402PaymentRequired)]
    [InlineData(StatusCodes.Status418ImATeapot)]
    [InlineData(StatusCodes.Status429TooManyRequests)]
    [InlineData(StatusCodes.Status503ServiceUnavailable)]
    public void ValidationStatus_StatusWithoutKnownType_OmitsType(int statusCode)
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().ValidationStatus(statusCode).BuildMapper();

        // Act (When)
        var response = mapper.GetFailureResponse(Validation)!;

        // Assert (Then)
        var problem = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal(statusCode, response.StatusCode);
        Assert.Null(problem.Type);
    }

    [Fact]
    public void ValidationStatus_MapCodeForSameCode_MapCodeWins()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions()
                     .ValidationStatus(StatusCodes.Status400BadRequest)
                     .MapCode("orders.period_closed", StatusCodes.Status409Conflict)
                     .BuildMapper();
        var failure = new ValidationFailure([new ValidationError(null, "The period is closed.")]) { Code = "orders.period_closed" };

        // Act (When)
        var response = mapper.GetFailureResponse(failure)!;

        // Assert (Then)
        var problem = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal(StatusCodes.Status409Conflict, response.StatusCode);
        Assert.Contains("errors", problem.Extensions);
    }

    [Fact]
    public void ValidationStatus_BadRequestFailure_StaysBadRequest()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().ValidationStatus(StatusCodes.Status422UnprocessableEntity).BuildMapper();

        // Act (When)
        var response = mapper.GetFailureResponse(new BadRequestFailure("Unreadable body."))!;

        // Assert (Then)
        Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(600)]
    public void ValidationStatus_OutOfRange_Throws(int statusCode)
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act & Assert (When/Then)
        Assert.Throws<ArgumentOutOfRangeException>(() => options.ValidationStatus(statusCode));
    }

    [Fact]
    public void ValidationStatus_ReturnsSameInstance()
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act (When)
        var result = options.ValidationStatus(StatusCodes.Status400BadRequest);

        // Assert (Then)
        Assert.Same(options, result);
    }

    public static TheoryData<IFailure, string> BuiltInTypes() => new()
    {
        { new BadRequestFailure("Unreadable body."), "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.1" },
        { new UnauthenticatedFailure("Sign in."),    "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.2" },
        { new UnauthorizedFailure("Admins only."),   "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.4" },
        { new NotFoundFailure("User", "42"),         "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.5" },
        { new ConflictFailure("Taken."),             "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.10" },
        { Validation,                                "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.21" },
        { new ExceptionalFailure(new InvalidOperationException("Boom")), "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.1" },
        { new Failure("Unmapped."),                  "https://www.rfc-editor.org/rfc/rfc9110#section-15.6.1" }
    };

    [Theory]
    [MemberData(nameof(BuiltInTypes))]
    public void GetFailureResponse_BuiltInFailure_CitesRfc9110(IFailure failure,
                                                               string   expectedType)
    {
        // Arrange (Given)
        var mapper = new DefaultFailureHttpMapper();

        // Act (When)
        var response = mapper.GetFailureResponse(failure);

        // Assert (Then)
        var problem = Assert.IsType<ProblemDetails>(response.Body);
        Assert.Equal(expectedType, problem.Type);
    }
}
