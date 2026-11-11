using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests.Failures;

public class FailureCodeOverrideTests
{
    public static TheoryData<Failure> TypedFailures()
    {
        return
        [
            new ConflictFailure("conflict"),
            new BadRequestFailure("bad"),
            new NotFoundFailure("Loan", "42"),
            new UnauthorizedFailure(),
            new UnauthenticatedFailure(),
            new ValidationFailure([new ValidationError("Cell", "unreadable")]),
            new TimeoutFailure(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)),
            new ExceptionalFailure(new InvalidOperationException("boom"))
        ];
    }

    [Theory]
    [MemberData(nameof(TypedFailures))]
    public void Code_WithDomainCode_ReplacesDefaultCodeAndKeepsEverythingElse(Failure failure)
    {
        // Arrange (Given)
        const string domainCode = "domain.refused";

        // Act (When)
        var coded = failure with { Code = domainCode };

        // Assert (Then)
        Assert.Equal(domainCode, coded.Code);
        Assert.Equal(failure.GetType(), coded.GetType());
        Assert.Equal(failure.Message, coded.Message);
        Assert.Equal(failure.Metadata, coded.Metadata);
        Assert.NotEqual(failure, coded);
    }

    [Fact]
    public void Code_InObjectInitializer_SetsDomainCodeOnSealedValidationFailure()
    {
        // Arrange (Given)
        IReadOnlyList<ValidationError> errors = [new("A1", "unreadable"), new("B2", "unreadable")];

        // Act (When)
        var failure = new ValidationFailure(errors) { Code = "import.unreadable_cell" };

        // Assert (Then)
        Assert.Equal("import.unreadable_cell", failure.Code);
        Assert.Equal(errors, failure.Errors);
    }

    [Fact]
    public void Code_WithoutDomainCode_KeepsDefaultCode()
    {
        // Arrange & Act (Given/When)
        var failure = new ConflictFailure("The envelope is closed.");

        // Assert (Then)
        Assert.Equal(FailureCodes.Conflict, failure.Code);
    }

    [Fact]
    public void Failure_WithDomainCode_FlowsThroughResult()
    {
        // Arrange (Given)
        var failure = new ConflictFailure("The envelope is closed.") { Code = "investments.envelope_closed" };

        // Act (When)
        var result = Result.Failure<int>(failure);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndCode("investments.envelope_closed");
    }
}
