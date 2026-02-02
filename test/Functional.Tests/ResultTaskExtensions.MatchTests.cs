namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for MapError, Match, MatchError, and Recover async extensions using ValueTask.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Match_WithResultTaskSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            value => ValueTask.FromResult($"Success: {value}"),
            error => ValueTask.FromResult($"Failure: {error.Message}"));

        // Assert (Then)
        Assert.Equal("Success: 42", output);
    }

    [Fact]
    public async Task Match_WithResultTaskFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            value => ValueTask.FromResult($"Success: {value}"),
            error => ValueTask.FromResult($"Failure: {error.Message}"));

        // Assert (Then)
        Assert.Equal("Failure: Error", output);
    }

    [Fact]
    public async Task Match_VoidReturn_WithResultTaskSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value =>
            {
                executed = $"Success: {value}";
                return ValueTask.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Success: 42", executed);
    }

    [Fact]
    public async Task Match_VoidReturn_WithResultTaskFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value =>
            {
                executed = $"Success: {value}";
                return ValueTask.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }

    [Fact]
    public async Task Match_SyncAction_WithResultTaskSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value => executed = $"Success: {value}",
            error => executed = $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Success: 42", executed);
    }

    [Fact]
    public async Task Match_SyncAction_WithResultTaskFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value => executed = $"Success: {value}",
            error => executed = $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }

    [Fact]
    public async Task Match_TaskReturn_WithResultTaskSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value =>
            {
                executed = $"Success: {value}";
                return Task.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return Task.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Success: 42", executed);
    }

    [Fact]
    public async Task Match_TaskReturn_WithResultTaskFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            value =>
            {
                executed = $"Success: {value}";
                return Task.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return Task.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }

    [Fact]
    public async Task Match_SyncFunc_WithResultTaskSuccess_ReturnsSuccessValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success(42).AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            value => $"Success: {value}",
            error => $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Success: 42", output);
    }

    [Fact]
    public async Task Match_SyncFunc_WithResultTaskFailure_ReturnsFailureValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure<int>("Error").AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            value => $"Success: {value}",
            error => $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Failure: Error", output);
    }

    [Fact]
    public async Task Match_NonGenericSyncAction_WithSuccess_ExecutesSuccessAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        await awaitableResult.Match(
            () => executed = "Success",
            error => executed = $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Success", executed);
    }

    [Fact]
    public async Task Match_NonGenericSyncAction_WithFailure_ExecutesFailureAction()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            () => executed = "Success",
            error => executed = $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }

    [Fact]
    public async Task Match_NonGenericValueTaskReturn_WithSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        await awaitableResult.Match(
            () =>
            {
                executed = "Success";
                return ValueTask.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Success", executed);
    }

    [Fact]
    public async Task Match_NonGenericValueTaskReturn_WithFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            () =>
            {
                executed = "Success";
                return ValueTask.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return ValueTask.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }

    [Fact]
    public async Task Match_NonGenericTaskReturn_WithSuccess_ExecutesSuccessFunc()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        await awaitableResult.Match(
            () =>
            {
                executed = "Success";
                return Task.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return Task.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Success", executed);
    }

    [Fact]
    public async Task Match_NonGenericTaskReturn_WithFailure_ExecutesFailureFunc()
    {
        // Arrange (Given)
        var executed = "";
        var awaitableResult = Result.Failure("Error").AsAsync();

        // Act (When)
        await awaitableResult.Match(
            () =>
            {
                executed = "Success";
                return Task.CompletedTask;
            },
            error =>
            {
                executed = $"Failure: {error.Message}";
                return Task.CompletedTask;
            });

        // Assert (Then)
        Assert.Equal("Failure: Error", executed);
    }

    [Fact]
    public async Task Match_NonGenericSyncFunc_WithSuccess_ReturnsSuccessValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            () => "Success",
            error => $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Success", output);
    }

    [Fact]
    public async Task Match_NonGenericSyncFunc_WithFailure_ReturnsFailureValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure("Error").AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            () => "Success",
            error => $"Failure: {error.Message}");

        // Assert (Then)
        Assert.Equal("Failure: Error", output);
    }

    [Fact]
    public async Task Match_NonGenericValueTaskOfTOut_WithSuccess_ReturnsSuccessValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Success().AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            () => ValueTask.FromResult("Success"),
            error => ValueTask.FromResult($"Failure: {error.Message}"));

        // Assert (Then)
        Assert.Equal("Success", output);
    }

    [Fact]
    public async Task Match_NonGenericValueTaskOfTOut_WithFailure_ReturnsFailureValue()
    {
        // Arrange (Given)
        var awaitableResult = Result.Failure("Error").AsAsync();

        // Act (When)
        var output = await awaitableResult.Match(
            () => ValueTask.FromResult("Success"),
            error => ValueTask.FromResult($"Failure: {error.Message}"));

        // Assert (Then)
        Assert.Equal("Failure: Error", output);
    }
}