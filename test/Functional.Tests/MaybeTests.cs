using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeTests
{
    #region None

    [Fact]
    public void None_ReturnsNone()
    {
        // Arrange (Given)

        // Act (When)
        var maybe = Maybe.None<int>();

        // Assert (Then)
        maybe.ShouldBe()
            .None();
    }

    #endregion

    #region Implicit Conversion

    [Fact]
    public void ImplicitConversion_WithValue_ReturnsSome()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        Maybe<int> maybe = value;

        // Assert (Then)
        maybe.ShouldBe()
            .Some()
            .And(v => Assert.Equal(value, v));
    }

    #endregion

    private record TestRecord(string Name);

    #region Some

    [Fact]
    public void Some_WithValue_ReturnsSome()
    {
        // Arrange (Given)
        var value = 42;

        // Act (When)
        var maybe = Maybe.Some(value);

        // Assert (Then)
        maybe.ShouldBe()
            .Some()
            .And(v => Assert.Equal(value, v));
    }

    [Fact]
    public void Some_WithRecord_ReturnsSome()
    {
        // Arrange (Given)
        var value = new TestRecord("test");

        // Act (When)
        var maybe = Maybe.Some(value);

        // Assert (Then)
        maybe.ShouldBe()
            .Some()
            .And(v => Assert.Equal(value, v));
    }

    #endregion

    #region IsSome / IsNone

    [Fact]
    public void IsSome_WithSome_ReturnsTrue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var isSome = maybe.IsSome;

        // Assert (Then)
        Assert.True(isSome);
    }

    [Fact]
    public void IsSome_WithNone_ReturnsFalse()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var isSome = maybe.IsSome;

        // Assert (Then)
        Assert.False(isSome);
    }

    [Fact]
    public void IsNone_WithSome_ReturnsFalse()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var isNone = maybe.IsNone;

        // Assert (Then)
        Assert.False(isNone);
    }

    [Fact]
    public void IsNone_WithNone_ReturnsTrue()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var isNone = maybe.IsNone;

        // Assert (Then)
        Assert.True(isNone);
    }

    #endregion

    #region Case

    [Fact]
    public void Case_WithSome_ReturnsValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var value = maybe.Case;

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public void Case_WithNone_ReturnsDefault()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var value = maybe.Case;

        // Assert (Then)
        Assert.Equal(default, value);
    }

    #endregion

    #region IfSome

    [Fact]
    public void IfSome_WithSome_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var executed = false;
        var result = 0;

        // Act (When)
        maybe.IfSome(v =>
        {
            executed = true;
            result = v;
        });

        // Assert (Then)
        Assert.True(executed);
        Assert.Equal(42, result);
    }

    [Fact]
    public void IfSome_WithNone_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        maybe.IfSome(_ => executed = true);

        // Assert (Then)
        Assert.False(executed);
    }

    [Fact]
    public async Task IfSome_Async_WithSome_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var executed = false;
        var result = 0;

        // Act (When)
        await maybe.IfSome(v =>
        {
            executed = true;
            result = v;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        Assert.True(executed);
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task IfSome_Async_WithNone_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        await maybe.IfSome(_ =>
        {
            executed = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        Assert.False(executed);
    }

    #endregion

    #region IfNone

    [Fact]
    public void IfNone_WithSome_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var executed = false;

        // Act (When)
        maybe.IfNone(() => executed = true);

        // Assert (Then)
        Assert.False(executed);
    }

    [Fact]
    public void IfNone_WithNone_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        maybe.IfNone(() => executed = true);

        // Assert (Then)
        Assert.True(executed);
    }

    [Fact]
    public async Task IfNone_Async_WithSome_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var executed = false;

        // Act (When)
        await maybe.IfNone(() =>
        {
            executed = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        Assert.False(executed);
    }

    [Fact]
    public async Task IfNone_Async_WithNone_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var executed = false;

        // Act (When)
        await maybe.IfNone(() =>
        {
            executed = true;
            return ValueTask.CompletedTask;
        });

        // Assert (Then)
        Assert.True(executed);
    }

    #endregion

    #region TryGetValue (Some out)

    [Fact]
    public void Some_Out_WithSome_ReturnsTrueAndValue()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var result = maybe.Some(out var value);

        // Assert (Then)
        Assert.True(result);
        Assert.Equal(42, value);
    }

    [Fact]
    public void Some_Out_WithNone_ReturnsFalseAndDefault()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var result = maybe.Some(out var value);

        // Assert (Then)
        Assert.False(result);
        Assert.Equal(default, value);
    }

    #endregion

    #region Match

    [Fact]
    public void Match_Func_WithSome_ReturnsSomeResult()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);

        // Act (When)
        var result = maybe.Match(
            v => v * 2,
            () => 0);

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public void Match_Func_WithNone_ReturnsNoneResult()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();

        // Act (When)
        var result = maybe.Match(
            v => v * 2,
            () => -1);

        // Assert (Then)
        Assert.Equal(-1, result);
    }

    [Fact]
    public void Match_Action_WithSome_ExecutesSomeAction()
    {
        // Arrange (Given)
        var maybe = Maybe.Some(42);
        var someExecuted = false;
        var noneExecuted = false;
        var result = 0;

        // Act (When)
        maybe.Match(
            v =>
            {
                someExecuted = true;
                result = v;
            },
            () => noneExecuted = true);

        // Assert (Then)
        Assert.True(someExecuted);
        Assert.False(noneExecuted);
        Assert.Equal(42, result);
    }

    [Fact]
    public void Match_Action_WithNone_ExecutesNoneAction()
    {
        // Arrange (Given)
        var maybe = Maybe.None<int>();
        var someExecuted = false;
        var noneExecuted = false;

        // Act (When)
        maybe.Match(
            _ => someExecuted = true,
            () =>
            {
                noneExecuted = true;
            });

        // Assert (Then)
        Assert.False(someExecuted);
        Assert.True(noneExecuted);
    }

    #endregion
}
