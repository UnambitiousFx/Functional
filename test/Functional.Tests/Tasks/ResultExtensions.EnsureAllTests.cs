using UnambitiousFx.Functional.Errors;
using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

public sealed partial class ResultExtensions
{
    [Fact]
    public async Task EnsureAll_WithAwaitableSuccess_AllMatch_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 2, 4, 6 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureAll<int[], int>(x => x % 2 == 0, "All must be even");

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureAll_WithAwaitableSuccess_NotAllMatch_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 2, 3, 6 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureAll<int[], int>(x => x % 2 == 0, "All must be even");

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }

    [Fact]
    public async Task EnsureAll_WithErrorFactory_WithAwaitableSuccess_NotAllMatch_UsesFactory()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 2, 3, 6 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureAll<int[], int>(
            x => x % 2 == 0,
            collection => new Error("Custom error"));

        // Assert (Then)
        ensured.ShouldBe().Failure().AndMessage("Custom error");
    }

    [Fact]
    public async Task EnsureAny_WithAwaitableSuccess_AnyMatch_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 1, 2, 3 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureAny<int[], int>(x => x == 2, "Must contain 2");

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureAny_WithAwaitableSuccess_NoneMatch_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 1, 3, 5 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureAny<int[], int>(x => x % 2 == 0, "Must contain even number");

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }

    [Fact]
    public async Task EnsureAny_WithAwaitableSuccess_NoneMatch_BecomesFailureWithErrorFactory()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 1, 3, 5 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureAny<int[], int>(x => x % 2 == 0, x => new Error("Must contain even number"));

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }

    [Fact]
    public async Task EnsureNone_WithAwaitableSuccess_NoneMatch_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 1, 3, 5 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureNone<int[], int>(x => x % 2 == 0, "Must not contain even numbers");

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureNone_WithAwaitableSuccess_AnyMatch_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success(new[] { 1, 2, 3 }));

        // Act (When)
        var ensured = await awaitableResult.EnsureNone<int[], int>(x => x % 2 == 0, "Must not contain even numbers");

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }
}
