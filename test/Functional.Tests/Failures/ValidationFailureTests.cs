using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests.Failures;

/// <summary>
///     Tests for ValidationFailure type.
/// </summary>
public class ValidationFailureTests
{
    [Fact]
    public void Constructor_WithStructuredErrors_ExposesErrorsAndMessages()
    {
        // Arrange (Given)
        ValidationError[] errors =
        [
            new("Email", "Email is required.", "Required"),
            new(null, "Dates are inconsistent.")
        ];

        // Act (When)
        var failure = new ValidationFailure(errors);

        // Assert (Then)
        Assert.Equal(errors, failure.Errors);
        Assert.Equal(["Email is required.", "Dates are inconsistent."], failure.Failures);
        Assert.Equal("Email is required.; Dates are inconsistent.", failure.Message);
        Assert.Equal(FailureCodes.Validation, failure.Code);
        Assert.Same(failure.Failures, failure.Metadata["failures"]);
    }

    [Fact]
    public void Constructor_WithMessages_CreatesFieldlessErrors()
    {
        // Arrange (Given) & Act (When)
        var failure = new ValidationFailure(["Name is required"]);

        // Assert (Then)
        var error = Assert.Single(failure.Errors);
        Assert.Equal(new ValidationError(null, "Name is required"), error);
        Assert.Equal(["Name is required"], failure.Failures);
    }

    [Fact]
    public void Constructor_WithNoErrors_UsesDefaultMessage()
    {
        // Arrange (Given) & Act (When)
        var failure = new ValidationFailure(Array.Empty<ValidationError>());

        // Assert (Then)
        Assert.Empty(failure.Errors);
        Assert.Empty(failure.Failures);
        Assert.Equal("Validation failed.", failure.Message);
    }
}
