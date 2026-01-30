using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class OneOfTests
{
    #region OneOf<T1, T2>

    [Fact]
    public void FromFirst_WithFirstValue_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        var oneOf = OneOf<int, string>.FromFirst(value);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        oneOf.ShouldBe().First().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void FromSecond_WithSecondValue_CreatesSecond()
    {
        // Arrange (Given)
        var value = "hello";

        // Act (When)
        var oneOf = OneOf<int, string>.FromSecond(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.True(oneOf.IsSecond);
        oneOf.ShouldBe().Second().And(v => Assert.Equal("hello", v));
    }

    [Fact]
    public void ImplicitConversion_FromFirstType_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        OneOf<int, string> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        oneOf.ShouldBe().First().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public void ImplicitConversion_FromSecondType_CreatesSecond()
    {
        // Arrange (Given)
        var value = "hello";

        // Act (When)
        OneOf<int, string> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
        oneOf.ShouldBe().Second().And(v => Assert.Equal("hello", v));
    }

    [Fact]
    public void First_WhenIsFirst_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromFirst(42);

        // Act (When)
        var result = oneOf.TryGetFirst(out var value);

        // Assert (Then)
        Assert.True(result);
        Assert.Equal(42, value);
    }

    [Fact]
    public void First_WhenIsSecond_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromSecond("hello");

        // Act (When)
        var result = oneOf.TryGetFirst(out var value);

        // Assert (Then)
        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void Match_Func_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string>.FromFirst(42);
        var oneOfSecond = OneOf<int, string>.FromSecond("hello");

        // Act (When)
        var result1 = oneOfFirst.Match(
            first => first.ToString(),
            second => second);

        var result2 = oneOfSecond.Match(
            first => first.ToString(),
            second => second);

        // Assert (Then)
        Assert.Equal("42", result1);
        Assert.Equal("hello", result2);
    }

    [Fact]
    public void Match_Action_WithFirst_ExecutesFirstAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromFirst(42);
        var firstCalled = false;
        var secondCalled = false;

        // Act (When)
        oneOf.Match(
            _ => firstCalled = true,
            _ => secondCalled = true);

        // Assert (Then)
        Assert.True(firstCalled);
        Assert.False(secondCalled);
    }

    [Fact]
    public void Match_Action_WithSecond_ExecutesSecondAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromSecond("hello");
        var firstCalled = false;
        var secondCalled = false;

        // Act (When)
        oneOf.Match(
            _ => firstCalled = true,
            _ => secondCalled = true);

        // Assert (Then)
        Assert.False(firstCalled);
        Assert.True(secondCalled);
    }

    [Fact]
    public void Second_WhenIsSecond_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromSecond("hello");

        // Act (When)
        var result = oneOf.TryGetSecond(out var value);

        // Assert (Then)
        Assert.True(result);
        Assert.Equal("hello", value);
    }

    [Fact]
    public void Second_WhenIsFirst_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromFirst(42);

        // Act (When)
        var result = oneOf.TryGetSecond(out var value);

        // Assert (Then)
        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void IsFirst_WithFirstValue_ReturnsTrue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromFirst(42);

        // Act (When)
        var isFirst = oneOf.IsFirst;

        // Assert (Then)
        Assert.True(isFirst);
    }

    [Fact]
    public void IsFirst_WithSecondValue_ReturnsFalse()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromSecond("hello");

        // Act (When)
        var isFirst = oneOf.IsFirst;

        // Assert (Then)
        Assert.False(isFirst);
    }

    [Fact]
    public void IsSecond_WithSecondValue_ReturnsTrue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromSecond("hello");

        // Act (When)
        var isSecond = oneOf.IsSecond;

        // Assert (Then)
        Assert.True(isSecond);
    }

    [Fact]
    public void IsSecond_WithFirstValue_ReturnsFalse()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string>.FromFirst(42);

        // Act (When)
        var isSecond = oneOf.IsSecond;

        // Assert (Then)
        Assert.False(isSecond);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-1)]
    public void OneOf2_WithDifferentFirstValues_StoresCorrectly(int value)
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string>.FromFirst(value);

        // Assert (Then)
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(value, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("a very long string with special chars!@#$%")]
    public void OneOf2_WithDifferentSecondValues_StoresCorrectly(string value)
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string>.FromSecond(value);

        // Assert (Then)
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal(value, result);
    }

    [Fact]
    public void OneOf2_Equality_WithSameFirstValue_AreEqual()
    {
        // Arrange (Given)
        var oneOf1 = OneOf<int, string>.FromFirst(42);
        var oneOf2 = OneOf<int, string>.FromFirst(42);

        // Act & Assert (Then)
        Assert.Equal(oneOf1, oneOf2);
        Assert.True(oneOf1 == oneOf2);
        Assert.False(oneOf1 != oneOf2);
    }

    [Fact]
    public void OneOf2_Equality_WithDifferentFirstValues_AreNotEqual()
    {
        // Arrange (Given)
        var oneOf1 = OneOf<int, string>.FromFirst(42);
        var oneOf2 = OneOf<int, string>.FromFirst(99);

        // Act & Assert (Then)
        Assert.NotEqual(oneOf1, oneOf2);
        Assert.False(oneOf1 == oneOf2);
        Assert.True(oneOf1 != oneOf2);
    }

    [Fact]
    public void OneOf2_Equality_WithDifferentTypes_AreNotEqual()
    {
        // Arrange (Given)
        var oneOf1 = OneOf<int, string>.FromFirst(42);
        var oneOf2 = OneOf<int, string>.FromSecond("42");

        // Act & Assert (Then)
        Assert.NotEqual(oneOf1, oneOf2);
        Assert.False(oneOf1 == oneOf2);
        Assert.True(oneOf1 != oneOf2);
    }

    [Fact]
    public void OneOf2_GetHashCode_WithSameValue_AreEqual()
    {
        // Arrange (Given)
        var oneOf1 = OneOf<int, string>.FromFirst(42);
        var oneOf2 = OneOf<int, string>.FromFirst(42);

        // Act (When)
        var hash1 = oneOf1.GetHashCode();
        var hash2 = oneOf2.GetHashCode();

        // Assert (Then)
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void OneOf2_ToString_ReturnsValueString()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string>.FromFirst(42);
        var oneOfSecond = OneOf<int, string>.FromSecond("test");

        // Act (When)
        var str1 = oneOfFirst.ToString();
        var str2 = oneOfSecond.ToString();

        // Assert (Then)
        Assert.Equal("42", str1);
        Assert.Equal("test", str2);
    }

    #endregion

    #region OneOf<T1, T2, T3>

    [Fact]
    public void OneOf3_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        var oneOf = OneOf<int, string, bool>.FromFirst(value);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(42, result);
    }

    [Fact]
    public void OneOf3_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "hello";

        // Act (When)
        var oneOf = OneOf<int, string, bool>.FromSecond(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.True(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal("hello", result);
    }

    [Fact]
    public void OneOf3_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = true;

        // Act (When)
        var oneOf = OneOf<int, string, bool>.FromThird(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.True(oneOf.IsThird);
        Assert.True(oneOf.TryGetThird(out var result));
        Assert.True(result);
    }

    [Fact]
    public void OneOf3_ImplicitConversion_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        OneOf<int, string, bool> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(42, result);
    }

    [Fact]
    public void OneOf3_ImplicitConversion_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "test";

        // Act (When)
        OneOf<int, string, bool> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal("test", result);
    }

    [Fact]
    public void OneOf3_ImplicitConversion_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = false;

        // Act (When)
        OneOf<int, string, bool> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsThird);
        Assert.True(oneOf.TryGetThird(out var result));
        Assert.False(result);
    }

    [Fact]
    public void OneOf3_First_WhenIsFirst_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromFirst(42);

        // Act (When)
        var success = oneOf.TryGetFirst(out var value);

        // Assert (Then)
        Assert.True(success);
        Assert.Equal(42, value);
    }

    [Fact]
    public void OneOf3_First_WhenNotFirst_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromSecond("test");

        // Act (When)
        var success = oneOf.TryGetFirst(out var value);

        // Assert (Then)
        Assert.False(success);
        Assert.Equal(default, value);
    }

    [Fact]
    public void OneOf3_Second_WhenIsSecond_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromSecond("test");

        // Act (When)
        var success = oneOf.TryGetSecond(out var value);

        // Assert (Then)
        Assert.True(success);
        Assert.Equal("test", value);
    }

    [Fact]
    public void OneOf3_Second_WhenNotSecond_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromThird(true);

        // Act (When)
        var success = oneOf.TryGetSecond(out var value);

        // Assert (Then)
        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void OneOf3_Third_WhenIsThird_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromThird(true);

        // Act (When)
        var success = oneOf.TryGetThird(out var value);

        // Assert (Then)
        Assert.True(success);
        Assert.True(value);
    }

    [Fact]
    public void OneOf3_Third_WhenNotThird_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromFirst(42);

        // Act (When)
        var success = oneOf.TryGetThird(out var value);

        // Assert (Then)
        Assert.False(success);
        Assert.False(value);
    }

    [Fact]
    public void OneOf3_Match_Func_WithFirst_ExecutesFirstBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromFirst(42);

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third");

        // Assert (Then)
        Assert.Equal("first", result);
    }

    [Fact]
    public void OneOf3_Match_Func_WithSecond_ExecutesSecondBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromSecond("test");

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third");

        // Assert (Then)
        Assert.Equal("second", result);
    }

    [Fact]
    public void OneOf3_Match_Func_WithThird_ExecutesThirdBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromThird(true);

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third");

        // Assert (Then)
        Assert.Equal("third", result);
    }

    [Fact]
    public void OneOf3_Match_Action_WithFirst_ExecutesFirstAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromFirst(42);
        var firstCalled = false;
        var secondCalled = false;
        var thirdCalled = false;

        // Act (When)
        oneOf.Match(
            _ => firstCalled = true,
            _ => secondCalled = true,
            _ => thirdCalled = true);

        // Assert (Then)
        Assert.True(firstCalled);
        Assert.False(secondCalled);
        Assert.False(thirdCalled);
    }

    [Fact]
    public void OneOf3_Match_Action_WithSecond_ExecutesSecondAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromSecond("test");
        var firstCalled = false;
        var secondCalled = false;
        var thirdCalled = false;

        // Act (When)
        oneOf.Match(
            _ => firstCalled = true,
            _ => secondCalled = true,
            _ => thirdCalled = true);

        // Assert (Then)
        Assert.False(firstCalled);
        Assert.True(secondCalled);
        Assert.False(thirdCalled);
    }

    [Fact]
    public void OneOf3_Match_Action_WithThird_ExecutesThirdAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool>.FromThird(true);
        var firstCalled = false;
        var secondCalled = false;
        var thirdCalled = false;

        // Act (When)
        oneOf.Match(
            _ => firstCalled = true,
            _ => secondCalled = true,
            _ => thirdCalled = true);

        // Assert (Then)
        Assert.False(firstCalled);
        Assert.False(secondCalled);
        Assert.True(thirdCalled);
    }

    [Fact]
    public void OneOf3_Equality_WithSameValues_AreEqual()
    {
        // Arrange (Given)
        var oneOf1 = OneOf<int, string, bool>.FromThird(true);
        var oneOf2 = OneOf<int, string, bool>.FromThird(true);

        // Act & Assert (Then)
        Assert.Equal(oneOf1, oneOf2);
        Assert.True(oneOf1 == oneOf2);
        Assert.False(oneOf1 != oneOf2);
    }

    [Fact]
    public void OneOf3_GetHashCode_WithSameValue_AreEqual()
    {
        // Arrange (Given)
        var oneOf1 = OneOf<int, string, bool>.FromSecond("test");
        var oneOf2 = OneOf<int, string, bool>.FromSecond("test");

        // Act (When)
        var hash1 = oneOf1.GetHashCode();
        var hash2 = oneOf2.GetHashCode();

        // Assert (Then)
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void OneOf3_ToString_ReturnsValueString()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string, bool>.FromFirst(42);
        var oneOfSecond = OneOf<int, string, bool>.FromSecond("hello");
        var oneOfThird = OneOf<int, string, bool>.FromThird(false);

        // Act (When)
        var str1 = oneOfFirst.ToString();
        var str2 = oneOfSecond.ToString();
        var str3 = oneOfThird.ToString();

        // Assert (Then)
        Assert.Equal("42", str1);
        Assert.Equal("hello", str2);
        Assert.Equal("False", str3);
    }

    #endregion

    #region OneOf<T1, T2, T3, T4>

    [Fact]
    public void OneOf4_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        var oneOf = OneOf<int, string, bool, double>.FromFirst(value);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(42, result);
    }

    [Fact]
    public void OneOf4_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "test";

        // Act (When)
        var oneOf = OneOf<int, string, bool, double>.FromSecond(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.True(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal("test", result);
    }

    [Fact]
    public void OneOf4_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = true;

        // Act (When)
        var oneOf = OneOf<int, string, bool, double>.FromThird(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.True(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.True(oneOf.TryGetThird(out var result));
        Assert.True(result);
    }

    [Fact]
    public void OneOf4_FromFourth_CreatesFourth()
    {
        // Arrange (Given)
        var value = 3.14;

        // Act (When)
        var oneOf = OneOf<int, string, bool, double>.FromFourth(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.True(oneOf.IsFourth);
        Assert.True(oneOf.TryGetFourth(out var result));
        Assert.Equal(3.14, result);
    }

    [Fact]
    public void OneOf4_ImplicitConversion_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 99;

        // Act (When)
        OneOf<int, string, bool, double> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
    }

    [Fact]
    public void OneOf4_ImplicitConversion_FromFourth_CreatesFourth()
    {
        // Arrange (Given)
        var value = 2.71;

        // Act (When)
        OneOf<int, string, bool, double> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
    }

    [Fact]
    public void OneOf4_Match_Func_WithFirst_ExecutesFirstBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromFirst(42);

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third",
            fourth => "fourth");

        // Assert (Then)
        Assert.Equal("first", result);
    }

    [Fact]
    public void OneOf4_Match_Func_WithSecond_ExecutesSecondBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromSecond("test");

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third",
            fourth => "fourth");

        // Assert (Then)
        Assert.Equal("second", result);
    }

    [Fact]
    public void OneOf4_Match_Func_WithThird_ExecutesThirdBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromThird(true);

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third",
            fourth => "fourth");

        // Assert (Then)
        Assert.Equal("third", result);
    }

    [Fact]
    public void OneOf4_Match_Func_WithFourth_ExecutesFourthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromFourth(3.14);

        // Act (When)
        var result = oneOf.Match(
            first => "first",
            second => "second",
            third => "third",
            fourth => "fourth");

        // Assert (Then)
        Assert.Equal("fourth", result);
    }

    [Fact]
    public void OneOf4_Match_Action_WithFirst_ExecutesFirstAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromFirst(42);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            _ => calls.Add("first"),
            _ => calls.Add("second"),
            _ => calls.Add("third"),
            _ => calls.Add("fourth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("first", calls[0]);
    }

    [Fact]
    public void OneOf4_Match_Action_WithFourth_ExecutesFourthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromFourth(3.14);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            _ => calls.Add("first"),
            _ => calls.Add("second"),
            _ => calls.Add("third"),
            _ => calls.Add("fourth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fourth", calls[0]);
    }

    [Fact]
    public void OneOf4_TryGet_WhenNotMatching_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromFirst(42);

        // Act (When)
        var secondResult = oneOf.TryGetSecond(out var secondValue);
        var thirdResult = oneOf.TryGetThird(out var thirdValue);
        var fourthResult = oneOf.TryGetFourth(out var fourthValue);

        // Assert (Then)
        Assert.False(secondResult);
        Assert.Null(secondValue);
        Assert.False(thirdResult);
        Assert.False(thirdValue);
        Assert.False(fourthResult);
        Assert.Equal(default, fourthValue);
    }

    [Fact]
    public void OneOf4_Match_Action_WithSecond_ExecutesSecondAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromSecond("test");
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            _ => calls.Add("first"),
            _ => calls.Add("second"),
            _ => calls.Add("third"),
            _ => calls.Add("fourth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("second", calls[0]);
    }

    [Fact]
    public void OneOf4_Match_Action_WithThird_ExecutesThirdAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double>.FromThird(true);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            _ => calls.Add("first"),
            _ => calls.Add("second"),
            _ => calls.Add("third"),
            _ => calls.Add("fourth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("third", calls[0]);
    }

    [Fact]
    public void OneOf4_ImplicitConversion_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "test";

        // Act (When)
        OneOf<int, string, bool, double> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
    }

    [Fact]
    public void OneOf4_ImplicitConversion_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = true;

        // Act (When)
        OneOf<int, string, bool, double> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsThird);
    }

    [Fact]
    public void OneOf4_ToString_ReturnsValueString()
    {
        // Arrange (Given)
        var oneOfFourth = OneOf<int, string, bool, double>.FromFourth(2.5);

        // Act (When)
        var str = oneOfFourth.ToString();

        // Assert (Then)
        Assert.Equal(2.5.ToString(), str);
    }

    #endregion

    #region OneOf<T1, T2, T3, T4, T5>

    [Fact]
    public void OneOf5_FromFirst_CreatesFirst()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float>.FromFirst(1);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(1, result);
    }

    [Fact]
    public void OneOf5_FromFifth_CreatesFifth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float>.FromFifth(1.5f);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.True(oneOf.IsFifth);
        Assert.True(oneOf.TryGetFifth(out var result));
        Assert.Equal(1.5f, result);
    }

    [Fact]
    public void OneOf5_Match_Func_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string, bool, double, float>.FromFirst(1);
        var oneOfThird = OneOf<int, string, bool, double, float>.FromThird(true);
        var oneOfFifth = OneOf<int, string, bool, double, float>.FromFifth(1.5f);

        // Act (When)
        var result1 = oneOfFirst.Match(f => "first", s => "second", t => "third", fo => "fourth", fi => "fifth");
        var result3 = oneOfThird.Match(f => "first", s => "second", t => "third", fo => "fourth", fi => "fifth");
        var result5 = oneOfFifth.Match(f => "first", s => "second", t => "third", fo => "fourth", fi => "fifth");

        // Assert (Then)
        Assert.Equal("first", result1);
        Assert.Equal("third", result3);
        Assert.Equal("fifth", result5);
    }

    [Fact]
    public void OneOf5_Match_Action_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromFifth(1.5f);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fifth", calls[0]);
    }

    [Fact]
    public void OneOf5_ImplicitConversion_FromFifth_CreatesFifth()
    {
        // Arrange (Given)
        var value = 3.14f;

        // Act (When)
        OneOf<int, string, bool, double, float> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFifth);
    }

    [Fact]
    public void OneOf5_FromSecond_CreatesSecond()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float>.FromSecond("test");

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal("test", result);
    }

    [Fact]
    public void OneOf5_FromThird_CreatesThird()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float>.FromThird(false);

        // Assert (Then)
        Assert.True(oneOf.IsThird);
        Assert.True(oneOf.TryGetThird(out var result));
        Assert.False(result);
    }

    [Fact]
    public void OneOf5_FromFourth_CreatesFourth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float>.FromFourth(9.99);

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
        Assert.True(oneOf.TryGetFourth(out var result));
        Assert.Equal(9.99, result);
    }

    [Fact]
    public void OneOf5_Match_Func_WithSecond_ExecutesSecondBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromSecond("second");

        // Act (When)
        var result = oneOf.Match(f => "first", s => "second", t => "third", fo => "fourth", fi => "fifth");

        // Assert (Then)
        Assert.Equal("second", result);
    }

    [Fact]
    public void OneOf5_Match_Func_WithFourth_ExecutesFourthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromFourth(4.4);

        // Act (When)
        var result = oneOf.Match(f => "first", s => "second", t => "third", fo => "fourth", fi => "fifth");

        // Assert (Then)
        Assert.Equal("fourth", result);
    }

    [Fact]
    public void OneOf5_Match_Action_WithFirst_ExecutesFirstAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromFirst(1);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("first", calls[0]);
    }

    [Fact]
    public void OneOf5_Match_Action_WithSecond_ExecutesSecondAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromSecond("test");
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("second", calls[0]);
    }

    [Fact]
    public void OneOf5_Match_Action_WithThird_ExecutesThirdAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromThird(true);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("third", calls[0]);
    }

    [Fact]
    public void OneOf5_Match_Action_WithFourth_ExecutesFourthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromFourth(4.5);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fourth", calls[0]);
    }

    [Fact]
    public void OneOf5_ImplicitConversion_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        OneOf<int, string, bool, double, float> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
    }

    [Fact]
    public void OneOf5_ImplicitConversion_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "hello";

        // Act (When)
        OneOf<int, string, bool, double, float> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
    }

    [Fact]
    public void OneOf5_ImplicitConversion_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = false;

        // Act (When)
        OneOf<int, string, bool, double, float> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsThird);
    }

    [Fact]
    public void OneOf5_ImplicitConversion_FromFourth_CreatesFourth()
    {
        // Arrange (Given)
        var value = 1.23;

        // Act (When)
        OneOf<int, string, bool, double, float> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
    }

    [Fact]
    public void OneOf5_TryGet_WhenNotMatching_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float>.FromThird(true);

        // Act & Assert (Then)
        Assert.False(oneOf.TryGetFirst(out _));
        Assert.False(oneOf.TryGetSecond(out _));
        Assert.True(oneOf.TryGetThird(out var thirdValue));
        Assert.True(thirdValue);
        Assert.False(oneOf.TryGetFourth(out _));
        Assert.False(oneOf.TryGetFifth(out _));
    }

    #endregion

    #region OneOf<T1, T2, T3, T4, T5, T6>

    [Fact]
    public void OneOf6_FromSixth_CreatesSixth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSixth(100L);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.True(oneOf.IsSixth);
        Assert.True(oneOf.TryGetSixth(out var result));
        Assert.Equal(100L, result);
    }

    [Fact]
    public void OneOf6_Match_Func_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string, bool, double, float, long>.FromFirst(1);
        var oneOfSixth = OneOf<int, string, bool, double, float, long>.FromSixth(100L);

        // Act (When)
        var result1 = oneOfFirst.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6);
        var result6 = oneOfSixth.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6);

        // Assert (Then)
        Assert.Equal(1, result1);
        Assert.Equal(6, result6);
    }

    [Fact]
    public void OneOf6_Match_Action_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSixth(100L);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("sixth", calls[0]);
    }

    [Fact]
    public void OneOf6_ImplicitConversion_FromSixth_CreatesSixth()
    {
        // Arrange (Given)
        var value = 999L;

        // Act (When)
        OneOf<int, string, bool, double, float, long> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSixth);
    }

    [Fact]
    public void OneOf6_FromFirst_CreatesFirst()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFirst(42);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(42, result);
    }

    [Fact]
    public void OneOf6_FromSecond_CreatesSecond()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSecond("test");

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal("test", result);
    }

    [Fact]
    public void OneOf6_FromThird_CreatesThird()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromThird(true);

        // Assert (Then)
        Assert.True(oneOf.IsThird);
        Assert.True(oneOf.TryGetThird(out var result));
        Assert.True(result);
    }

    [Fact]
    public void OneOf6_FromFourth_CreatesFourth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFourth(3.14);

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
        Assert.True(oneOf.TryGetFourth(out var result));
        Assert.Equal(3.14, result);
    }

    [Fact]
    public void OneOf6_FromFifth_CreatesFifth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFifth(2.5f);

        // Assert (Then)
        Assert.True(oneOf.IsFifth);
        Assert.True(oneOf.TryGetFifth(out var result));
        Assert.Equal(2.5f, result);
    }

    [Fact]
    public void OneOf6_Match_Func_WithSecond_ExecutesSecondBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSecond("test");

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6);

        // Assert (Then)
        Assert.Equal(2, result);
    }

    [Fact]
    public void OneOf6_Match_Func_WithThird_ExecutesThirdBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromThird(true);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6);

        // Assert (Then)
        Assert.Equal(3, result);
    }

    [Fact]
    public void OneOf6_Match_Func_WithFourth_ExecutesFourthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFourth(4.4);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6);

        // Assert (Then)
        Assert.Equal(4, result);
    }

    [Fact]
    public void OneOf6_Match_Func_WithFifth_ExecutesFifthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFifth(5.5f);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6);

        // Assert (Then)
        Assert.Equal(5, result);
    }

    [Fact]
    public void OneOf6_Match_Action_WithFirst_ExecutesFirstAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFirst(1);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("first", calls[0]);
    }

    [Fact]
    public void OneOf6_Match_Action_WithSecond_ExecutesSecondAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSecond("test");
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("second", calls[0]);
    }

    [Fact]
    public void OneOf6_Match_Action_WithThird_ExecutesThirdAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromThird(true);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("third", calls[0]);
    }

    [Fact]
    public void OneOf6_Match_Action_WithFourth_ExecutesFourthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFourth(4.0);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fourth", calls[0]);
    }

    [Fact]
    public void OneOf6_Match_Action_WithFifth_ExecutesFifthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFifth(5.0f);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fifth", calls[0]);
    }

    [Fact]
    public void OneOf6_ImplicitConversion_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        OneOf<int, string, bool, double, float, long> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
    }

    [Fact]
    public void OneOf6_ImplicitConversion_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "test";

        // Act (When)
        OneOf<int, string, bool, double, float, long> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
    }

    [Fact]
    public void OneOf6_ImplicitConversion_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = true;

        // Act (When)
        OneOf<int, string, bool, double, float, long> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsThird);
    }

    [Fact]
    public void OneOf6_ImplicitConversion_FromFourth_CreatesFourth()
    {
        // Arrange (Given)
        var value = 2.71;

        // Act (When)
        OneOf<int, string, bool, double, float, long> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
    }

    [Fact]
    public void OneOf6_ImplicitConversion_FromFifth_CreatesFifth()
    {
        // Arrange (Given)
        var value = 1.5f;

        // Act (When)
        OneOf<int, string, bool, double, float, long> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFifth);
    }

    [Fact]
    public void OneOf6_TryGet_WhenNotMatching_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFourth(2.5);

        // Act & Assert (Then)
        Assert.False(oneOf.TryGetFirst(out _));
        Assert.False(oneOf.TryGetSecond(out _));
        Assert.False(oneOf.TryGetThird(out _));
        Assert.True(oneOf.TryGetFourth(out var fourthValue));
        Assert.Equal(2.5, fourthValue);
        Assert.False(oneOf.TryGetFifth(out _));
        Assert.False(oneOf.TryGetSixth(out _));
    }

    #endregion

    #region OneOf<T1, T2, T3, T4, T5, T6, T7>

    [Fact]
    public void OneOf7_FromSeventh_CreatesSeventh()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromSeventh(255);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.False(oneOf.IsSixth);
        Assert.True(oneOf.IsSeventh);
        Assert.True(oneOf.TryGetSeventh(out var result));
        Assert.Equal((byte)255, result);
    }

    [Fact]
    public void OneOf7_Match_Func_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string, bool, double, float, long, byte>.FromFirst(1);
        var oneOfSeventh = OneOf<int, string, bool, double, float, long, byte>.FromSeventh(7);

        // Act (When)
        var result1 = oneOfFirst.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7);
        var result7 = oneOfSeventh.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7);

        // Assert (Then)
        Assert.Equal(1, result1);
        Assert.Equal(7, result7);
    }

    [Fact]
    public void OneOf7_Match_Action_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromSeventh(7);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("seventh", calls[0]);
    }

    [Fact]
    public void OneOf7_ImplicitConversion_FromSeventh_CreatesSeventh()
    {
        // Arrange (Given)
        byte value = 128;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSeventh);
    }

    [Fact]
    public void OneOf7_FromSecond_Through_Sixth_CreatesCorrectly()
    {
        // Arrange (Given) & Act (When)
        var oneOf2 = OneOf<int, string, bool, double, float, long, byte>.FromSecond("test");
        var oneOf3 = OneOf<int, string, bool, double, float, long, byte>.FromThird(true);
        var oneOf4 = OneOf<int, string, bool, double, float, long, byte>.FromFourth(1.1);
        var oneOf5 = OneOf<int, string, bool, double, float, long, byte>.FromFifth(2.2f);
        var oneOf6 = OneOf<int, string, bool, double, float, long, byte>.FromSixth(100L);

        // Assert (Then)
        Assert.True(oneOf2.IsSecond && oneOf2.TryGetSecond(out var v2) && v2 == "test");
        Assert.True(oneOf3.IsThird && oneOf3.TryGetThird(out var v3) && v3);
        Assert.True(oneOf4.IsFourth && oneOf4.TryGetFourth(out var v4) && Math.Abs(v4 - 1.1) < 0.001);
        Assert.True(oneOf5.IsFifth && oneOf5.TryGetFifth(out var v5) && Math.Abs(v5 - 2.2f) < 0.001f);
        Assert.True(oneOf6.IsSixth && oneOf6.TryGetSixth(out var v6) && v6 == 100L);
    }

    [Fact]
    public void OneOf7_Match_Func_AllBranches_ExecuteCorrectly()
    {
        // Arrange (Given)
        var oneOfs = new[]
        {
            (OneOf<int, string, bool, double, float, long, byte>.FromSecond("s"), 2),
            (OneOf<int, string, bool, double, float, long, byte>.FromThird(true), 3),
            (OneOf<int, string, bool, double, float, long, byte>.FromFourth(4.0), 4),
            (OneOf<int, string, bool, double, float, long, byte>.FromFifth(5f), 5),
            (OneOf<int, string, bool, double, float, long, byte>.FromSixth(6L), 6)
        };

        // Act & Assert (Then)
        foreach (var (oneOf, expected) in oneOfs)
        {
            var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void OneOf7_Match_Action_AllBranches_ExecuteCorrectly()
    {
        // Arrange (Given)
        var oneOf2 = OneOf<int, string, bool, double, float, long, byte>.FromSecond("s");
        var oneOf3 = OneOf<int, string, bool, double, float, long, byte>.FromThird(true);
        var oneOf4 = OneOf<int, string, bool, double, float, long, byte>.FromFourth(4.0);
        var oneOf5 = OneOf<int, string, bool, double, float, long, byte>.FromFifth(5f);
        var oneOf6 = OneOf<int, string, bool, double, float, long, byte>.FromSixth(6L);

        // Act (When)
        var calls2 = new List<int>();
        oneOf2.Match(_ => calls2.Add(1), _ => calls2.Add(2), _ => calls2.Add(3), _ => calls2.Add(4), _ => calls2.Add(5),
            _ => calls2.Add(6), _ => calls2.Add(7));

        var calls3 = new List<int>();
        oneOf3.Match(_ => calls3.Add(1), _ => calls3.Add(2), _ => calls3.Add(3), _ => calls3.Add(4), _ => calls3.Add(5),
            _ => calls3.Add(6), _ => calls3.Add(7));

        var calls4 = new List<int>();
        oneOf4.Match(_ => calls4.Add(1), _ => calls4.Add(2), _ => calls4.Add(3), _ => calls4.Add(4), _ => calls4.Add(5),
            _ => calls4.Add(6), _ => calls4.Add(7));

        var calls5 = new List<int>();
        oneOf5.Match(_ => calls5.Add(1), _ => calls5.Add(2), _ => calls5.Add(3), _ => calls5.Add(4), _ => calls5.Add(5),
            _ => calls5.Add(6), _ => calls5.Add(7));

        var calls6 = new List<int>();
        oneOf6.Match(_ => calls6.Add(1), _ => calls6.Add(2), _ => calls6.Add(3), _ => calls6.Add(4), _ => calls6.Add(5),
            _ => calls6.Add(6), _ => calls6.Add(7));

        // Assert (Then)
        Assert.Equal(new[] { 2 }, calls2);
        Assert.Equal(new[] { 3 }, calls3);
        Assert.Equal(new[] { 4 }, calls4);
        Assert.Equal(new[] { 5 }, calls5);
        Assert.Equal(new[] { 6 }, calls6);
    }

    [Fact]
    public void OneOf7_ImplicitConversions_AllPositions_CreateCorrectly()
    {
        // Arrange (Given) & Act (When)
        OneOf<int, string, bool, double, float, long, byte> oneOf1 = 1;
        OneOf<int, string, bool, double, float, long, byte> oneOf2 = "s";
        OneOf<int, string, bool, double, float, long, byte> oneOf3 = true;
        OneOf<int, string, bool, double, float, long, byte> oneOf4 = 4.0;
        OneOf<int, string, bool, double, float, long, byte> oneOf5 = 5f;
        OneOf<int, string, bool, double, float, long, byte> oneOf6 = 6L;
        OneOf<int, string, bool, double, float, long, byte> oneOf7 = (byte)7;

        // Assert (Then)
        Assert.True(oneOf1.IsFirst);
        Assert.True(oneOf2.IsSecond);
        Assert.True(oneOf3.IsThird);
        Assert.True(oneOf4.IsFourth);
        Assert.True(oneOf5.IsFifth);
        Assert.True(oneOf6.IsSixth);
        Assert.True(oneOf7.IsSeventh);
    }

    #endregion

    #region OneOf<T1, T2, T3, T4, T5, T6, T7, T8>

    [Fact]
    public void OneOf8_FromEighth_CreatesEighth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromEighth('Z');

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.False(oneOf.IsSixth);
        Assert.False(oneOf.IsSeventh);
        Assert.True(oneOf.IsEighth);
        Assert.True(oneOf.TryGetEighth(out var result));
        Assert.Equal('Z', result);
    }

    [Fact]
    public void OneOf8_Match_Func_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string, bool, double, float, long, byte, char>.FromFirst(1);
        var oneOfEighth = OneOf<int, string, bool, double, float, long, byte, char>.FromEighth('A');

        // Act (When)
        var result1 = oneOfFirst.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8);
        var result8 = oneOfEighth.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8);

        // Assert (Then)
        Assert.Equal(1, result1);
        Assert.Equal(8, result8);
    }

    [Fact]
    public void OneOf8_Match_Action_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromEighth('X');
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("eighth", calls[0]);
    }

    [Fact]
    public void OneOf8_ImplicitConversion_FromEighth_CreatesEighth()
    {
        // Arrange (Given)
        var value = 'B';

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsEighth);
    }

    [Fact]
    public void OneOf8_FromSecond_Through_Seventh_CreatesCorrectly()
    {
        // Arrange (Given) & Act (When)
        var oneOf2 = OneOf<int, string, bool, double, float, long, byte, char>.FromSecond("test");
        var oneOf3 = OneOf<int, string, bool, double, float, long, byte, char>.FromThird(true);
        var oneOf4 = OneOf<int, string, bool, double, float, long, byte, char>.FromFourth(1.1);
        var oneOf5 = OneOf<int, string, bool, double, float, long, byte, char>.FromFifth(2.2f);
        var oneOf6 = OneOf<int, string, bool, double, float, long, byte, char>.FromSixth(100L);
        var oneOf7 = OneOf<int, string, bool, double, float, long, byte, char>.FromSeventh(128);

        // Assert (Then)
        Assert.True(oneOf2.IsSecond && oneOf2.TryGetSecond(out var v2) && v2 == "test");
        Assert.True(oneOf3.IsThird && oneOf3.TryGetThird(out var v3) && v3);
        Assert.True(oneOf4.IsFourth && oneOf4.TryGetFourth(out var v4) && Math.Abs(v4 - 1.1) < 0.001);
        Assert.True(oneOf5.IsFifth && oneOf5.TryGetFifth(out var v5) && Math.Abs(v5 - 2.2f) < 0.001f);
        Assert.True(oneOf6.IsSixth && oneOf6.TryGetSixth(out var v6) && v6 == 100L);
        Assert.True(oneOf7.IsSeventh && oneOf7.TryGetSeventh(out var v7) && v7 == 128);
    }

    [Fact]
    public void OneOf8_Match_Func_AllBranches_ExecuteCorrectly()
    {
        // Arrange (Given)
        var oneOfs = new[]
        {
            (OneOf<int, string, bool, double, float, long, byte, char>.FromSecond("s"), 2),
            (OneOf<int, string, bool, double, float, long, byte, char>.FromThird(true), 3),
            (OneOf<int, string, bool, double, float, long, byte, char>.FromFourth(4.0), 4),
            (OneOf<int, string, bool, double, float, long, byte, char>.FromFifth(5f), 5),
            (OneOf<int, string, bool, double, float, long, byte, char>.FromSixth(6L), 6),
            (OneOf<int, string, bool, double, float, long, byte, char>.FromSeventh(7), 7)
        };

        // Act & Assert (Then)
        foreach (var (oneOf, expected) in oneOfs)
        {
            var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void OneOf8_Match_Action_AllBranches_ExecuteCorrectly()
    {
        // Arrange (Given)
        var oneOf2 = OneOf<int, string, bool, double, float, long, byte, char>.FromSecond("s");
        var oneOf3 = OneOf<int, string, bool, double, float, long, byte, char>.FromThird(true);
        var oneOf4 = OneOf<int, string, bool, double, float, long, byte, char>.FromFourth(4.0);
        var oneOf5 = OneOf<int, string, bool, double, float, long, byte, char>.FromFifth(5f);
        var oneOf6 = OneOf<int, string, bool, double, float, long, byte, char>.FromSixth(6L);
        var oneOf7 = OneOf<int, string, bool, double, float, long, byte, char>.FromSeventh(7);

        // Act (When)
        var calls2 = new List<int>();
        oneOf2.Match(_ => calls2.Add(1), _ => calls2.Add(2), _ => calls2.Add(3), _ => calls2.Add(4), _ => calls2.Add(5),
            _ => calls2.Add(6), _ => calls2.Add(7), _ => calls2.Add(8));

        var calls3 = new List<int>();
        oneOf3.Match(_ => calls3.Add(1), _ => calls3.Add(2), _ => calls3.Add(3), _ => calls3.Add(4), _ => calls3.Add(5),
            _ => calls3.Add(6), _ => calls3.Add(7), _ => calls3.Add(8));

        var calls4 = new List<int>();
        oneOf4.Match(_ => calls4.Add(1), _ => calls4.Add(2), _ => calls4.Add(3), _ => calls4.Add(4), _ => calls4.Add(5),
            _ => calls4.Add(6), _ => calls4.Add(7), _ => calls4.Add(8));

        var calls5 = new List<int>();
        oneOf5.Match(_ => calls5.Add(1), _ => calls5.Add(2), _ => calls5.Add(3), _ => calls5.Add(4), _ => calls5.Add(5),
            _ => calls5.Add(6), _ => calls5.Add(7), _ => calls5.Add(8));

        var calls6 = new List<int>();
        oneOf6.Match(_ => calls6.Add(1), _ => calls6.Add(2), _ => calls6.Add(3), _ => calls6.Add(4), _ => calls6.Add(5),
            _ => calls6.Add(6), _ => calls6.Add(7), _ => calls6.Add(8));

        var calls7 = new List<int>();
        oneOf7.Match(_ => calls7.Add(1), _ => calls7.Add(2), _ => calls7.Add(3), _ => calls7.Add(4), _ => calls7.Add(5),
            _ => calls7.Add(6), _ => calls7.Add(7), _ => calls7.Add(8));

        // Assert (Then)
        Assert.Equal(new[] { 2 }, calls2);
        Assert.Equal(new[] { 3 }, calls3);
        Assert.Equal(new[] { 4 }, calls4);
        Assert.Equal(new[] { 5 }, calls5);
        Assert.Equal(new[] { 6 }, calls6);
        Assert.Equal(new[] { 7 }, calls7);
    }

    [Fact]
    public void OneOf8_ImplicitConversions_AllPositions_CreateCorrectly()
    {
        // Arrange (Given) & Act (When)
        OneOf<int, string, bool, double, float, long, byte, char> oneOf1 = 1;
        OneOf<int, string, bool, double, float, long, byte, char> oneOf2 = "s";
        OneOf<int, string, bool, double, float, long, byte, char> oneOf3 = true;
        OneOf<int, string, bool, double, float, long, byte, char> oneOf4 = 4.0;
        OneOf<int, string, bool, double, float, long, byte, char> oneOf5 = 5f;
        OneOf<int, string, bool, double, float, long, byte, char> oneOf6 = 6L;
        OneOf<int, string, bool, double, float, long, byte, char> oneOf7 = (byte)7;
        OneOf<int, string, bool, double, float, long, byte, char> oneOf8 = 'Z';

        // Assert (Then)
        Assert.True(oneOf1.IsFirst);
        Assert.True(oneOf2.IsSecond);
        Assert.True(oneOf3.IsThird);
        Assert.True(oneOf4.IsFourth);
        Assert.True(oneOf5.IsFifth);
        Assert.True(oneOf6.IsSixth);
        Assert.True(oneOf7.IsSeventh);
        Assert.True(oneOf8.IsEighth);
    }

    #endregion

    #region OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9>

    [Fact]
    public void OneOf9_FromFirst_CreatesFirst()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFirst(1);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(1, result);
    }

    [Fact]
    public void OneOf9_FromSecond_CreatesSecond()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSecond("test");

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
        Assert.True(oneOf.TryGetSecond(out var result));
        Assert.Equal("test", result);
    }

    [Fact]
    public void OneOf9_FromThird_CreatesThird()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromThird(true);

        // Assert (Then)
        Assert.True(oneOf.IsThird);
        Assert.True(oneOf.TryGetThird(out var result));
        Assert.True(result);
    }

    [Fact]
    public void OneOf9_FromFourth_CreatesFourth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFourth(4.4);

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
        Assert.True(oneOf.TryGetFourth(out var result));
        Assert.Equal(4.4, result);
    }

    [Fact]
    public void OneOf9_FromFifth_CreatesFifth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFifth(5.5f);

        // Assert (Then)
        Assert.True(oneOf.IsFifth);
        Assert.True(oneOf.TryGetFifth(out var result));
        Assert.Equal(5.5f, result);
    }

    [Fact]
    public void OneOf9_FromSixth_CreatesSixth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSixth(666L);

        // Assert (Then)
        Assert.True(oneOf.IsSixth);
        Assert.True(oneOf.TryGetSixth(out var result));
        Assert.Equal(666L, result);
    }

    [Fact]
    public void OneOf9_FromSeventh_CreatesSeventh()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSeventh(77);

        // Assert (Then)
        Assert.True(oneOf.IsSeventh);
        Assert.True(oneOf.TryGetSeventh(out var result));
        Assert.Equal((byte)77, result);
    }

    [Fact]
    public void OneOf9_FromEighth_CreatesEighth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromEighth('X');

        // Assert (Then)
        Assert.True(oneOf.IsEighth);
        Assert.True(oneOf.TryGetEighth(out var result));
        Assert.Equal('X', result);
    }

    [Fact]
    public void OneOf9_FromNinth_CreatesNinth()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromNinth(999);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.False(oneOf.IsSixth);
        Assert.False(oneOf.IsSeventh);
        Assert.False(oneOf.IsEighth);
        Assert.True(oneOf.IsNinth);
        Assert.True(oneOf.TryGetNinth(out var result));
        Assert.Equal((short)999, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithFirst_ExecutesFirstBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFirst(1);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(1, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithSecond_ExecutesSecondBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSecond("test");

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(2, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithThird_ExecutesThirdBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromThird(true);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(3, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithFourth_ExecutesFourthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFourth(4.4);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(4, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithFifth_ExecutesFifthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFifth(5.5f);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(5, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithSixth_ExecutesSixthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSixth(666L);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(6, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithSeventh_ExecutesSeventhBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSeventh(77);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(7, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithEighth_ExecutesEighthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromEighth('X');

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(8, result);
    }

    [Fact]
    public void OneOf9_Match_Func_WithNinth_ExecutesNinthBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromNinth(9);

        // Act (When)
        var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9);

        // Assert (Then)
        Assert.Equal(9, result);
    }

    [Fact]
    public void OneOf9_Match_Action_WithFirst_ExecutesFirstAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFirst(1);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("first", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithSecond_ExecutesSecondAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSecond("test");
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("second", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithThird_ExecutesThirdAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromThird(true);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("third", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithFourth_ExecutesFourthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFourth(4.4);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fourth", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithFifth_ExecutesFifthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFifth(5.5f);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("fifth", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithSixth_ExecutesSixthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSixth(666L);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("sixth", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithSeventh_ExecutesSeventhAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSeventh(77);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("seventh", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithEighth_ExecutesEighthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromEighth('X');
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("eighth", calls[0]);
    }

    [Fact]
    public void OneOf9_Match_Action_WithNinth_ExecutesNinthAction()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromNinth(100);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("ninth", calls[0]);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromSecond_CreatesSecond()
    {
        // Arrange (Given)
        var value = "hello";

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSecond);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromThird_CreatesThird()
    {
        // Arrange (Given)
        var value = true;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsThird);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromFourth_CreatesFourth()
    {
        // Arrange (Given)
        var value = 3.14;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFourth);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromFifth_CreatesFifth()
    {
        // Arrange (Given)
        var value = 2.71f;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFifth);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromSixth_CreatesSixth()
    {
        // Arrange (Given)
        var value = 999L;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSixth);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromSeventh_CreatesSeventh()
    {
        // Arrange (Given)
        byte value = 128;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsSeventh);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromEighth_CreatesEighth()
    {
        // Arrange (Given)
        var value = 'Z';

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsEighth);
    }

    [Fact]
    public void OneOf9_ImplicitConversion_FromNinth_CreatesNinth()
    {
        // Arrange (Given)
        short value = 512;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsNinth);
    }

    [Fact]
    public void OneOf9_TryGet_WhenNotMatching_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFifth(3.14f);

        // Act & Assert (Then)
        Assert.False(oneOf.TryGetFirst(out _));
        Assert.False(oneOf.TryGetSecond(out _));
        Assert.False(oneOf.TryGetThird(out _));
        Assert.False(oneOf.TryGetFourth(out _));
        Assert.True(oneOf.TryGetFifth(out var fifthValue));
        Assert.Equal(3.14f, fifthValue);
        Assert.False(oneOf.TryGetSixth(out _));
        Assert.False(oneOf.TryGetSeventh(out _));
        Assert.False(oneOf.TryGetEighth(out _));
        Assert.False(oneOf.TryGetNinth(out _));
    }

    #endregion

    #region OneOf<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>

    [Fact]
    public void OneOf10_FromFirst_CreatesFirst()
    {
        // Arrange (Given) & Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFirst(1);

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.False(oneOf.IsSixth);
        Assert.False(oneOf.IsSeventh);
        Assert.False(oneOf.IsEighth);
        Assert.False(oneOf.IsNinth);
        Assert.False(oneOf.IsTenth);
        Assert.True(oneOf.TryGetFirst(out var result));
        Assert.Equal(1, result);
    }

    [Fact]
    public void OneOf10_FromTenth_CreatesTenth()
    {
        // Arrange (Given)
        var value = 10.5m;

        // Act (When)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromTenth(value);

        // Assert (Then)
        Assert.False(oneOf.IsFirst);
        Assert.False(oneOf.IsSecond);
        Assert.False(oneOf.IsThird);
        Assert.False(oneOf.IsFourth);
        Assert.False(oneOf.IsFifth);
        Assert.False(oneOf.IsSixth);
        Assert.False(oneOf.IsSeventh);
        Assert.False(oneOf.IsEighth);
        Assert.False(oneOf.IsNinth);
        Assert.True(oneOf.IsTenth);
        Assert.True(oneOf.TryGetTenth(out var result));
        Assert.Equal(10.5m, result);
    }

    [Fact]
    public void OneOf10_Match_Func_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOfFirst = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFirst(1);
        var oneOfFifth = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFifth(5.5f);
        var oneOfTenth = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromTenth(10.0m);

        // Act (When)
        var result1 = oneOfFirst.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9,
            te => 10);
        var result5 = oneOfFifth.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9,
            te => 10);
        var result10 = oneOfTenth.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9,
            te => 10);

        // Assert (Then)
        Assert.Equal(1, result1);
        Assert.Equal(5, result5);
        Assert.Equal(10, result10);
    }

    [Fact]
    public void OneOf10_Match_Action_ExecutesCorrectBranch()
    {
        // Arrange (Given)
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromTenth(99.9m);
        var calls = new List<string>();

        // Act (When)
        oneOf.Match(
            f => calls.Add("first"),
            s => calls.Add("second"),
            t => calls.Add("third"),
            fo => calls.Add("fourth"),
            fi => calls.Add("fifth"),
            si => calls.Add("sixth"),
            se => calls.Add("seventh"),
            ei => calls.Add("eighth"),
            ni => calls.Add("ninth"),
            te => calls.Add("tenth"));

        // Assert (Then)
        Assert.Single(calls);
        Assert.Equal("tenth", calls[0]);
    }

    [Fact]
    public void OneOf10_ImplicitConversion_FromFirst_CreatesFirst()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsFirst);
    }

    [Fact]
    public void OneOf10_ImplicitConversion_FromTenth_CreatesTenth()
    {
        // Arrange (Given)
        var value = 10.0m;

        // Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf = value;

        // Assert (Then)
        Assert.True(oneOf.IsTenth);
    }

    [Fact]
    public void OneOf10_TryGet_AllPositions_WorkCorrectly()
    {
        // Arrange (Given)
        var oneOfFifth = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFifth(3.14f);

        // Act & Assert (Then)
        Assert.False(oneOfFifth.TryGetFirst(out _));
        Assert.False(oneOfFifth.TryGetSecond(out _));
        Assert.False(oneOfFifth.TryGetThird(out _));
        Assert.False(oneOfFifth.TryGetFourth(out _));
        Assert.True(oneOfFifth.TryGetFifth(out var fifthValue));
        Assert.Equal(3.14f, fifthValue);
        Assert.False(oneOfFifth.TryGetSixth(out _));
        Assert.False(oneOfFifth.TryGetSeventh(out _));
        Assert.False(oneOfFifth.TryGetEighth(out _));
        Assert.False(oneOfFifth.TryGetNinth(out _));
        Assert.False(oneOfFifth.TryGetTenth(out _));
    }

    [Fact]
    public void OneOf10_FromSecond_Through_Ninth_CreatesCorrectly()
    {
        // Arrange (Given) & Act (When)
        var oneOf2 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSecond("test");
        var oneOf3 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromThird(true);
        var oneOf4 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFourth(1.1);
        var oneOf5 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFifth(2.2f);
        var oneOf6 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSixth(100L);
        var oneOf7 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSeventh(128);
        var oneOf8 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromEighth('X');
        var oneOf9 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromNinth(500);

        // Assert (Then)
        Assert.True(oneOf2.IsSecond && oneOf2.TryGetSecond(out var v2) && v2 == "test");
        Assert.True(oneOf3.IsThird && oneOf3.TryGetThird(out var v3) && v3);
        Assert.True(oneOf4.IsFourth && oneOf4.TryGetFourth(out var v4) && Math.Abs(v4 - 1.1) < 0.001);
        Assert.True(oneOf5.IsFifth && oneOf5.TryGetFifth(out var v5) && Math.Abs(v5 - 2.2f) < 0.001f);
        Assert.True(oneOf6.IsSixth && oneOf6.TryGetSixth(out var v6) && v6 == 100L);
        Assert.True(oneOf7.IsSeventh && oneOf7.TryGetSeventh(out var v7) && v7 == 128);
        Assert.True(oneOf8.IsEighth && oneOf8.TryGetEighth(out var v8) && v8 == 'X');
        Assert.True(oneOf9.IsNinth && oneOf9.TryGetNinth(out var v9) && v9 == 500);
    }

    [Fact]
    public void OneOf10_Match_Func_AllBranches_ExecuteCorrectly()
    {
        // Arrange (Given)
        var oneOfs = new[]
        {
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSecond("s"), 2),
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromThird(true), 3),
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFourth(4.0), 4),
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSixth(6L), 6),
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSeventh(7), 7),
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromEighth('E'), 8),
            (OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromNinth(9), 9)
        };

        // Act & Assert (Then)
        foreach (var (oneOf, expected) in oneOfs)
        {
            var result = oneOf.Match(f => 1, s => 2, t => 3, fo => 4, fi => 5, si => 6, se => 7, ei => 8, ni => 9,
                te => 10);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void OneOf10_Match_Action_AllBranches_ExecuteCorrectly()
    {
        // Arrange (Given)
        var oneOf2 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSecond("s");
        var oneOf3 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromThird(true);
        var oneOf4 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFourth(4.0);
        var oneOf6 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSixth(6L);
        var oneOf7 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSeventh(7);
        var oneOf8 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromEighth('A');
        var oneOf9 = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromNinth(9);

        // Act (When)
        var calls2 = new List<int>();
        oneOf2.Match(_ => calls2.Add(1), _ => calls2.Add(2), _ => calls2.Add(3), _ => calls2.Add(4), _ => calls2.Add(5),
            _ => calls2.Add(6), _ => calls2.Add(7), _ => calls2.Add(8), _ => calls2.Add(9), _ => calls2.Add(10));

        var calls3 = new List<int>();
        oneOf3.Match(_ => calls3.Add(1), _ => calls3.Add(2), _ => calls3.Add(3), _ => calls3.Add(4), _ => calls3.Add(5),
            _ => calls3.Add(6), _ => calls3.Add(7), _ => calls3.Add(8), _ => calls3.Add(9), _ => calls3.Add(10));

        var calls4 = new List<int>();
        oneOf4.Match(_ => calls4.Add(1), _ => calls4.Add(2), _ => calls4.Add(3), _ => calls4.Add(4), _ => calls4.Add(5),
            _ => calls4.Add(6), _ => calls4.Add(7), _ => calls4.Add(8), _ => calls4.Add(9), _ => calls4.Add(10));

        var calls6 = new List<int>();
        oneOf6.Match(_ => calls6.Add(1), _ => calls6.Add(2), _ => calls6.Add(3), _ => calls6.Add(4), _ => calls6.Add(5),
            _ => calls6.Add(6), _ => calls6.Add(7), _ => calls6.Add(8), _ => calls6.Add(9), _ => calls6.Add(10));

        var calls7 = new List<int>();
        oneOf7.Match(_ => calls7.Add(1), _ => calls7.Add(2), _ => calls7.Add(3), _ => calls7.Add(4), _ => calls7.Add(5),
            _ => calls7.Add(6), _ => calls7.Add(7), _ => calls7.Add(8), _ => calls7.Add(9), _ => calls7.Add(10));

        var calls8 = new List<int>();
        oneOf8.Match(_ => calls8.Add(1), _ => calls8.Add(2), _ => calls8.Add(3), _ => calls8.Add(4), _ => calls8.Add(5),
            _ => calls8.Add(6), _ => calls8.Add(7), _ => calls8.Add(8), _ => calls8.Add(9), _ => calls8.Add(10));

        var calls9 = new List<int>();
        oneOf9.Match(_ => calls9.Add(1), _ => calls9.Add(2), _ => calls9.Add(3), _ => calls9.Add(4), _ => calls9.Add(5),
            _ => calls9.Add(6), _ => calls9.Add(7), _ => calls9.Add(8), _ => calls9.Add(9), _ => calls9.Add(10));

        // Assert (Then)
        Assert.Equal(new[] { 2 }, calls2);
        Assert.Equal(new[] { 3 }, calls3);
        Assert.Equal(new[] { 4 }, calls4);
        Assert.Equal(new[] { 6 }, calls6);
        Assert.Equal(new[] { 7 }, calls7);
        Assert.Equal(new[] { 8 }, calls8);
        Assert.Equal(new[] { 9 }, calls9);
    }

    [Fact]
    public void OneOf10_ImplicitConversions_AllPositions_CreateCorrectly()
    {
        // Arrange (Given) & Act (When)
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf1 = 1;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf2 = "s";
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf3 = true;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf4 = 4.0;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf5 = 5f;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf6 = 6L;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf7 = (byte)7;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf8 = 'Z';
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf9 = (short)9;
        OneOf<int, string, bool, double, float, long, byte, char, short, decimal> oneOf10 = 10m;

        // Assert (Then)
        Assert.True(oneOf1.IsFirst);
        Assert.True(oneOf2.IsSecond);
        Assert.True(oneOf3.IsThird);
        Assert.True(oneOf4.IsFourth);
        Assert.True(oneOf5.IsFifth);
        Assert.True(oneOf6.IsSixth);
        Assert.True(oneOf7.IsSeventh);
        Assert.True(oneOf8.IsEighth);
        Assert.True(oneOf9.IsNinth);
        Assert.True(oneOf10.IsTenth);
    }

    #endregion
}