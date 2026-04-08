using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests.Failures;

/// <summary>
///     Tests for FunctionalException.
/// </summary>
public class FunctionalExceptionTests
{
    [Fact]
    public void Constructor_WithFailure_SetsMessageAndCode()
    {
        // Arrange (Given)
        var error = new Failure("ERROR_CODE", "Error message");

        // Act (When)
        var exception = new FunctionalException(error);

        // Assert (Then)
        Assert.Equal("Error message", exception.Message);
        Assert.Equal("ERROR_CODE", exception.Code);
    }

    [Fact]
    public void Constructor_WithFailureWithMetadata_CopiesMetadataToDataDictionary()
    {
        // Arrange (Given)
        var metadata = new Dictionary<string, object?>
        {
            ["Key1"] = "Value1",
            ["Key2"] = 42,
            ["Key3"] = true
        };
        var error = new Failure("ERROR_CODE", "Error message", metadata);

        // Act (When)
        var exception = new FunctionalException(error);

        // Assert (Then)
        Assert.Equal(3, exception.Data.Count);
        Assert.Equal("Value1", exception.Data["Key1"]);
        Assert.Equal(42, exception.Data["Key2"]);
        Assert.Equal(true, exception.Data["Key3"]);
    }

    [Fact]
    public void Constructor_WithFailureWithNoMetadata_HasEmptyDataDictionary()
    {
        // Arrange (Given)
        var error = new Failure("ERROR_CODE", "Error message");

        // Act (When)
        var exception = new FunctionalException(error);

        // Assert (Then)
        Assert.Empty(exception.Data);
    }

    [Fact]
    public void Constructor_WithValidationFailure_SetsPropertiesCorrectly()
    {
        // Arrange (Given)
        var validationError = new ValidationFailure([
            "Field1 is required",
            "Field2 is invalid"
        ]);

        // Act (When)
        var exception = new FunctionalException(validationError);

        // Assert (Then)
        Assert.Equal(FailureCodes.Validation, exception.Code);
        Assert.Contains("Field1 is required", exception.Message);
        Assert.Contains("Field2 is invalid", exception.Message);
    }

    [Fact]
    public void Constructor_WithNotFoundFailure_SetsPropertiesCorrectly()
    {
        // Arrange (Given)
        var notFoundError = new NotFoundFailure("User", "123");

        // Act (When)
        var exception = new FunctionalException(notFoundError);

        // Assert (Then)
        Assert.Equal(FailureCodes.NotFound, exception.Code);
        Assert.Contains("User", exception.Message);
        Assert.Contains("123", exception.Message);
    }

    [Fact]
    public void Code_Property_ReturnsErrorCode()
    {
        // Arrange (Given)
        var error = new Failure("CUSTOM_CODE", "Message");
        var exception = new FunctionalException(error);

        // Act (When)
        var code = exception.Code;

        // Assert (Then)
        Assert.Equal("CUSTOM_CODE", code);
    }

    [Fact]
    public void Constructor_WithComplexMetadata_PreservesAllValues()
    {
        // Arrange (Given)
        var metadata = new Dictionary<string, object?>
        {
            ["StringValue"] = "test",
            ["IntValue"] = 123,
            ["BoolValue"] = false,
            ["DoubleValue"] = 3.14,
            ["NullValue"] = null
        };
        var error = new Failure("ERROR_CODE", "Message", metadata);

        // Act (When)
        var exception = new FunctionalException(error);

        // Assert (Then)
        Assert.Equal("test", exception.Data["StringValue"]);
        Assert.Equal(123, exception.Data["IntValue"]);
        Assert.Equal(false, exception.Data["BoolValue"]);
        Assert.Equal(3.14, exception.Data["DoubleValue"]);
        Assert.Null(exception.Data["NullValue"]);
    }
}
