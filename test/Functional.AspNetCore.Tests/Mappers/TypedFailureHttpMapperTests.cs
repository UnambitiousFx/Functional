using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class TypedErrorHttpMapperTests
{
    [Fact(DisplayName = "GetFailureResponse returns response when failure matches type")]
    public void GetErrorResponse_MatchingFailureType_ReturnsResponse()
    {
        // Arrange (Given)
        var failure = new ValidationFailure(["Field is required"]);
        var sut     = new TypedFailureHttpMapper<ValidationFailure>(_ => new FailureHttpResponse { StatusCode = 400 });

        // Act (When)
        var response = sut.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode);
    }

    [Fact(DisplayName = "GetFailureResponse returns null when failure does not match type")]
    public void GetErrorResponse_NonMatchingFailureType_ReturnsNull()
    {
        // Arrange (Given)
        var failure = new NotFoundFailure("User", "123");
        var sut     = new TypedFailureHttpMapper<ValidationFailure>(_ => new FailureHttpResponse { StatusCode = 400 });

        // Act (When)
        var response = sut.GetFailureResponse(failure);

        // Assert (Then)
        Assert.Null(response);
    }

    [Fact(DisplayName = "GetFailureResponse passes typed failure to factory")]
    public void GetErrorResponse_MatchingType_PassesTypedFailureToFactory()
    {
        // Arrange (Given)
        ValidationFailure? captured = null;
        var                failure  = new ValidationFailure(["Invalid email"]);
        var sut = new TypedFailureHttpMapper<ValidationFailure>(f =>
        {
            captured = f;
            return new FailureHttpResponse { StatusCode = 422 };
        });

        // Act (When)
        sut.GetFailureResponse(failure);

        // Assert (Then)
        Assert.Same(failure, captured);
    }

    [Fact(DisplayName = "GetFailureResponse works with subtype of registered failure type")]
    public void GetErrorResponse_SubtypeOfRegisteredType_ReturnsResponse()
    {
        // Arrange (Given)
        var failure = new NotFoundFailure("Order", "42");
        // NotFoundFailure extends Failure which extends IFailure
        var sut = new TypedFailureHttpMapper<NotFoundFailure>(f => new FailureHttpResponse { StatusCode = 404 });

        // Act (When)
        var response = sut.GetFailureResponse(failure);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(404, response.StatusCode);
    }

    [Fact(DisplayName = "GetFailureResponse forwards null factory return as null")]
    public void GetErrorResponse_FactoryReturnsNull_PropagatesNull()
    {
        // Arrange (Given)
        var failure = new ConflictFailure("Already exists");
        var sut     = new TypedFailureHttpMapper<ConflictFailure>(_ => null);

        // Act (When)
        var response = sut.GetFailureResponse(failure);

        // Assert (Then)
        Assert.Null(response);
    }
}
