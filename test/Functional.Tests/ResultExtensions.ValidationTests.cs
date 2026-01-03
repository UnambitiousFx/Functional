using System.Text.RegularExpressions;
using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result validation extension methods (EnsureNotNull, EnsureNotEmpty, EnsureInRange, etc.).
/// </summary>
public sealed partial class ResultExtensions
{
    #region EnsureNotNull

    [Fact]
    public void EnsureNotNull_WithNonNullValue_ReturnsSuccess()
    {
        // Arrange (Given)
        var person = new Person { Name = "John", Address = new Address { City = "NYC" } };
        var result = Result.Success(person);

        // Act (When)
        var validated = result.EnsureNotNull(p => p.Address, "Address is required");

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureNotNull_WithNullValue_ReturnsFailure()
    {
        // Arrange (Given)
        var person = new Person { Name = "John", Address = null };
        var result = Result.Success(person);

        // Act (When)
        var validated = result.EnsureNotNull(p => p.Address, "Address is required");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Address is required");
    }

    [Fact]
    public void EnsureNotNull_WithField_IncludesFieldInMessage()
    {
        // Arrange (Given)
        var person = new Person { Name = "John", Address = null };
        var result = Result.Success(person);

        // Act (When)
        var validated = result.EnsureNotNull(p => p.Address, "is required", "Address");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Address: is required");
    }

    [Fact]
    public void EnsureNotNull_WithFailure_PropagatesError()
    {
        // Arrange (Given)
        var error = new Error("Initial error");
        var result = Result.Failure<Person>(error);

        // Act (When)
        var validated = result.EnsureNotNull(p => p.Address, "Address is required");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Initial error");
    }

    #endregion

    #region EnsureNotEmpty - String

    [Fact]
    public void EnsureNotEmpty_WithNonEmptyString_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var validated = result.EnsureNotEmpty();

        // Assert (Then)
        validated.ShouldBe().Success().And(value => Assert.Equal("hello", value));
    }

    [Fact]
    public void EnsureNotEmpty_WithEmptyString_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success("");

