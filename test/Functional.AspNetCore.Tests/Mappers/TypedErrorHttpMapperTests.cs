using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class TypedErrorHttpMapperTests
{
    [Fact(DisplayName = "GetErrorResponse returns response when failure matches type")]
    public void GetErrorResponse_MatchingFailureType_ReturnsResponse()
    {
        // Arrange (Given)
        var failure = new ValidationFailure(["Field is required"]);
        var sut = new TypedErrorHttpMapper<ValidationFailure>(
            _ => new ErrorHttpResponse { StatusCode = 400 });

        // Act (When)
        var response = sut.GetErrorResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns null when failure does not match type")]
    public void GetErrorResponse_NonMatchingFailureType_ReturnsNull()
    {
        // Arrange (Given)
        var failure = new NotFoundFailure("User", "123");
        var sut = new TypedErrorHttpMapper<ValidationFailure>(
            _ => new ErrorHttpResponse { StatusCode = 400 });

        // Act (When)
        var response = sut.GetErrorResponse(failure);

        // Assert (Then)
        Assert.Null(response);
    }

    [Fact(DisplayName = "GetErrorResponse passes typed failure to factory")]
    public void GetErrorResponse_MatchingType_PassesTypedFailureToFactory()
    {
        // Arrange (Given)
        ValidationFailure? captured = null;
        var failure = new ValidationFailure(["Invalid email"]);
        var sut = new TypedErrorHttpMapper<ValidationFailure>(f =>
        {
            captured = f;
            return new ErrorHttpResponse { StatusCode = 422 };
        });

        // Act (When)
        sut.GetErrorResponse(failure);

        // Assert (Then)
        Assert.Same(failure, captured);
    }

    [Fact(DisplayName = "GetErrorResponse works with subtype of registered failure type")]
    public void GetErrorResponse_SubtypeOfRegisteredType_ReturnsResponse()
    {
        // Arrange (Given)
        var failure = new NotFoundFailure("Order", "42");
        // NotFoundFailure extends Failure which extends IFailure
        var sut = new TypedErrorHttpMapper<NotFoundFailure>(
            f => new ErrorHttpResponse { StatusCode = 404 });

        // Act (When)
        var response = sut.GetErrorResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(404, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse forwards null factory return as null")]
    public void GetErrorResponse_FactoryReturnsNull_PropagatesNull()
    {
        // Arrange (Given)
        var failure = new ConflictFailure("Already exists");
        var sut = new TypedErrorHttpMapper<ConflictFailure>(_ => null);

        // Act (When)
        var response = sut.GetErrorResponse(failure);

        // Assert (Then)
        Assert.Null(response);
    }
}
