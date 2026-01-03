using UnambitiousFx.Functional.ValueTasks;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.ValueTasks;

public sealed partial class ResultExtensions
{
    [Fact]
    public async Task EnsureNotNullAsync_WithAwaitableSuccess_NotNull_RemainsSuccess()
    {
        // Arrange (Given)
        var awaitableResult = ValueTask.FromResult(Result.Success(new { Value = "test" }));

        // Act (When)
        var ensured = await awaitableResult.EnsureNotNullAsync(x => x.Value, "Value must not be null");

        // Assert (Then)
        ensured.ShouldBe().Success();
    }
}
