using System.Diagnostics;
using System.Reflection;

namespace UnambitiousFx.Functional.Tests;

public sealed class OneOfTests_DebuggerDisplay
{
    [Fact]
    public void DebuggerDisplay_OneOf2_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string>.FromFirst(42);
        var oneOf2 = OneOf<int, string>.FromSecond("hello");

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display2 = GetDebuggerDisplay(oneOf2);

        // Assert
        Assert.Equal("First(42)", display1);
        Assert.Equal("Second(hello)", display2);
    }

    [Fact]
    public void DebuggerDisplay_OneOf3_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool>.FromFirst(42);
        var oneOf2 = OneOf<int, string, bool>.FromSecond("hello");
        var oneOf3 = OneOf<int, string, bool>.FromThird(true);

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display2 = GetDebuggerDisplay(oneOf2);
        var display3 = GetDebuggerDisplay(oneOf3);

        // Assert
        Assert.Equal("First(42)", display1);
        Assert.Equal("Second(hello)", display2);
        Assert.Equal("Third(True)", display3);
    }

    [Fact]
    public void DebuggerDisplay_OneOf4_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool, double>.FromFirst(42);
        var oneOf2 = OneOf<int, string, bool, double>.FromSecond("test");
        var oneOf3 = OneOf<int, string, bool, double>.FromThird(false);
        var oneOf4 = OneOf<int, string, bool, double>.FromFourth(5.0);

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display2 = GetDebuggerDisplay(oneOf2);
        var display3 = GetDebuggerDisplay(oneOf3);
        var display4 = GetDebuggerDisplay(oneOf4);

        // Assert
        Assert.Equal("First(42)", display1);
        Assert.Equal("Second(test)", display2);
        Assert.Equal("Third(False)", display3);
        Assert.Equal($"Fourth({5.0})", display4);
    }

    [Fact]
    public void DebuggerDisplay_OneOf5_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool, double, float>.FromFirst(1);
        var oneOf5 = OneOf<int, string, bool, double, float>.FromFifth(2.0f);

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display5 = GetDebuggerDisplay(oneOf5);

        // Assert
        Assert.Equal("First(1)", display1);
        Assert.Equal($"Fifth({2.0f})", display5);
    }

    [Fact]
    public void DebuggerDisplay_OneOf6_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool, double, float, long>.FromFirst(100);
        var oneOf6 = OneOf<int, string, bool, double, float, long>.FromSixth(999L);

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display6 = GetDebuggerDisplay(oneOf6);

        // Assert
        Assert.Equal("First(100)", display1);
        Assert.Equal("Sixth(999)", display6);
    }

    [Fact]
    public void DebuggerDisplay_OneOf7_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool, double, float, long, byte>.FromFirst(1);
        var oneOf7 = OneOf<int, string, bool, double, float, long, byte>.FromSeventh(255);

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display7 = GetDebuggerDisplay(oneOf7);

        // Assert
        Assert.Equal("First(1)", display1);
        Assert.Equal("Seventh(255)", display7);
    }

    [Fact]
    public void DebuggerDisplay_OneOf8_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool, double, float, long, byte, char>.FromFirst(42);
        var oneOf8 = OneOf<int, string, bool, double, float, long, byte, char>.FromEighth('Z');

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display8 = GetDebuggerDisplay(oneOf8);

        // Assert
        Assert.Equal("First(42)", display1);
        Assert.Equal("Eighth(Z)", display8);
    }

    [Fact]
    public void DebuggerDisplay_OneOf9_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFirst(1);
        var oneOf9 = OneOf<int, string, bool, double, float, long, byte, char, short>.FromNinth(999);

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);
        var display9 = GetDebuggerDisplay(oneOf9);

        // Assert
        Assert.Equal("First(1)", display1);
        Assert.Equal("Ninth(999)", display9);
    }

    [Fact]
    public void DebuggerDisplay_OneOf10_ReturnsCorrectString()
    {
        // Arrange
        var oneOf1 = OneOf<int, int, int, int, int, int, int, int, int, string>.FromTenth("last");

        // Act
        var display1 = GetDebuggerDisplay(oneOf1);

        // Assert
        Assert.Equal("Tenth(last)", display1);
    }

    [Fact]
    public void DebugView_OneOf2_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string>.FromSecond("test");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.False(isFirst);
        Assert.True(isSecond);
        Assert.Equal("test", secondValue);
    }

    [Fact]
    public void DebugView_OneOf3_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool>.FromThird(true);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(true, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf4_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double>.FromFourth(3.14);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(3.14, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf5_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float>.FromFifth(2.5f);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFifth = (bool?)debugView?.GetType().GetProperty("IsFifth")?.GetValue(debugView);
        var fifthValue = debugView?.GetType().GetProperty("FifthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fifth", activeType);
        Assert.True(isFifth);
        Assert.Equal(2.5f, fifthValue);
    }

    [Fact]
    public void DebugView_OneOf6_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSixth(100L);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSixth = (bool?)debugView?.GetType().GetProperty("IsSixth")?.GetValue(debugView);
        var sixthValue = debugView?.GetType().GetProperty("SixthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Sixth", activeType);
        Assert.True(isSixth);
        Assert.Equal(100L, sixthValue);
    }

    [Fact]
    public void DebugView_OneOf7_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromSeventh(255);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSeventh = (bool?)debugView?.GetType().GetProperty("IsSeventh")?.GetValue(debugView);
        var seventhValue = debugView?.GetType().GetProperty("SeventhValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Seventh", activeType);
        Assert.True(isSeventh);
        Assert.Equal((byte)255, seventhValue);
    }

    [Fact]
    public void DebugView_OneOf8_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromEighth('A');

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isEighth = (bool?)debugView?.GetType().GetProperty("IsEighth")?.GetValue(debugView);
        var eighthValue = debugView?.GetType().GetProperty("EighthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Eighth", activeType);
        Assert.True(isEighth);
        Assert.Equal('A', eighthValue);
    }

    [Fact]
    public void DebugView_OneOf9_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromNinth(999);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isNinth = (bool?)debugView?.GetType().GetProperty("IsNinth")?.GetValue(debugView);
        var ninthValue = debugView?.GetType().GetProperty("NinthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Ninth", activeType);
        Assert.True(isNinth);
        Assert.Equal((short)999, ninthValue);
    }

    [Fact]
    public void DebugView_OneOf10_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromTenth(99.9m);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isTenth = (bool?)debugView?.GetType().GetProperty("IsTenth")?.GetValue(debugView);
        var tenthValue = debugView?.GetType().GetProperty("TenthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Tenth", activeType);
        Assert.True(isTenth);
        Assert.Equal(99.9m, tenthValue);
    }

    [Fact]
    public void DebugView_OneOf2_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string>.FromFirst(100);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(100, firstValue);
    }

    [Fact]
    public void DebugView_OneOf3_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool>.FromFirst(42);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(42, firstValue);
    }

    [Fact]
    public void DebugView_OneOf3_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool>.FromSecond("hello");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("hello", secondValue);
    }

    [Fact]
    public void DebugView_OneOf4_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double>.FromFirst(123);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(123, firstValue);
    }

    [Fact]
    public void DebugView_OneOf4_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double>.FromSecond("world");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("world", secondValue);
    }

    [Fact]
    public void DebugView_OneOf4_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double>.FromThird(false);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(false, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf5_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float>.FromFirst(999);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(999, firstValue);
    }

    [Fact]
    public void DebugView_OneOf5_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float>.FromSecond("test");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("test", secondValue);
    }

    [Fact]
    public void DebugView_OneOf5_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float>.FromThird(true);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(true, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf5_WithFourthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float>.FromFourth(1.23);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(1.23, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf6_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFirst(111);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(111, firstValue);
    }

    [Fact]
    public void DebugView_OneOf6_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long>.FromSecond("six");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("six", secondValue);
    }

    [Fact]
    public void DebugView_OneOf6_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long>.FromThird(false);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(false, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf6_WithFourthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFourth(6.66);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(6.66, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf6_WithFifthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long>.FromFifth(5.5f);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFifth = (bool?)debugView?.GetType().GetProperty("IsFifth")?.GetValue(debugView);
        var fifthValue = debugView?.GetType().GetProperty("FifthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fifth", activeType);
        Assert.True(isFifth);
        Assert.Equal(5.5f, fifthValue);
    }

    [Fact]
    public void DebugView_OneOf7_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromFirst(777);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(777, firstValue);
    }

    [Fact]
    public void DebugView_OneOf7_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromSecond("seven");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("seven", secondValue);
    }

    [Fact]
    public void DebugView_OneOf7_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromThird(true);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(true, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf7_WithFourthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromFourth(7.77);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(7.77, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf7_WithFifthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromFifth(5.7f);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFifth = (bool?)debugView?.GetType().GetProperty("IsFifth")?.GetValue(debugView);
        var fifthValue = debugView?.GetType().GetProperty("FifthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fifth", activeType);
        Assert.True(isFifth);
        Assert.Equal(5.7f, fifthValue);
    }

    [Fact]
    public void DebugView_OneOf7_WithSixthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte>.FromSixth(777L);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSixth = (bool?)debugView?.GetType().GetProperty("IsSixth")?.GetValue(debugView);
        var sixthValue = debugView?.GetType().GetProperty("SixthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Sixth", activeType);
        Assert.True(isSixth);
        Assert.Equal(777L, sixthValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromFirst(888);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(888, firstValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromSecond("eight");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("eight", secondValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromThird(false);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(false, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithFourthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromFourth(8.88);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(8.88, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithFifthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromFifth(5.8f);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFifth = (bool?)debugView?.GetType().GetProperty("IsFifth")?.GetValue(debugView);
        var fifthValue = debugView?.GetType().GetProperty("FifthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fifth", activeType);
        Assert.True(isFifth);
        Assert.Equal(5.8f, fifthValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithSixthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromSixth(888L);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSixth = (bool?)debugView?.GetType().GetProperty("IsSixth")?.GetValue(debugView);
        var sixthValue = debugView?.GetType().GetProperty("SixthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Sixth", activeType);
        Assert.True(isSixth);
        Assert.Equal(888L, sixthValue);
    }

    [Fact]
    public void DebugView_OneOf8_WithSeventhValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char>.FromSeventh(88);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSeventh = (bool?)debugView?.GetType().GetProperty("IsSeventh")?.GetValue(debugView);
        var seventhValue = debugView?.GetType().GetProperty("SeventhValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Seventh", activeType);
        Assert.True(isSeventh);
        Assert.Equal((byte)88, seventhValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFirst(999);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(999, firstValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSecond("nine");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("nine", secondValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromThird(true);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(true, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithFourthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFourth(9.99);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(9.99, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithFifthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromFifth(5.9f);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFifth = (bool?)debugView?.GetType().GetProperty("IsFifth")?.GetValue(debugView);
        var fifthValue = debugView?.GetType().GetProperty("FifthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fifth", activeType);
        Assert.True(isFifth);
        Assert.Equal(5.9f, fifthValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithSixthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSixth(999L);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSixth = (bool?)debugView?.GetType().GetProperty("IsSixth")?.GetValue(debugView);
        var sixthValue = debugView?.GetType().GetProperty("SixthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Sixth", activeType);
        Assert.True(isSixth);
        Assert.Equal(999L, sixthValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithSeventhValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromSeventh(99);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSeventh = (bool?)debugView?.GetType().GetProperty("IsSeventh")?.GetValue(debugView);
        var seventhValue = debugView?.GetType().GetProperty("SeventhValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Seventh", activeType);
        Assert.True(isSeventh);
        Assert.Equal((byte)99, seventhValue);
    }

    [Fact]
    public void DebugView_OneOf9_WithEighthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short>.FromEighth('N');

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isEighth = (bool?)debugView?.GetType().GetProperty("IsEighth")?.GetValue(debugView);
        var eighthValue = debugView?.GetType().GetProperty("EighthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Eighth", activeType);
        Assert.True(isEighth);
        Assert.Equal('N', eighthValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithFirstValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFirst(1000);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFirst = (bool?)debugView?.GetType().GetProperty("IsFirst")?.GetValue(debugView);
        var firstValue = debugView?.GetType().GetProperty("FirstValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("First", activeType);
        Assert.True(isFirst);
        Assert.Equal(1000, firstValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithSecondValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSecond("ten");

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSecond = (bool?)debugView?.GetType().GetProperty("IsSecond")?.GetValue(debugView);
        var secondValue = debugView?.GetType().GetProperty("SecondValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Second", activeType);
        Assert.True(isSecond);
        Assert.Equal("ten", secondValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithThirdValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromThird(false);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isThird = (bool?)debugView?.GetType().GetProperty("IsThird")?.GetValue(debugView);
        var thirdValue = debugView?.GetType().GetProperty("ThirdValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Third", activeType);
        Assert.True(isThird);
        Assert.Equal(false, thirdValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithFourthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFourth(10.10);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFourth = (bool?)debugView?.GetType().GetProperty("IsFourth")?.GetValue(debugView);
        var fourthValue = debugView?.GetType().GetProperty("FourthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fourth", activeType);
        Assert.True(isFourth);
        Assert.Equal(10.10, fourthValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithFifthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromFifth(5.10f);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isFifth = (bool?)debugView?.GetType().GetProperty("IsFifth")?.GetValue(debugView);
        var fifthValue = debugView?.GetType().GetProperty("FifthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Fifth", activeType);
        Assert.True(isFifth);
        Assert.Equal(5.10f, fifthValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithSixthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSixth(1000L);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSixth = (bool?)debugView?.GetType().GetProperty("IsSixth")?.GetValue(debugView);
        var sixthValue = debugView?.GetType().GetProperty("SixthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Sixth", activeType);
        Assert.True(isSixth);
        Assert.Equal(1000L, sixthValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithSeventhValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromSeventh(100);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isSeventh = (bool?)debugView?.GetType().GetProperty("IsSeventh")?.GetValue(debugView);
        var seventhValue = debugView?.GetType().GetProperty("SeventhValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Seventh", activeType);
        Assert.True(isSeventh);
        Assert.Equal((byte)100, seventhValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithEighthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromEighth('T');

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isEighth = (bool?)debugView?.GetType().GetProperty("IsEighth")?.GetValue(debugView);
        var eighthValue = debugView?.GetType().GetProperty("EighthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Eighth", activeType);
        Assert.True(isEighth);
        Assert.Equal('T', eighthValue);
    }

    [Fact]
    public void DebugView_OneOf10_WithNinthValue_ReturnsCorrectProperties()
    {
        // Arrange
        var oneOf = OneOf<int, string, bool, double, float, long, byte, char, short, decimal>.FromNinth(1000);

        // Act
        var debugView = GetDebugView(oneOf);
        var activeType = debugView?.GetType().GetProperty("ActiveType")?.GetValue(debugView) as string;
        var isNinth = (bool?)debugView?.GetType().GetProperty("IsNinth")?.GetValue(debugView);
        var ninthValue = debugView?.GetType().GetProperty("NinthValue")?.GetValue(debugView);

        // Assert
        Assert.Equal("Ninth", activeType);
        Assert.True(isNinth);
        Assert.Equal((short)1000, ninthValue);
    }

    private static string? GetDebuggerDisplay<T>(T value)
    {
        var property = typeof(T).GetProperty("DebuggerDisplay", BindingFlags.Instance | BindingFlags.NonPublic);
        return property?.GetValue(value) as string;
    }

    private static object? GetDebugView<T>(T value)
    {
        var debuggerTypeProxyAttribute = typeof(T).GetCustomAttribute<DebuggerTypeProxyAttribute>();
        if (debuggerTypeProxyAttribute == null)
        {
            return null;
        }

        var proxyTypeName = debuggerTypeProxyAttribute.ProxyTypeName;
        var proxyTypeGenericName = proxyTypeName.Split(',')[0].Split('.').Last().Split('`')[0];

        // Get the generic arguments from the OneOf type
        var genericArgs = typeof(T).GetGenericArguments();
        var arity = genericArgs.Length;

        // Find the proxy type with the correct arity
        var proxyTypeGeneric = typeof(T).Assembly.GetTypes()
            .FirstOrDefault(t =>
                t.Name == $"{proxyTypeGenericName}`{arity}" && t.GetGenericArguments().Length == arity);

        if (proxyTypeGeneric == null)
        {
            return null;
        }

        var proxyType = proxyTypeGeneric.MakeGenericType(genericArgs);

        return Activator.CreateInstance(proxyType, value);
    }
}
