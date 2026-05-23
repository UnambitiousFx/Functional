using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result.FailBadRequest extension methods.
/// </summary>
public sealed class ResultFailBadRequestTests
{
    #region FailBadRequest - Non-Generic

    [Fact]
    public void FailBadRequest_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest("Request body is malformed");

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public void FailBadRequest_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest("Invalid input");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.BadRequest);
    }

    [Fact]
    public void FailBadRequest_SetsProvidedMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest("Unsupported content type");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Unsupported content type");
    }

    [Fact]
    public void FailBadRequest_ErrorIsBadRequestError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest("Malformed JSON");

        // Assert (Then)
        result.TryGetFailure(out var error);
        Assert.IsType<BadRequestFailure>(error);
    }

    [Fact]
    public void FailBadRequest_WithEmptyMessage_CreatesFailure()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest("");

        // Assert (Then)
        result.ShouldBe()
              .Failure();
        result.ShouldBe()
              .Failure()
              .AndMessage("");
    }

    [Fact]
    public void FailBadRequest_WithLongMessage_PreservesEntireMessage()
    {
        // Arrange (Given)
        var longMessage =
            "The request could not be processed because the provided payload exceeds the maximum allowed size and contains fields that are not recognized by the current API version. Please review the API documentation and resubmit.";

        // Act (When)
        var result = Result.FailBadRequest(longMessage);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage(longMessage);
    }

    #endregion

    #region FailBadRequest - Generic

    [Fact]
    public void FailBadRequest_Generic_CreatesFailureResult()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest<string>("Request body is malformed");

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public void FailBadRequest_Generic_SetsCorrectErrorCode()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest<int>("Invalid input");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.BadRequest);
    }

    [Fact]
    public void FailBadRequest_Generic_SetsProvidedMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest<string>("Unsupported content type");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Unsupported content type");
    }

    [Fact]
    public void FailBadRequest_Generic_ErrorIsBadRequestError()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest<string>("Malformed JSON");

        // Assert (Then)
        result.TryGetFailure(out var error);
        Assert.IsType<BadRequestFailure>(error);
    }

    [Fact]
    public void FailBadRequest_Generic_CanBeUsedWithComplexType()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest<Dictionary<string, object>>("Invalid payload");

        // Assert (Then)
        result.ShouldBe()
              .Failure();
    }

    [Fact]
    public void FailBadRequest_Generic_CanBeChainedWithOtherOperations()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest<int>("Bad input")
                           .Recover(_ => 0);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(value => Assert.Equal(0, value));
    }

    #endregion

    #region FailBadRequest - Edge Cases

    [Fact]
    public void FailBadRequest_WithSpecialCharacters_PreservesMessage()
    {
        // Arrange & Act (Given/When)
        var result = Result.FailBadRequest("Bad request: field '<name>' cannot be null or empty");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Bad request: field '<name>' cannot be null or empty");
    }

    [Fact]
    public void FailBadRequest_CanBeUsedInBindChain()
    {
        // Arrange (Given)
        var input = Result.Success(-1);

        // Act (When)
        var result = input.Bind(value =>
                                    value > 0
                                        ? Result.Success(value)
                                        : Result.FailBadRequest<int>("Value must be positive"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.BadRequest);
    }

    [Fact]
    public void FailBadRequest_WithMissingFieldMessage_PreservesDetails()
    {
        // Arrange (Given)
        var message = "Required field 'email' is missing from the request body";

        // Act (When)
        var result = Result.FailBadRequest(message);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage(message);
    }

    [Fact]
    public void FailBadRequest_WithJsonInMessage_PreservesFormatting()
    {
        // Arrange (Given)
        var message = "Invalid request body: {\"error\": \"unexpected token\", \"position\": 42}";

        // Act (When)
        var result = Result.FailBadRequest(message);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage(message);
    }

    #endregion
}
