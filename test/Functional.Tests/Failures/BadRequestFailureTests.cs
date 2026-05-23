using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests.Failures;

/// <summary>
///     Tests for BadRequestFailure type.
/// </summary>
public class BadRequestFailureTests
{
    [Fact]
    public void BadRequestError_WithMessage_SetsMessage()
    {
        // Arrange & Act (Given/When)
        var error = new BadRequestFailure("Request body is malformed");

        // Assert (Then)
        Assert.Equal((string?)"Request body is malformed", (string?)error.Message);
        Assert.Equal((string?)FailureCodes.BadRequest,      (string?)error.Code);
    }

    [Fact]
    public void BadRequestError_Code_IsBadRequestErrorCode()
    {
        // Arrange & Act (Given/When)
        var error = new BadRequestFailure("Invalid input");

        // Assert (Then)
        Assert.Equal((string?)FailureCodes.BadRequest, (string?)error.Code);
    }

    [Fact]
    public void BadRequestError_WithEmptyMessage_PreservesEmptyMessage()
    {
        // Arrange & Act (Given/When)
        var error = new BadRequestFailure("");

        // Assert (Then)
        Assert.Equal((string?)"", (string?)error.Message);
    }

    [Fact]
    public void BadRequestError_WithExtra_PreservesExtraData()
    {
        // Arrange (Given)
        var extra = new Dictionary<string, object?>
        {
            ["field"]  = "userId",
            ["reason"] = "must be a positive integer"
        };

        // Act (When)
        var error = new BadRequestFailure("Invalid field value", extra);

        // Assert (Then)
        Assert.Equal((string?)"Invalid field value", (string?)error.Message);
        Assert.NotNull(error.Metadata);
        Assert.Equal<object>("userId",                  error.Metadata["field"]);
        Assert.Equal<object>("must be a positive integer", error.Metadata["reason"]);
    }

    [Fact]
    public void BadRequestError_RecordEquality_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new BadRequestFailure("Malformed JSON");
        var error2 = new BadRequestFailure("Malformed JSON");

        // Act & Assert (When/Then)
        Assert.Equal(error1, error2);
    }

    [Fact]
    public void BadRequestError_RecordInequality_WithDifferentMessage_WorksCorrectly()
    {
        // Arrange (Given)
        var error1 = new BadRequestFailure("Malformed JSON");
        var error2 = new BadRequestFailure("Missing required field");

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void BadRequestError_RecordInequality_WithDifferentExtra_WorksCorrectly()
    {
        // Arrange (Given)
        var extra1 = new Dictionary<string, object?> { ["key"] = "value1" };
        var extra2 = new Dictionary<string, object?> { ["key"] = "value2" };
        var error1 = new BadRequestFailure("Bad input", extra1);
        var error2 = new BadRequestFailure("Bad input", extra2);

        // Act & Assert (When/Then)
        Assert.NotEqual(error1, error2);
    }

    [Fact]
    public void BadRequestError_CanBeUsedInResult()
    {
        // Arrange (Given)
        var badRequestError = new BadRequestFailure("Invalid request format");

        // Act (When)
        var result = Result.Failure<string>(badRequestError);

        // Assert (Then)
        Assert.True(result.IsFailure);
        result.TryGetFailure(out var error);
        Assert.IsType<BadRequestFailure>(error);
        var typedError = (BadRequestFailure)error;
        Assert.Equal((string?)"Invalid request format", (string?)typedError.Message);
        Assert.Equal((string?)FailureCodes.BadRequest,    (string?)typedError.Code);
    }

    [Fact]
    public void BadRequestError_InheritsFromFailure()
    {
        // Arrange & Act (Given/When)
        var error = new BadRequestFailure("Bad input");

        // Assert (Then)
        Assert.IsAssignableFrom<Failure>(error);
    }

    [Fact]
    public void BadRequestError_WithComplexMessage_PreservesMessage()
    {
        // Arrange (Given)
        var complexMessage =
            "Request validation failed: the 'startDate' field must precede 'endDate' and both must be within the allowed range of the last 90 days.";

        // Act (When)
        var error = new BadRequestFailure(complexMessage);

        // Assert (Then)
        Assert.Equal(complexMessage, (string?)error.Message);
    }
}
