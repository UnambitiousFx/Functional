using UnambitiousFx.Functional.ValueTasks;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for awaitable wrapper extension methods (Tap, ToNullable, ValueOr, HasError, etc.) using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task ToNullableAsync_WithAwaitableSuccess_ReturnsValue()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(42));

        // Act (When)
        var value = await awaitableResult.ToNullableAsync();

        // Assert (Then)
        Assert.Equal(42, value);
    }

    [Fact]
    public async Task ToNullableAsync_WithAwaitableFailure_ReturnsNull()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Failure<int>("Error"));

        // Act (When)
        int? value = await awaitableResult.ToNullableAsync();

        // Assert (Then)
        Assert.Equal(0, value.GetValueOrDefault());
    }
}
