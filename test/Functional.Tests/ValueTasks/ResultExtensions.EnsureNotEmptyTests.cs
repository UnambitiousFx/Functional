using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

/// <summary>
///     Tests for Then, Bind, Ensure, and TryAsync awaitable wrapper extensions using ValueTask.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task EnsureNotEmptyAsync_String_WithAwaitableSuccess_NotEmpty_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success("test"));

        // Act (When)
        var ensured = await awaitableResult.EnsureNotEmptyAsync();

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureNotEmptyAsync_String_WithAwaitableSuccess_Empty_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(""));

        // Act (When)
        var ensured = await awaitableResult.EnsureNotEmptyAsync();

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }

    [Fact]
    public async Task EnsureNotEmptyAsync_Collection_WithAwaitableSuccess_NotEmpty_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(new[] { 1, 2, 3 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureNotEmptyAsync<int[], int>();

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureNotEmptyAsync_Collection_WithAwaitableSuccess_Empty_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(Array.Empty<int>()));

        // Act (When)
        var ensured = await awaitableResult.EnsureNotEmptyAsync<int[], int>();

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }
}
