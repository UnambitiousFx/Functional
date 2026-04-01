using UnambitiousFx.Functional.xunit.ValueTasks;

namespace UnambitiousFx.Functional.xunit.Tests;

public sealed class MaybeAssertionExtensionsTests
{
    // --- BeEquivalentTo ---

    [Fact]
    public void BeEquivalentTo_WithMatchingValue_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .BeEquivalentTo(42);
    }

    [Fact]
    public void BeEquivalentTo_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some("hello");

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .BeEquivalentTo("hello", "the value should match");
    }

    // --- NotBeNull ---

    [Fact]
    public void NotBeNull_WithNonNullValue_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some("non-null");

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .NotBeNull();
    }

    [Fact]
    public void NotBeNull_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some("non-null");

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .NotBeNull("value must not be null");
    }

    // --- BeOfType ---

    [Fact]
    public void BeOfType_WithMatchingType_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some<object>("a string");

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .BeOfType<object, string>();
    }

    [Fact]
    public void BeOfType_WithBecauseParam_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some<object>(123);

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .BeOfType<object, int>("should be int");
    }

    [Fact]
    public void BeEquivalentTo_WithNonMatchingValue_ThrowsFailException()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().Some().BeEquivalentTo(99));
    }

    [Fact]
    public void BeEquivalentTo_WithNonMatchingValueAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            maybe.ShouldBe().Some().BeEquivalentTo(99, "should be 99"));
    }

    // --- BeOfType failure path ---

    [Fact]
    public void BeOfType_WithWrongType_ThrowsFailException()
    {
        // Arrange (Given)
        var maybe = Maybe.Some<object>(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().Some().BeOfType<object, string>());
    }

    [Fact]
    public void BeOfType_WithWrongTypeAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var maybe = Maybe.Some<object>(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            maybe.ShouldBe().Some().BeOfType<object, string>("should be string"));
    }

    // --- Where failure path ---

    [Fact]
    public void Where_WithFailingPredicate_ThrowsFailException()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() => maybe.ShouldBe().Some().Where(v => v == 99));
    }

    [Fact]
    public void Where_WithFailingPredicateAndBecause_ThrowsWithBecause()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When) / Assert (Then)
        Assert.Throws<Xunit.Sdk.FailException>(() =>
            maybe.ShouldBe().Some().Where(v => v == 99, "should be 99"));
    }

    // --- SatisfyAll ---

    [Fact]
    public void SatisfyAll_WithAllPassingAssertions_ReturnsSameAssertion()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(10);

        // Act (When) / Assert (Then)
        maybe.ShouldBe()
             .Some()
             .SatisfyAll(
                 v => Assert.True(v > 0),
                 v => Assert.True(v < 100)
             );
    }

    // --- Task<T> async overloads ---

    [Fact]
    public async Task Task_ShouldBe_WithSomeMaybe_ReturnsAssertion()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(99));

        // Act (When)
        var assertion = await maybeTask.ShouldBe();

        // Assert (Then)
        assertion.Some().And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task Task_ShouldBe_Some_WithSomeMaybe_ReturnsSomeAssertion()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(7));

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe()
                       .Some()
                       .And(v => Assert.Equal(7, v));
    }

    [Fact]
    public async Task Task_ShouldBe_None_WithNoneMaybe_ReturnsNoneAssertion()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.None<int>());

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe()
                       .None();
    }

    [Fact]
    public async Task Task_SomeAssertion_And_ExecutesAction()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(5));

        // Act (When) / Assert (Then)
        await maybeTask.ShouldBe()
                       .Some()
                       .And(v => Assert.Equal(5, v));
    }

    [Fact]
    public async Task Task_SomeAssertion_Map_TransformsValue()
    {
        // Arrange (Given)
        var maybeTask = Task.FromResult(Maybe.Some(3));

        // Act (When)
        var result = await maybeTask.ShouldBe()
                                    .Some()
                                    .Map(v => v * 2);

        // Assert (Then)
        Assert.Equal(6, result.Value);
    }
}
