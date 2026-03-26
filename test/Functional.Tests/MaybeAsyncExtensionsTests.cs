using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class MaybeAsyncExtensionsTests
{
    [Fact]
    public async Task ValueTaskMaybe_Pipeline_CanBindMapTapSomeTapNoneWithoutWrapper()
    {
        // Arrange (Given)
        var start   = ValueTask.FromResult(Maybe.Some(2));
        var seen    = 0;
        var noneHit = false;

        // Act (When)
        var maybe = await start
                         .Bind(v => ValueTask.FromResult(Maybe.Some(v + 1)))
                         .Map(v => v * 10)
                         .TapSome(v => seen = v)
                         .TapNone(() => noneHit = true);

        // Assert (Then)
        maybe.ShouldBe()
             .Some()
             .And(v => Assert.Equal(30, v));
        Assert.Equal(30, seen);
        Assert.False(noneHit);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_UsesDirectAsyncExtensions()
    {
        // Arrange (Given)
        var start = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await start.ToResult(new ValidationFailure("missing"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    // Filter Tests
    [Fact]
    public async Task ValueTaskMaybe_Filter_WithPredicateTrue_ReturnsSome()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(x => x > 40);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithPredicateFalse_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(x => x > 50);

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithAsyncPredicate_FiltersCorrectly()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x =>
        {
            await Task.Delay(1);
            return x > 40;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithPredicate_FiltersCorrectly()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(x => x > 40);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    // Map async Tests
    [Fact]
    public async Task ValueTaskMaybe_Map_WithAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(5));

        // Act (When)
        var result = await maybe.Map(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(10, v));
    }

    [Fact]
    public async Task TaskMaybe_Map_WithAsyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(5));

        // Act (When)
        var result = await maybe.Map(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(10, v));
    }

    // ValueOr Tests
    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithSome_ReturnsValue()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ValueOr(0);

        // Assert (Then)
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithNone_ReturnsFallback()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithFactory_ComputesFallback()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithAsyncFactory_ComputesAsyncFallback()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(async () =>
        {
            await Task.Delay(1);
            return 99;
        });

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithAsyncFactory_ComputesAsyncFallback()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(async () =>
        {
            await Task.Delay(1);
            return 99;
        });

        // Assert (Then)
        Assert.Equal(99, result);
    }

    // OrElse Tests
    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithSome_ReturnsOriginal()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithNone_ReturnsFallback()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithFactory_ComputesFallback()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(() => Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithAsyncFactory_ComputesAsyncFallback()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(async () =>
        {
            await Task.Delay(1);
            return Maybe.Some(99);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithAsyncFactory_ComputesAsyncFallback()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(async () =>
        {
            await Task.Delay(1);
            return Maybe.Some(99);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    // Match Tests
    [Fact]
    public async Task ValueTaskMaybe_Match_WithSome_ReturnsSomeResult()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
            some: x => x * 2,
            none: () => 0);

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_Match_WithNone_ReturnsNoneResult()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
            some: x => x * 2,
            none: () => 0);

        // Assert (Then)
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_Match_WithAsyncFunctions_ReturnsSomeResult()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
            some: async x =>
            {
                await Task.Delay(1);
                return x * 2;
            },
            none: async () =>
            {
                await Task.Delay(1);
                return 0;
            });

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithAsyncFunctions_ReturnsNoneResult()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
            some: async x =>
            {
                await Task.Delay(1);
                return x * 2;
            },
            none: async () =>
            {
                await Task.Delay(1);
                return 0;
            });

        // Assert (Then)
        Assert.Equal(0, result);
    }

    // ToResult overload Tests
    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithFactory_CreatesFailureFromFactory()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(() => new ValidationFailure("error"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithMessage_CreatesFailureFromMessage()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult("Value not found");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value not found");
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithAsyncFactory_CreatesFailureFromAsyncFactory()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(async () =>
        {
            await Task.Delay(1);
            return new ValidationFailure("async error");
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithMessage_CreatesFailureFromMessage()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult("Value not found");

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("Value not found");
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithAsyncFactory_CreatesFailureFromAsyncFactory()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(async () =>
        {
            await Task.Delay(1);
            return new ValidationFailure("async error");
        });

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));
        var error = new Failure("ERROR", "Should not be used");

        // Act (When)
        var result = await maybe.ToResult(error);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_ToResult_WithFactory_SomeDoesNotCallFactory()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));
        var factoryCalled = false;

        // Act (When)
        var result = await maybe.ToResult(() =>
        {
            factoryCalled = true;
            return new Failure("ERROR", "Should not be used");
        });

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
        Assert.False(factoryCalled);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithFactory_UsesFactory()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ToResult(() => new ValidationFailure("missing"));

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode(FailureCodes.Validation);
    }

    [Fact]
    public async Task TaskMaybe_ToResult_WithSome_ReturnsSuccess()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ToResult(new Failure("ERROR", "Should not be used"));

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(42, v));
    }

    // Additional Task-based tests for completeness
    [Fact]
    public async Task TaskMaybe_Bind_WithAsyncSelector_ChainsCorrectly()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(10));

        // Act (When)
        var result = await maybe.Bind(async x =>
        {
            await Task.Delay(1);
            return Maybe.Some(x * 2);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task TaskMaybe_Map_WithSyncMapper_TransformsValue()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(10));

        // Act (When)
        var result = await maybe.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task TaskMaybe_TapSome_WithSyncAction_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));
        var seenValue = 0;

        // Act (When)
        var result = await maybe.TapSome(x => seenValue = x);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, seenValue);
    }

    [Fact]
    public async Task TaskMaybe_TapSome_WithAsyncAction_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));
        var seenValue = 0;

        // Act (When)
        var result = await maybe.TapSome(async x =>
        {
            await Task.Delay(1);
            seenValue = x;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.Equal(42, seenValue);
    }

    [Fact]
    public async Task TaskMaybe_TapNone_WithSyncAction_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(() => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe().None();
        Assert.True(actionExecuted);
    }

    [Fact]
    public async Task TaskMaybe_TapNone_WithAsyncAction_ExecutesAction()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(async () =>
        {
            await Task.Delay(1);
            actionExecuted = true;
        });

        // Assert (Then)
        result.ShouldBe().None();
        Assert.True(actionExecuted);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithSyncFunctions_MatchesSome()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
            some: x => x * 2,
            none: () => 0
        );

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithSyncFunctions_MatchesNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
            some: x => x * 2,
            none: () => -1
        );

        // Assert (Then)
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithAsyncFunctions_MatchesSome()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Match(
            some: async x =>
            {
                await Task.Delay(1);
                return x * 2;
            },
            none: async () =>
            {
                await Task.Delay(1);
                return 0;
            }
        );

        // Assert (Then)
        Assert.Equal(84, result);
    }

    [Fact]
    public async Task TaskMaybe_Match_WithAsyncFunctions_MatchesNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Match(
            some: async x =>
            {
                await Task.Delay(1);
                return x * 2;
            },
            none: async () =>
            {
                await Task.Delay(1);
                return -1;
            }
        );

        // Assert (Then)
        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithSyncFactory_ReturnsAlternativeOnNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(() => Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithAsyncFactory_ReturnsAlternativeOnNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.OrElse(async () =>
        {
            await Task.Delay(1);
            return Maybe.Some(99);
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(99, v));
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithSyncValue_ReturnsAlternativeOnNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithSyncFactory_ReturnsAlternativeOnNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(() => 99);

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithAsyncFactory_ReturnsAlternativeOnNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.ValueOr(async () =>
        {
            await Task.Delay(1);
            return 99;
        });

        // Assert (Then)
        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_Map_OnNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Map(async x =>
        {
            await Task.Delay(1);
            return x * 2;
        });

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithAsyncPredicateFalse_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x =>
        {
            await Task.Delay(1);
            return x > 50;
        });

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithAsyncPredicate_FiltersCorrectly()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x =>
        {
            await Task.Delay(1);
            return x > 40;
        });

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithAsyncPredicateFalse_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.Filter(async x =>
        {
            await Task.Delay(1);
            return x > 50;
        });

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task ValueTaskMaybe_Bind_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());
        var functionCalled = false;

        // Act (When)
        var result = await maybe.Bind(x =>
        {
            functionCalled = true;
            return ValueTask.FromResult(Maybe.Some(x * 2));
        });

        // Assert (Then)
        result.ShouldBe().None();
        Assert.False(functionCalled);
    }

    [Fact]
    public async Task TaskMaybe_Bind_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());
        var functionCalled = false;

        // Act (When)
        var result = await maybe.Bind(x =>
        {
            functionCalled = true;
            return Maybe.Some(x * 2);
        });

        // Assert (Then)
        result.ShouldBe().None();
        Assert.False(functionCalled);
    }

    [Fact]
    public async Task ValueTaskMaybe_TapSome_WithNone_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapSome(x => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe().None();
        Assert.False(actionExecuted);
    }

    [Fact]
    public async Task ValueTaskMaybe_TapNone_WithSome_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));
        var actionExecuted = false;

        // Act (When)
        var result = await maybe.TapNone(() => actionExecuted = true);

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
        Assert.False(actionExecuted);
    }

    [Fact]
    public async Task TaskMaybe_Bind_WithSyncFunction_ReturningSome_ReturnsTransformedValue()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(10));

        // Act (When)
        var result = await maybe.Bind(x => Maybe.Some(x * 2));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(20, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_Filter_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Filter(x => x > 0);

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task TaskMaybe_Filter_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Filter(x => x > 0);

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task ValueTaskMaybe_OrElse_WithSyncValue_ReturnsOriginal()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TaskMaybe_OrElse_WithSyncValue_ReturnsOriginal()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.OrElse(Maybe.Some(99));

        // Assert (Then)
        result.ShouldBe()
              .Some()
              .And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task ValueTaskMaybe_ValueOr_WithSyncValue_ReturnsSome()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task TaskMaybe_ValueOr_WithSyncValue_ReturnsSome()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.Some(42));

        // Act (When)
        var result = await maybe.ValueOr(99);

        // Assert (Then)
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task ValueTaskMaybe_Map_WithSyncMapper_OnNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = ValueTask.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe().None();
    }

    [Fact]
    public async Task TaskMaybe_Map_WithNone_ReturnsNone()
    {
        // Arrange (Given)
        var maybe = Task.FromResult(Maybe.None<int>());

        // Act (When)
        var result = await maybe.Map(x => x * 2);

        // Assert (Then)
        result.ShouldBe().None();
    }
}
