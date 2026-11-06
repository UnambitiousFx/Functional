#if NET11_0_OR_GREATER
using UnambitiousFx.Functional.Failures;
using UnambitiousFx.Functional.xunit;

namespace UnambitiousFx.Functional.Tests;

public sealed class ResultUnionTests {
    [Fact]
    public void Switch_WithSuccessResult_MatchesValueArm() {
        // Arrange (Given)
        var result = Result.Success(5);

        // Act (When) — two-arm exhaustive switch, NO null arm: compiling is the proof the
        // union's Value is non-null and a result is always success or failure.
        var output = result switch {
            int value => value * 2,
            Failure _ => -1,
        };

        // Assert (Then)
        Assert.Equal(10, output);
    }

    [Fact]
    public void Switch_WithFailureResult_MatchesFailureArm() {
        // Arrange (Given)
        var result = Result.Failure<int>("boom");

        // Act (When)
        var output = result switch {
            int value       => value.ToString(),
            Failure failure => failure.Message,
        };

        // Assert (Then)
        Assert.Equal("boom", output);
    }

    [Fact]
    public void Switch_WithFailureSubtype_FallsThroughFailureArm() {
        // Arrange (Given)
        var result = Result.Failure<int>(new ValidationFailure("invalid"));

        // Act (When) — a ValidationFailure matches the base Failure arm
        var output = result switch {
            int value       => value.ToString(),
            Failure failure => failure.Code,
        };

        // Assert (Then)
        Assert.Equal(FailureCodes.Validation, output);
    }

    [Fact]
    public void Switch_WithSuccessReferenceType_MatchesValueArm() {
        // Arrange (Given)
        var result = Result.Success("ok");

        // Act (When)
        var output = result switch {
            string value    => value,
            Failure failure => failure.Message,
        };

        // Assert (Then)
        Assert.Equal("ok", output);
    }

    [Fact]
    public void Switch_WithDefaultResult_MatchesFailureArm() {
        // Arrange (Given) — a degenerate default result is treated as a failure
        var result = default(Result<int>);

        // Act (When) — still no null arm
        var output = result switch {
            int value => value.ToString(),
            Failure _ => "failure",
        };

        // Assert (Then)
        Assert.Equal("failure", output);
    }

    [Fact]
    public void IsPattern_WithFailureResult_UnwrapsFailure() {
        // Arrange (Given)
        var result = Result.Failure<string>("broken");

        // Act (When)
        var isFailure = result is Failure failure;

        // Assert (Then)
        Assert.True(isFailure);
    }

    [Fact]
    public void Value_WithSuccessReferenceType_ReturnsSameInstance() {
        // Arrange (Given)
        var payload = new object();
        var result  = Result.Success(payload);

        // Act (When)
        var value = result.Value;

        // Assert (Then)
        Assert.Same(payload, value);
    }

    [Fact]
    public void Value_WithFailureResult_ReturnsFailureInstance() {
        // Arrange (Given)
        var failure = new ValidationFailure("invalid");
        var result  = Result.Failure<int>(failure);

        // Act (When)
        var value = result.Value;

        // Assert (Then)
        Assert.Same(failure, value);
    }

    [Fact]
    public void Value_WithDefaultResult_ReturnsNonNullFailure() {
        // Arrange (Given)
        var result = default(Result<int>);

        // Act (When)
        var value = result.Value;

        // Assert (Then) — never null; a default result surfaces a sentinel failure
        Assert.NotNull(value);
        Assert.IsAssignableFrom<Failure>(value);
    }

    [Fact]
    public void HasValue_WithDefaultResult_ReturnsTrue() {
        // Arrange (Given)
        var result = default(Result<int>);

        // Act (When) — Value is never null, so HasValue is always true...
        var hasValue = result.HasValue;
        var gotValue = result.TryGetValue(out _);

        // Assert (Then) — ...but the non-boxing success accessor still reports "not a success"
        Assert.True(hasValue);
        Assert.False(gotValue);
    }

    [Fact]
    public void HasValue_WithSuccessResult_ReturnsTrue() {
        // Arrange (Given)
        var result = Result.Success(42);

        // Act (When)
        var hasValue = result.HasValue;

        // Assert (Then)
        Assert.True(hasValue);
    }

    [Fact]
    public void UnionCreate_WithValue_CreatesSuccessResult() {
        // Arrange (Given)
        const int value = 7;

        // Act (When)
        var result = Result<int>.IUnionMembers.Create(value);

        // Assert (Then)
        result.ShouldBe()
              .Success()
              .And(v => Assert.Equal(7, v));
    }

    [Fact]
    public void UnionCreate_WithFailure_CreatesFailedResult() {
        // Arrange (Given)
        var failure = new ValidationFailure("nope");

        // Act (When)
        var result = Result<int>.IUnionMembers.Create(failure);

        // Assert (Then)
        result.ShouldBe()
              .Failure()
              .AndMessage("nope");
    }

    [Fact]
    public void ImplicitConversion_OnNet11_StillWorks() {
        // Arrange (Given)
        Result<int> fromValue   = 5;
        Result<int> fromFailure = new ValidationFailure("bad");

        // Act (When) & Assert (Then)
        fromValue.ShouldBe()
                 .Success()
                 .And(v => Assert.Equal(5, v));
        fromFailure.ShouldBe()
                   .Failure()
                   .AndMessage("bad");
    }

    [Fact]
    public void Match_OnNet11_StillWorks() {
        // Arrange (Given)
        var result = Result.Success(3);

        // Act (When)
        var output = result.Match(value => value + 1,
                                  _ => -1);

        // Assert (Then)
        Assert.Equal(4, output);
    }
}
#endif
