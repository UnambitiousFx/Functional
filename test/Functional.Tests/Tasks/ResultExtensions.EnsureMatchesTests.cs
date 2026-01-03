using System.Text.RegularExpressions;
using UnambitiousFx.Functional.Tasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Tasks;

/// <summary>
///     Tests for Then, Bind, Ensure, and TryAsync awaitable wrapper extensions using Task.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public async Task EnsureMatches_StringPattern_WithAwaitableSuccess_Matches_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success("test123"));

        // Act (When)
        var ensured = await awaitableResult.EnsureMatches(@"^\w+\d+$");

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureMatches_StringPattern_WithAwaitableSuccess_DoesNotMatch_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success("test"));

        // Act (When)
        var ensured = await awaitableResult.EnsureMatches(@"^\d+$");

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }

    [Fact]
    public async Task EnsureMatches_Regex_WithAwaitableSuccess_Matches_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success("test123"));
        var regex = new Regex(@"^\w+\d+$");

        // Act (When)
        var ensured = await awaitableResult.EnsureMatches(regex);

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureDoesNotMatch_WithAwaitableSuccess_DoesNotMatch_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success("test"));

        // Act (When)
        var ensured = await awaitableResult.EnsureDoesNotMatch(@"^\d+$");

        // Assert (Then)
        ensured.ShouldBe().Success();
    }

    [Fact]
    public async Task EnsureDoesNotMatch_WithAwaitableSuccess_Matches_BecomesFailure()
    {
        // Arrange (Given)
        var awaitableResult = Task.FromResult(Result.Success("123"));

        // Act (When)
        var ensured = await awaitableResult.EnsureDoesNotMatch(@"^\d+$");

        // Assert (Then)
        ensured.ShouldBe().Failure();
    }
}
