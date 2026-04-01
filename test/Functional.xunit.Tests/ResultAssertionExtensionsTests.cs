using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.xunit.Tests;

public sealed class ResultAssertionExtensionsTests
{
    // --- Where on SuccessAssertion<T> ---

    [Fact]
    public void Where_OnSuccessAssertion_WithPassingPredicate_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .Where(v => v == 10);
    }

    [Fact]
    public void Where_OnSuccessAssertion_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .Where(v => v > 0, "value must be positive");
    }

    // --- Where on FailureAssertion ---

    [Fact]
    public void Where_OnFailureAssertion_WithPassingPredicate_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure<int>(new Failure("ERR_001", "something went wrong"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .Where(f => f.Code == "ERR_001");
    }

    [Fact]
    public void Where_OnFailureAssertion_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR_002", "bad request"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .Where(f => f.Message.StartsWith("bad"), "message should start with 'bad'");
    }

    // --- Where on SuccessAssertion<T> failure path ---

    [Fact]
    public void Where_OnSuccessAssertion_WithFailingPredicate_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Success().Where(v => v == 99));
    }

    [Fact]
    public void Where_OnSuccessAssertion_WithFailingPredicateAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Success().Where(v => v == 99, "should be 99"));
    }

    // --- Where on FailureAssertion failure path ---

    [Fact]
    public void Where_OnFailureAssertion_WithFailingPredicate_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "msg"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().Where(f => f.Code == "WRONG"));
    }

    [Fact]
    public void Where_OnFailureAssertion_WithFailingPredicateAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "msg"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().Where(f => f.Code == "WRONG", "should be WRONG"));
    }

    // --- BeEquivalentTo ---

    [Fact]
    public void BeEquivalentTo_WithMatchingValue_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .BeEquivalentTo(42);
    }

    [Fact]
    public void BeEquivalentTo_WithNonMatchingValue_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Success().BeEquivalentTo(99));
    }

    [Fact]
    public void BeEquivalentTo_WithNonMatchingValueAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Success().BeEquivalentTo(99, "should be 99"));
    }

    [Fact]
    public void BeEquivalentTo_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success("hello");

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .BeEquivalentTo("hello", "the value should match");
    }

    // --- NotBeNull ---

    [Fact]
    public void NotBeNull_WithNonNullValue_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success("non-null");

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .NotBeNull();
    }

    [Fact]
    public void NotBeNull_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success("value");

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .NotBeNull("must not be null");
    }

    // --- BeOfType ---

    [Fact]
    public void BeOfType_WithMatchingType_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success<object>("a string");

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .BeOfType<object, string>();
    }

    [Fact]
    public void BeOfType_WithWrongType_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Success<object>(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Success().BeOfType<object, string>());
    }

    [Fact]
    public void BeOfType_WithWrongTypeAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Success<object>(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Success().BeOfType<object, string>("should be string"));
    }

    [Fact]
    public void BeOfType_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success<object>(99);

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .BeOfType<object, int>("should be int");
    }

    // --- ContainMessage ---

    [Fact]
    public void ContainMessage_WithMatchingSubstring_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "validation error occurred"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .ContainMessage("validation");
    }

    [Fact]
    public void ContainMessage_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "validation error occurred"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .ContainMessage("error", "error should be in message");
    }

    [Fact]
    public void ContainMessage_WithNonMatchingSubstring_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "actual message"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().ContainMessage("not-there"));
    }

    [Fact]
    public void ContainMessage_WithNonMatchingSubstringAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "actual message"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().ContainMessage("not-there", "should contain not-there"));
    }

    // --- StartWithMessage ---

    [Fact]
    public void StartWithMessage_WithMatchingPrefix_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "validation: field is required"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .StartWithMessage("validation");
    }

    [Fact]
    public void StartWithMessage_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "bad request"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .StartWithMessage("bad", "message should start with 'bad'");
    }

    [Fact]
    public void StartWithMessage_WithNonMatchingPrefix_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "actual message"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().StartWithMessage("wrong"));
    }

    [Fact]
    public void StartWithMessage_WithNonMatchingPrefixAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR", "actual message"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().StartWithMessage("wrong", "should start with 'wrong'"));
    }

    // --- ContainCode ---

    [Fact]
    public void ContainCode_WithMatchingSubstring_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("VALIDATION_ERR", "some error"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .ContainCode("VALIDATION");
    }

    [Fact]
    public void ContainCode_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("NOT_FOUND_ERR", "not found"));

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Failure()
              .ContainCode("NOT_FOUND", "code should contain NOT_FOUND");
    }

    [Fact]
    public void ContainCode_WithNonMatchingSubstring_ThrowsFailException()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR_CODE", "error"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().ContainCode("NOT_THERE"));
    }

    [Fact]
    public void ContainCode_WithNonMatchingSubstringAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var result = Result.Failure(new Failure("ERR_CODE", "error"));

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            result.ShouldBe().Failure().ContainCode("NOT_THERE", "should contain NOT_THERE"));
    }

    // --- SatisfyAll ---

    [Fact]
    public void SatisfyAll_WithAllPassingAssertions_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var result = Result.Success(10);

        // Act (When) / Assert (Then)
        result.ShouldBe()
              .Success()
              .SatisfyAll(
                  v => Assert.True(v > 0),
                  v => Assert.True(v < 100)
              );
    }
}
