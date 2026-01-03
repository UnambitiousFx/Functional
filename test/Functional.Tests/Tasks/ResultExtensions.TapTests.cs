using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for async Tap extension methods on Result using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task TapAsync_CanBeChained()
    {
        // Arrange (Given)
        var result = Result.Success(10);
        var calls = new List<int>();

        // Act (When)
        var final = await result
            .TapAsync(async x =>
            {
                await Task.CompletedTask;
                calls.Add(x);
            })
            .TapAsync(x => calls.Add(x * 2));

        // Assert (Then)
        final.ShouldBe().Success();
        Assert.Equal(2, calls.Count);
        Assert.Equal(10, calls[0]);
        Assert.Equal(20, calls[1]);
    }


    [Fact]
    public async Task TapAsync_WithSuccessResult_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success();
        var executed = false;

        // Act (When)
        var tapped = await result.TapAsync(async () =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.True(executed);
    }

    [Fact]
    public async Task TapAsync_WithFailureResult_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Failure("Error");
        var executed = false;

        // Act (When)
        var tapped = await result.TapAsync(async () =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(executed);
    }


    [Fact]
    public async Task TapAsync_WithGenericSuccessResult_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Result.Success(42);
        var capturedValue = 0;

        // Act (When)
        var tapped = await result.TapAsync(async x =>
        {
            await Task.CompletedTask;
            capturedValue = x;
        });

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(42, value));
        Assert.Equal(42, capturedValue);
    }

    [Fact]
    public async Task TapAsync_WithGenericFailureResult_DoesNotExecuteSideEffect()
    {
        // Arrange (Given)
        var result = Result.Failure<int>("Error");
        var executed = false;

        // Act (When)
        var tapped = await result.TapAsync(async x =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(executed);
    }


    [Fact]
    public async Task TapAsync_WithTaskResultAndSyncAction_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success());
        var executed = false;

        // Act (When)
        var tapped = await result.TapAsync(() => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Success();
        Assert.True(executed);
    }

    [Fact]
    public async Task TapAsync_WithTaskResultFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Failure("Error"));
        var executed = false;

        // Act (When)
        var tapped = await result.TapAsync(() => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(executed);
    }


    [Fact]
    public async Task TapAsync_WithTaskGenericResultAndSyncAction_ExecutesSideEffect()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Success(100));
        var capturedValue = 0;

        // Act (When)
        var tapped = await result.TapAsync(x => capturedValue = x);

        // Assert (Then)
        tapped.ShouldBe().Success().And(value => Assert.Equal(100, value));
        Assert.Equal(100, capturedValue);
    }

    [Fact]
    public async Task TapAsync_WithTaskGenericResultFailure_DoesNotExecute()
    {
        // Arrange (Given)
        var result = Task.FromResult(Result.Failure<int>("Failed"));
        var executed = false;

        // Act (When)
        var tapped = await result.TapAsync(x => executed = true);

        // Assert (Then)
        tapped.ShouldBe().Failure();
        Assert.False(executed);
    }


    [Fact]
    public async Task TapAsync_WithAwaitableSuccess_ExecutesAction()
    {
        // Arrange (Given)
        var executed = false;
        var awaitableResult = Task.FromResult(Result.Success());

        // Act (When)
        var result = await awaitableResult.TapAsync(async () =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        Assert.True(executed);
        result.ShouldBe().Success();
    }

    [Fact]
    public async Task TapAsync_WithAwaitableFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var executed = false;
        var awaitableResult = Task.FromResult(Result.Failure("Error"));

        // Act (When)
        var result = await awaitableResult.TapAsync(async () =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        Assert.False(executed);
        result.ShouldBe().Failure();
    }

    [Fact]
    public async Task TapAsync_Generic_WithAwaitableSuccess_ExecutesActionWithValue()
    {
        // Arrange (Given)
        var capturedValue = 0;
        var awaitableResult = Task.FromResult(Result.Success(42));

        // Act (When)
        var result = await awaitableResult.TapAsync(async value =>
        {
            await Task.CompletedTask;
            capturedValue = value;
        });

        // Assert (Then)
        Assert.Equal(42, capturedValue);
        result.ShouldBe().Success().And(v => Assert.Equal(42, v));
    }

    [Fact]
    public async Task TapAsync_Generic_WithAwaitableFailure_DoesNotExecuteAction()
    {
        // Arrange (Given)
        var executed = false;
        var awaitableResult = Task.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        var result = await awaitableResult.TapAsync(async value =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // Assert (Then)
        Assert.False(executed);
        result.ShouldBe().Failure();
    }
}