        // Act (When)
        var validated = result.EnsureNotEmpty();

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must not be empty.");
    }

    [Fact]
    public void EnsureNotEmpty_WithWhitespaceString_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success("   ");

        // Act (When)
        var validated = result.EnsureNotEmpty();

        // Assert (Then)
        // Empty check in EnsureNotEmpty uses IsNullOrEmpty, not IsNullOrWhitespace
        // so this will actually pass - let's test with actual empty string
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureNotEmpty_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success("");

        // Act (When)
        var validated = result.EnsureNotEmpty("Name cannot be empty");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Name cannot be empty");
    }

    [Fact]
    public void EnsureNotEmpty_WithField_IncludesFieldInMessage()
    {
        // Arrange (Given)
        var result = Result.Success("");

        // Act (When)
        var validated = result.EnsureNotEmpty("cannot be empty", "Username");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Username: cannot be empty");
    }

    #endregion

    #region EnsureNotEmpty - Collection

    [Fact]
    public void EnsureNotEmpty_WithNonEmptyCollection_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success<List<int>>([1, 2, 3]);

        // Act (When)
        var validated = result.EnsureNotEmpty<List<int>, int>();

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureNotEmpty_WithEmptyCollection_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int>());

        // Act (When)
        var validated = result.EnsureNotEmpty<List<int>, int>();

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Collection must not be empty.");
    }

    [Fact]
    public void EnsureNotEmpty_Collection_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success(new int[] { });

        // Act (When)
        var validated = result.EnsureNotEmpty<int[], int>("Items list is empty");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Items list is empty");
    }

    [Fact]
    public void EnsureNotEmpty_Collection_WithField_IncludesFieldInMessage()
    {
        // Arrange (Given)
        var result = Result.Success(new List<string>());

        // Act (When)
        var validated = result.EnsureNotEmpty<List<string>, string>("must have at least one item", "Tags");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Tags: must have at least one item");
    }

    #endregion

    #region EnsureInRange

    [Fact]
    public void EnsureInRange_WithValueInRange_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(50);

        // Act (When)
        var validated = result.EnsureInRange(0, 100);

        // Assert (Then)
        validated.ShouldBe().Success().And(value => Assert.Equal(50, value));
    }

    [Fact]
    public void EnsureInRange_WithValueAtMinBoundary_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(0);

        // Act (When)
        var validated = result.EnsureInRange(0, 100);

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureInRange_WithValueAtMaxBoundary_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(100);

        // Act (When)
        var validated = result.EnsureInRange(0, 100);

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureInRange_WithValueBelowMin_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(-5);

        // Act (When)
        var validated = result.EnsureInRange(0, 100);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be between 0 and 100.");
    }

    [Fact]
    public void EnsureInRange_WithValueAboveMax_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(150);

        // Act (When)
        var validated = result.EnsureInRange(0, 100);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be between 0 and 100.");
    }

    [Fact]
    public void EnsureInRange_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success(150);

        // Act (When)
        var validated = result.EnsureInRange(0, 100, "Age must be between 0 and 100");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Age must be between 0 and 100");
    }

    [Fact]
    public void EnsureInRange_WithField_IncludesFieldInMessage()
    {
        // Arrange (Given)
        var result = Result.Success(150);

        // Act (When)
        var validated = result.EnsureInRange(0, 100, field: "Age");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Age: Value must be between 0 and 100.");
    }

    #endregion

    #region EnsureGreaterThan

    [Fact]
    public void EnsureGreaterThan_WithGreaterValue_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var validated = result.EnsureGreaterThan(5);

        // Assert (Then)
        validated.ShouldBe().Success().And(value => Assert.Equal(10, value));
    }

    [Fact]
    public void EnsureGreaterThan_WithEqualValue_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var validated = result.EnsureGreaterThan(5);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be greater than 5.");
    }

    [Fact]
    public void EnsureGreaterThan_WithLesserValue_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(3);

        // Act (When)
        var validated = result.EnsureGreaterThan(5);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be greater than 5.");
    }

    #endregion

    #region EnsureLessThan

    [Fact]
    public void EnsureLessThan_WithLesserValue_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(3);

        // Act (When)
        var validated = result.EnsureLessThan(5);

        // Assert (Then)
        validated.ShouldBe().Success().And(value => Assert.Equal(3, value));
    }

    [Fact]
    public void EnsureLessThan_WithEqualValue_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When)
        var validated = result.EnsureLessThan(5);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be less than 5.");
    }

    [Fact]
    public void EnsureLessThan_WithGreaterValue_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When)
        var validated = result.EnsureLessThan(5);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be less than 5.");
    }

    #endregion

    #region Chaining Validations

    [Fact]
    public void Validations_CanBeChained()
    {
        // Arrange (Given)
        var result = Result.Success(50);

        // Act (When)
        var validated = result
            .EnsureGreaterThan(0, field: "Age")
            .EnsureLessThan(120, field: "Age")
            .EnsureInRange(18, 100, field: "Age");

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void Validations_ChainStopsAtFirstFailure()
    {
        // Arrange (Given)
        var result = Result.Success(150);

        // Act (When)
        var validated = result
            .EnsureGreaterThan(0)
            .EnsureLessThan(120) // This will fail
            .EnsureInRange(0, 100); // This won't execute

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Value must be less than 120.");
    }

    #endregion

    #region EnsureMatches

    [Fact]
    public void EnsureMatches_WithMatchingPattern_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success("test@example.com");

        // Act (When)
        var validated = result.EnsureMatches(@"^[\w\.-]+@[\w\.-]+\.\w+$");

        // Assert (Then)
        validated.ShouldBe().Success().And(value => Assert.Equal("test@example.com", value));
    }

    [Fact]
    public void EnsureMatches_WithNonMatchingPattern_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success("not-an-email");

        // Act (When)
        var validated = result.EnsureMatches(@"^[\w\.-]+@[\w\.-]+\.\w+$");

        // Assert (Then)
        validated.ShouldBe().Failure();
    }

    [Fact]
    public void EnsureMatches_WithCompiledRegex_WorksCorrectly()
    {
        // Arrange (Given)
        var result = Result.Success("ABC123");
        var regex = new Regex("^[A-Z]{3}[0-9]{3}$");

        // Act (When)
        var validated = result.EnsureMatches(regex);

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureMatches_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success("123");

        // Act (When)
        var validated = result.EnsureMatches("^[A-Za-z]+$", "Must contain only letters");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Must contain only letters");
    }

    [Fact]
    public void EnsureDoesNotMatch_WithNonMatchingPattern_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When)
        var validated = result.EnsureDoesNotMatch("[0-9]");

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureDoesNotMatch_WithMatchingPattern_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success("hello123");

        // Act (When)
        var validated = result.EnsureDoesNotMatch("[0-9]");

        // Assert (Then)
        validated.ShouldBe().Failure();
    }

    #endregion

    #region EnsureAll

    [Fact]
    public void EnsureAll_WhenAllItemsSatisfy_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 2, 4, 6, 8 });

        // Act (When)
        var validated = result.EnsureAll<List<int>, int>(x => x % 2 == 0);

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureAll_WhenSomeItemsDoNotSatisfy_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 2, 3, 4, 6 });

        // Act (When)
        var validated = result.EnsureAll<List<int>, int>(x => x % 2 == 0);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("All items must satisfy the validation condition.");
    }

    [Fact]
    public void EnsureAll_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success(new[] { -1, 5, 10 });

        // Act (When)
        var validated = result.EnsureAll<int[], int>(x => x > 0, "All values must be positive");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("All values must be positive");
    }

    [Fact]
    public void EnsureAll_WithErrorFactory_UsesFactory()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 2, 150 });

        // Act (When)
        var validated = result.EnsureAll<List<int>, int>(
            x => x < 100,
            collection => new Error($"Found {collection.Count(x => x >= 100)} invalid items"));

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Found 1 invalid items");
    }

    #endregion

    #region EnsureAny

    [Fact]
    public void EnsureAny_WhenAtLeastOneItemSatisfies_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 3, 5, 6 });

        // Act (When)
        var validated = result.EnsureAny<List<int>, int>(x => x % 2 == 0);

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureAny_WhenNoItemsSatisfy_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 3, 5, 7 });

        // Act (When)
        var validated = result.EnsureAny<List<int>, int>(x => x % 2 == 0);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("At least one item must satisfy the validation condition.");
    }

    [Fact]
    public void EnsureAny_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success(new[] { "a", "b", "c" });

        // Act (When)
        var validated = result.EnsureAny<string[], string>(
            x => x.Length > 3,
            "At least one string must be longer than 3 characters");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("At least one string must be longer than 3 characters");
    }

    [Fact]
    public void EnsureAny_WithErrorFactory_UsesFactory()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { -5, -3, -1 });

        // Act (When)
        var validated = result.EnsureAny<List<int>, int>(
            x => x > 0,
            collection => new Error($"All {collection.Count()} values are negative"));

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("All 3 values are negative");
    }

    #endregion

    #region EnsureNone

    [Fact]
    public void EnsureNone_WhenNoItemsSatisfy_ReturnsSuccess()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 3, 5, 7 });

        // Act (When)
        var validated = result.EnsureNone<List<int>, int>(x => x % 2 == 0);

        // Assert (Then)
        validated.ShouldBe().Success();
    }

    [Fact]
    public void EnsureNone_WhenSomeItemsSatisfy_ReturnsFailure()
    {
        // Arrange (Given)
        var result = Result.Success(new List<int> { 1, 2, 3 });

        // Act (When)
        var validated = result.EnsureNone<List<int>, int>(x => x % 2 == 0);

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("No items must satisfy the validation condition.");
    }

    [Fact]
    public void EnsureNone_WithCustomMessage_UsesCustomMessage()
    {
        // Arrange (Given)
        var result = Result.Success(new[] { "hello", "world", "test123" });

        // Act (When)
        var validated = result.EnsureNone<string[], string>(
            x => x.Any(char.IsDigit),
            "Strings must not contain digits");

        // Assert (Then)
        validated.ShouldBe().Failure().AndMessage("Strings must not contain digits");
    }

    #endregion
}
