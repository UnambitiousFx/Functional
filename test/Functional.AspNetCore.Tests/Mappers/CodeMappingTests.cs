using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Mappers;
using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.AspNetCore.Tests.Mappers;

public class CodeMappingTests
{
    [Fact]
    public void MapCode_PlainFailureWithMappedCode_ReturnsMappedStatusWithCodeAndMessage()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().MapCode("fx.sync_running", 409).BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new Failure("fx.sync_running", "A sync is running."));

        // Assert (Then)
        Assert.Equal(409,                 problem.Status);
        Assert.Equal("A sync is running.", problem.Detail);
        Assert.Equal("fx.sync_running",   problem.Extensions["code"]);
    }

    [Fact]
    public void MapCodePrefix_CodeWithPrefix_ReturnsPrefixStatus()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().MapCodePrefix("documents.", 413).BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new Failure("documents.too_large", "Too large."));

        // Assert (Then)
        Assert.Equal(413, problem.Status);
    }

    [Fact]
    public void MapCode_ExactAndPrefixMatch_ExactCodeWins()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().MapCodePrefix("documents.", 413)
                                            .MapCode("documents.locked", 423)
                                            .BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new Failure("documents.locked", "Locked."));

        // Assert (Then)
        Assert.Equal(423, problem.Status);
    }

    [Fact]
    public void MapCode_TypedFailure_KeepsTypedBodyWithMappedStatus()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().MapCode(FailureCodes.Validation, 422).BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new ValidationFailure([new ValidationError("Email", "Invalid")]));

        // Assert (Then)
        Assert.Equal(422, problem.Status);
        Assert.Null(problem.Type);
        Assert.Contains("errors", problem.Extensions);
    }

    [Fact]
    public void MapCode_TypedMapperForSameFailure_TypedMapperWins()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().AddMapper<ConflictFailure>(418)
                                            .MapCode(FailureCodes.Conflict, 423)
                                            .BuildMapper();

        // Act (When)
        var response = mapper.GetFailureResponse(new ConflictFailure("taken"));

        // Assert (Then)
        Assert.Equal(418, response!.StatusCode);
    }

    [Theory]
    [InlineData(null, 500, null)]
    [InlineData(422,  422, "Refused.")]
    public void DefaultStatus_PlainFailure_UsesConfiguredStatusAndShowsMessageBelow500(int?    defaultStatus,
                                                                                      int     expectedStatus,
                                                                                      string? expectedDetail)
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        if (defaultStatus is { } status) {
            options.DefaultStatus(status);
        }

        // Act (When)
        var problem = Problem(options.BuildMapper(), new Failure("goals.share_exceeded", "Refused."));

        // Assert (Then)
        Assert.Equal(expectedStatus, problem.Status);
        Assert.Equal(expectedDetail, problem.Detail);
        Assert.Equal("goals.share_exceeded", problem.Extensions["code"]);
    }

    [Fact]
    public void ExposeMetadata_ListedKeys_CopiedIntoExtensionsOthersHidden()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().ExposeMetadata("freeBp", "line", "code").BuildMapper();
        var failure = new ConflictFailure("Share exceeded.", new Dictionary<string, object?>
        {
            ["freeBp"]   = 1200,
            ["internal"] = "secret",
            ["code"]     = "overwritten?"
        });

        // Act (When)
        var problem = Problem(mapper, failure);

        // Assert (Then)
        Assert.Equal(1200,                  problem.Extensions["freeBp"]);
        Assert.Equal(FailureCodes.Conflict, problem.Extensions["code"]);
        Assert.DoesNotContain("internal", problem.Extensions);
        Assert.DoesNotContain("line",     problem.Extensions);
    }

    [Fact]
    public void GetFailureResponse_AnyTypedFailure_WritesCode()
    {
        // Arrange (Given)
        var mapper = new DefaultFailureHttpMapper();

        // Act (When)
        var problem = Problem(mapper, new ConflictFailure("taken"));

        // Assert (Then)
        Assert.Equal(FailureCodes.Conflict, problem.Extensions["code"]);
    }

    [Theory]
    [InlineData("", 409)]
    [InlineData("code", 99)]
    [InlineData("code", 600)]
    public void MapCode_InvalidArguments_Throws(string code,
                                                int    statusCode)
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act & Assert (When/Then)
        Assert.ThrowsAny<ArgumentException>(() => options.MapCode(code, statusCode));
    }

    private static ProblemDetails Problem(IFailureHttpMapper mapper,
                                          IFailure           failure)
    {
        return Assert.IsType<ProblemDetails>(mapper.GetFailureResponse(failure)!.Body);
    }
}
