using System.Text.Json;
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
        var mapper = new ResultHttpOptions().MapCode(FailureCodes.Validation, 409).BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new ValidationFailure([new ValidationError("Email", "Invalid")]));

        // Assert (Then)
        Assert.Equal(409, problem.Status);
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

    [Theory]
    [InlineData("type")]
    [InlineData("title")]
    [InlineData("status")]
    [InlineData("detail")]
    [InlineData("instance")]
    [InlineData("Status")]
    public void ExposeMetadata_BuiltInProblemDetailsMember_Throws(string key)
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();

        // Act & Assert (When/Then)
        Assert.Throws<ArgumentException>(() => options.ExposeMetadata(key));
    }

    [Fact]
    public void ExposeMetadata_SerializedResponse_WritesEachFieldOnce()
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().ExposeMetadata("freeBp").BuildMapper();
        var failure = new ConflictFailure("Share exceeded.", new Dictionary<string, object?>
        {
            ["freeBp"] = 1200,
            ["status"] = 200
        });

        // Act (When)
        var json = JsonSerializer.Serialize(Problem(mapper, failure), new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert (Then)
        using var document = JsonDocument.Parse(json);
        var names = document.RootElement.EnumerateObject().Select(p => p.Name).ToArray();
        Assert.Equal(names.Length, names.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Equal(409,  document.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(1200, document.RootElement.GetProperty("freeBp").GetInt32());
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
    [InlineData("orders.locked",  409, "Conflict")]
    [InlineData("orders.locked2", 423, "Locked")]
    [InlineData("orders.custom",  470, "An error occurred.")]
    public void MapCode_PlainFailure_TitleIsStatusReasonPhrase(string code,
                                                                int    statusCode,
                                                                string expectedTitle)
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().MapCode(code, statusCode).BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new Failure(code, "Refused."));

        // Assert (Then)
        Assert.Equal(statusCode,    problem.Status);
        Assert.Equal(expectedTitle, problem.Title);
    }

    [Theory]
    [InlineData(null, "Internal Server Error")]
    [InlineData(429,  "Too Many Requests")]
    public void DefaultStatus_PlainFailure_TitleIsStatusReasonPhrase(int?   defaultStatus,
                                                                     string expectedTitle)
    {
        // Arrange (Given)
        var options = new ResultHttpOptions();
        if (defaultStatus is { } status) {
            options.DefaultStatus(status);
        }

        // Act (When)
        var problem = Problem(options.BuildMapper(), new Failure("unmapped", "Refused."));

        // Assert (Then)
        Assert.Equal(expectedTitle, problem.Title);
    }

    [Theory]
    [InlineData("orders.closed",  400, null, "Bad Request")]
    [InlineData("orders.changed", 409, "https://www.rfc-editor.org/rfc/rfc9110#section-15.5.10", "Conflict")]
    public void MapCode_TypedFailure_TitleFollowsAnsweredStatus(string  code,
                                                                int     expectedStatus,
                                                                string? expectedType,
                                                                string  expectedTitle)
    {
        // Arrange (Given)
        var mapper = new ResultHttpOptions().MapCode("orders.closed", 400).BuildMapper();

        // Act (When)
        var problem = Problem(mapper, new ConflictFailure("Refused.") { Code = code });

        // Assert (Then)
        Assert.Equal(expectedStatus, problem.Status);
        Assert.Equal(expectedType,   problem.Type);
        Assert.Equal(expectedTitle,  problem.Title);
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
