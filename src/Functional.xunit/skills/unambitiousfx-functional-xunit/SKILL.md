---
name: unambitiousfx-functional-xunit
description: Write xUnit tests that assert on UnambitiousFx.Functional Result, Result<T> and Maybe<T> with the fluent ShouldBe() assertions. Use when a test project references UnambitiousFx.Functional.xunit, or when testing code that returns Result/Maybe.
---

# UnambitiousFx.Functional.xunit

`using UnambitiousFx.Functional.xunit;` (+ `using UnambitiousFx.Functional.xunit.ValueTasks;` for async assertions)

## Rules

- Assert with `ShouldBe()`. Do not unwrap by hand (`Assert.True(result.IsSuccess)`, `TryGet`, `Match`) — the fluent assertions report the failure/value on mismatch.
- Assert the failure **type and content**, not only that it failed: `.Failure().WhichIsNotFoundError()`, `.AndCode(...)`, `.AndMessage(...)`.
- Async: import `UnambitiousFx.Functional.xunit.ValueTasks`, call `ShouldBe()` directly on `ValueTask<Result<T>>` / `Task<Result<T>>` (and Maybe) and `await` the whole chain.

## Result

```csharp
result.ShouldBe().Success();                                    // Result
result.ShouldBe().Success().And(user => Assert.Equal("John", user.Name)); // Result<T>
result.ShouldBe().Success().Where(user => user.IsActive, "user should be active");
var user = result.ShouldBe().Success().Value;                    // grab the value
result.ShouldBe().Success().Map(u => u.Email).And(e => Assert.EndsWith("@x.com", e));

result.ShouldBe().Failure().AndMessage("Email already used").AndCode(FailureCodes.Conflict);
result.ShouldBe().Failure().And(f => Assert.Contains("email", f.Message));
```

### Typed failures

```csharp
result.ShouldBe().Failure().WhichIsValidationError()
    .WithFailure("Email is required")      // exact entry in ValidationFailure.Failures
    .WithFailureContaining("Email")
    .WithFailureCount(1);

result.ShouldBe().Failure().WhichIsNotFoundError()
    .WithResource("User")
    .WithIdentifier(id.ToString());

result.ShouldBe().Failure().WhichIsConflictError();
result.ShouldBe().Failure().WhichIsBadRequestError();
result.ShouldBe().Failure().WhichIsUnauthorizedError();
result.ShouldBe().Failure().WhichIsUnauthenticatedError();
result.ShouldBe().Failure().WhichIsTimeoutError();
result.ShouldBe().Failure().WhichIs<MyDomainFailure>().And(f => Assert.Equal(42, f.Limit));
```

## Maybe

```csharp
maybe.ShouldBe().Some().And(user => Assert.Equal("John", user.Name));
maybe.ShouldBe().None();
```

## Async (`using UnambitiousFx.Functional.xunit.ValueTasks;`)

Only `Success().And/Map`, `Failure().AndMessage`, `Some().And/Map`, `None()` are chainable on the awaitable; for typed failures `await` the result first.

```csharp
await service.CreateAsync(request).ShouldBe().Success().And(u => Assert.NotEqual(Guid.Empty, u.Id));
await service.CreateAsync(invalid).ShouldBe().Failure().AndMessage("Email is required");
await repo.FindAsync(id).ShouldBe().None();
```

## Test shape

```csharp
[Fact]
public void GetUser_WhenUnknownId_ReturnsNotFound()
{
    // Arrange (Given)
    var id = Guid.NewGuid();

    // Act (When)
    var result = _service.GetUser(id);

    // Assert (Then)
    result.ShouldBe().Failure().WhichIsNotFoundError().WithIdentifier(id.ToString());
}
```

Use real `Result`/`Maybe` instances in arrange/stubs (`Result.Success(x)`, `Result.FailNotFound<T>(...)`, `Maybe.None<T>()`); never mock them.
