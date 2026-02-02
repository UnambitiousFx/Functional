using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class CompositeErrorHttpMapperTests
{
    [Fact(DisplayName = "GetErrorResponse returns status from first mapper that provides one")]
    public void GetErrorResponse_FirstMapperReturnsStatus_ReturnsFirstStatus()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetErrorResponse(error).Returns(new ErrorHttpResponse { StatusCode = 400 });

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetErrorResponse(error).Returns(new ErrorHttpResponse { StatusCode = 500 });

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetErrorResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode);
        mapper1.Received(1).GetErrorResponse(error);
        mapper2.DidNotReceive().GetErrorResponse(Arg.Any<IFailure>());
    }

    [Fact(DisplayName = "GetErrorResponse tries next mapper when first returns null")]
    public void GetErrorResponse_FirstMapperReturnsNull_TriesNextMapper()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetErrorResponse(error).Returns((ErrorHttpResponse?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetErrorResponse(error).Returns(new ErrorHttpResponse { StatusCode = 400 });

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetErrorResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.StatusCode);
    }

    [Fact(DisplayName = "GetErrorResponse returns null when all mappers return null")]
    public void GetErrorResponse_AllMappersReturnNull_ReturnsNull()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetErrorResponse(error).Returns((ErrorHttpResponse?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetErrorResponse(error).Returns((ErrorHttpResponse?)null);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetErrorResponse(error);

        // Assert (Then)
        Assert.Null(response);
    }

    [Fact(DisplayName = "GetErrorResponse returns body from first mapper that provides one")]
    public void GetErrorResponse_FirstMapperReturnsBody_ReturnsFirstBody()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var expectedBody = new { error = "validation_error" };

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetErrorResponse(error).Returns(new ErrorHttpResponse { StatusCode = 400, Body = expectedBody });

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetErrorResponse(error).Returns(new ErrorHttpResponse { StatusCode = 400, Body = new { error = "other_error" } });

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetErrorResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Same(expectedBody, response.Body);
    }

    [Fact(DisplayName = "GetErrorResponse tries next mapper when first returns null for body")]
    public void GetErrorResponse_FirstMapperReturnsNull_TriesNextMapper_ForBody()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var expectedBody = new { error = "validation_error" };

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetErrorResponse(error).Returns((ErrorHttpResponse?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetErrorResponse(error).Returns(new ErrorHttpResponse { StatusCode = 400, Body = expectedBody });

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetErrorResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Same(expectedBody, response.Body);
    }

    [Fact(DisplayName = "GetErrorResponse returns null when all mappers return null for body")]
    public void GetErrorResponse_AllMappersReturnNull_ReturnsNull_ForBody()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetErrorResponse(error).Returns((ErrorHttpResponse?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetErrorResponse(error).Returns((ErrorHttpResponse?)null);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetErrorResponse(error);

        // Assert (Then)
        Assert.Null(response);
    }

    [Fact(DisplayName = "Constructor with params array creates mapper correctly")]
    public void Constructor_WithParamsArray_CreatesMapperCorrectly()
    {
        // Arrange (Given)
        var mapper1 = Substitute.For<IErrorHttpMapper>();
        var mapper2 = Substitute.For<IErrorHttpMapper>();

        // Act (When)
        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Assert (Then)
        Assert.NotNull(sut);
    }

    [Fact(DisplayName = "Constructor with IReadOnlyList creates mapper correctly")]
    public void Constructor_WithReadOnlyList_CreatesMapperCorrectly()
    {
        // Arrange (Given)
        var mapper1 = Substitute.For<IErrorHttpMapper>();
        var mapper2 = Substitute.For<IErrorHttpMapper>();
        IReadOnlyList<IErrorHttpMapper> mappers = new[] { mapper1, mapper2 };

        // Act (When)
        var sut = new CompositeErrorHttpMapper(mappers);

        // Assert (Then)
        Assert.NotNull(sut);
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when mappers array is null")]
    public void Constructor_ParamsArrayNull_ThrowsArgumentNullException()
    {
        // Arrange (Given) & Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => new CompositeErrorHttpMapper(null!));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when mappers list is null")]
    public void Constructor_ReadOnlyListNull_ThrowsArgumentNullException()
    {
        // Arrange (Given) & Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() =>
            new CompositeErrorHttpMapper((IReadOnlyList<IErrorHttpMapper>)null!));
    }
}