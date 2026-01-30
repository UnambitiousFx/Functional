using NSubstitute;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class CompositeErrorHttpMapperTests
{
    [Fact(DisplayName = "GetResponse returns status from first mapper that provides one")]
    public void GetResponse_FirstMapperReturnsStatus_ReturnsFirstStatus()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponse(error).Returns((400, null));

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponse(error).Returns((500, null));

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.Value.StatusCode);
        mapper1.Received(1).GetResponse(error);
        mapper2.DidNotReceive().GetResponse(Arg.Any<IFailure>());
    }

    [Fact(DisplayName = "GetResponse tries next mapper when first returns null")]
    public void GetResponse_FirstMapperReturnsNull_TriesNextMapper()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponse(error).Returns(((int StatusCode, object? Body)?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponse(error).Returns((400, null));

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Equal(400, response.Value.StatusCode);
        mapper1.Received(1).GetResponse(error);
        mapper2.Received(1).GetResponse(error);
    }

    [Fact(DisplayName = "GetResponse returns null when all mappers return null")]
    public void GetResponse_AllMappersReturnNull_ReturnsNull()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponse(error).Returns(((int StatusCode, object? Body)?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponse(error).Returns(((int StatusCode, object? Body)?)null);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var statusCode = sut.GetResponse(error);

        // Assert (Then)
        Assert.Null(statusCode);
        mapper1.Received(1).GetResponse(error);
        mapper2.Received(1).GetResponse(error);
    }

    [Fact(DisplayName = "GetResponseBody returns body from first mapper that provides one")]
    public void GetResponseBody_FirstMapperReturnsBody_ReturnsFirstBody()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var expectedBody = new { error = "validation_error" };

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponse(error).Returns((400, expectedBody));

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponse(error).Returns((400, new { error = "other_error" }));

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Same(expectedBody, response.Value.Body);
        mapper1.Received(1).GetResponse(error);
        mapper2.DidNotReceive().GetResponse(Arg.Any<IFailure>());
    }

    [Fact(DisplayName = "GetResponseBody tries next mapper when first returns null")]
    public void GetResponseBody_FirstMapperReturnsNull_TriesNextMapper()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);
        var expectedBody = new { error = "validation_error" };

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponse(error).Returns((object?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponse(error).Returns((400, expectedBody));

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetResponse(error);

        // Assert (Then)
        Assert.NotNull(response);
        Assert.Same(expectedBody, response.Value.Body);
        mapper1.Received(1).GetResponse(error);
        mapper2.Received(1).GetResponse(error);
    }

    [Fact(DisplayName = "GetResponseBody returns null when all mappers return null")]
    public void GetResponseBody_AllMappersReturnNull_ReturnsNull()
    {
        // Arrange (Given)
        var error = new ValidationFailure(["Field is required"]);

        var mapper1 = Substitute.For<IErrorHttpMapper>();
        mapper1.GetResponse(error).Returns((object?)null);

        var mapper2 = Substitute.For<IErrorHttpMapper>();
        mapper2.GetResponse(error).Returns((object?)null);

        var sut = new CompositeErrorHttpMapper(mapper1, mapper2);

        // Act (When)
        var response = sut.GetResponse(error);

        // Assert (Then)
        Assert.Null(response);
        mapper1.Received(1).GetResponse(error);
        mapper2.Received(1).GetResponse(error);
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