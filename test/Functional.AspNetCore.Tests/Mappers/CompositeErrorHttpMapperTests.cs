using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class CompositeErrorHttpMapperTests
{
    [Fact(DisplayName = "GetStatusCode returns status from first mapper that provides one")]
    public void GetStatusCode_FirstMapperReturnsStatus_ReturnsFirstStatus()
    {
        // Arrange (Given)
        var error = new ValidationError(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetStatusCode(error).Returns(400);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetStatusCode(error).Returns(500);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(400, statusCode);
        mapper1.Received(1).GetStatusCode(error);
        mapper2.DidNotReceive().GetStatusCode(Arg.Any<IError>());
    }

    [Fact(DisplayName = "GetStatusCode tries next mapper when first returns null")]
    public void GetStatusCode_FirstMapperReturnsNull_TriesNextMapper()
    {
        // Arrange (Given)
        var error = new ValidationError(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetStatusCode(error).Returns((int?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetStatusCode(error).Returns(400);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Equal(400, statusCode);
        mapper1.Received(1).GetStatusCode(error);
        mapper2.Received(1).GetStatusCode(error);
    }

    [Fact(DisplayName = "GetStatusCode returns null when all mappers return null")]
    public void GetStatusCode_AllMappersReturnNull_ReturnsNull()
    {
        // Arrange (Given)
        var error = new ValidationError(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetStatusCode(error).Returns((int?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetStatusCode(error).Returns((int?)null);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var statusCode = sut.GetStatusCode(error);

        // Assert (Then)
        Assert.Null(statusCode);
        mapper1.Received(1).GetStatusCode(error);
        mapper2.Received(1).GetStatusCode(error);
    }

    [Fact(DisplayName = "GetResponseBody returns body from first mapper that provides one")]
    public void GetResponseBody_FirstMapperReturnsBody_ReturnsFirstBody()
    {
        // Arrange (Given)
        var error = new ValidationError(["Field is required"]);
        var expectedBody = new { error = "validation_error" };

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponseBody(error).Returns(expectedBody);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponseBody(error).Returns(new { error = "other_error" });

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        Assert.Same(expectedBody, body);
        mapper1.Received(1).GetResponseBody(error);
        mapper2.DidNotReceive().GetResponseBody(Arg.Any<IError>());
    }

    [Fact(DisplayName = "GetResponseBody tries next mapper when first returns null")]
    public void GetResponseBody_FirstMapperReturnsNull_TriesNextMapper()
    {
        // Arrange (Given)
        var error = new ValidationError(["Field is required"]);
        var expectedBody = new { error = "validation_error" };

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponseBody(error).Returns((object?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponseBody(error).Returns(expectedBody);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        Assert.Same(expectedBody, body);
        mapper1.Received(1).GetResponseBody(error);
        mapper2.Received(1).GetResponseBody(error);
    }

    [Fact(DisplayName = "GetResponseBody returns null when all mappers return null")]
    public void GetResponseBody_AllMappersReturnNull_ReturnsNull()
    {
        // Arrange (Given)
        var error = new ValidationError(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponseBody(error).Returns((object?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponseBody(error).Returns((object?)null);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var body = sut.GetResponseBody(error);

        // Assert (Then)
        Assert.Null(body);
        mapper1.Received(1).GetResponseBody(error);
        mapper2.Received(1).GetResponseBody(error);
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
        Assert.Throws<ArgumentNullException>(() => new CompositeErrorHttpMapper((IErrorHttpMapper[])null!));
    }

    [Fact(DisplayName = "Constructor throws ArgumentNullException when mappers list is null")]
    public void Constructor_ReadOnlyListNull_ThrowsArgumentNullException()
    {
        // Arrange (Given) & Act (When) & Assert (Then)
        Assert.Throws<ArgumentNullException>(() => new CompositeErrorHttpMapper((IReadOnlyList<IErrorHttpMapper>)null!));
    }
}
