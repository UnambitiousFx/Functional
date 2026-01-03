using UnambitiousFx.Functional.Errors;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for Result HasError, HasException, AppendError, and PrependError extension methods.
/// </summary>
public sealed partial class ResultExtensions
{
    [Fact]
    public void ThrowIfFailed_WithSuccess_DoesNotThrow()
    {
        // Arrange (Given)
        var result = Result.Success();

        // Act & Assert (Then)
        var exception = Record.Exception(() => result.ThrowIfFailed());
        Assert.Null(exception);
    }

    [Fact]
    public void ThrowIfFailed_WithFailure_ThrowsException()
    {
        // Arrange (Given)
        var error = new Error("Test error");
        var result = Result.Failure(error);

        // Act & Assert (Then)
        var exception = Assert.Throws<FunctionalException>(() => result.ThrowIfFailed());
        Assert.Contains("Test error", exception.Message);
    }
}
