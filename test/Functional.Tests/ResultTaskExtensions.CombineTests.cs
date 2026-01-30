using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

/// <summary>
///     Tests for ResultTask Combine extension methods.
/// </summary>
public sealed partial class ResultTaskExtensionsTests
{
    [Fact]
    public async Task Combine_WithAllTasksSucceeding_ReturnsSuccess()
    {
        // Arrange (Given)
        var tasks = new[]
        {
            Result.Success().AsAsync(),
            Result.Success().AsAsync(),
            Result.Success().AsAsync()
        };

        // Act (When)
        var combined = await tasks.Combine();

        // Assert (Then)
        combined.ShouldBe().Success();
    }

    [Fact]
    public async Task Combine_WithOneTaskFailing_ReturnsFailure()
    {
        // Arrange (Given)
        var tasks = new[]
        {
            Result.Success().AsAsync(),
            Result.Failure("Error 1").AsAsync(),
            Result.Success().AsAsync()
        };

        // Act (When)
        var combined = await tasks.Combine();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public async Task Combine_WithMultipleTasksFailing_AggregatesErrors()
    {
        // Arrange (Given)
        var tasks = new[]
        {
            Result.Failure("Error 1").AsAsync(),
            Result.Success().AsAsync(),
            Result.Failure("Error 2").AsAsync()
        };

        // Act (When)
        var combined = await tasks.Combine();

        // Assert (Then)
        combined.ShouldBe().Failure();
    }

    [Fact]
    public async Task Combine_WithEmptyCollection_ReturnsSuccess()
    {
        // Arrange (Given)
        var tasks = Array.Empty<ResultTask>();

        // Act (When)
        var combined = await tasks.Combine();

        // Assert (Then)
        combined.ShouldBe().Success();
    }
}