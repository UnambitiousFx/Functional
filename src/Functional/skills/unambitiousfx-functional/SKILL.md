---
name: unambitiousfx-functional
description: Write C# code with UnambitiousFx.Functional (Result, Result<T>, Maybe<T>, Failure types) using railway-oriented composition. Use when a .NET project references UnambitiousFx.Functional, or when writing/refactoring code that returns Result/Maybe, models errors as values, or chains Bind/Map/Tap/Ensure/Match.
---

# UnambitiousFx.Functional

Namespaces: `UnambitiousFx.Functional` (Result, Maybe, extensions) and `UnambitiousFx.Functional.Failures` (failure types).

## Rules

- Expected failures (not found, validation, conflict, auth) are **values**: return `Result`/`Result<T>`, never throw.
- Absence is `Maybe<T>`, never `null`. Convert to `Result<T>` with `ToResult(...)` once absence becomes an error.
- Compose with `Bind`/`Map`/`Ensure`/`Tap`; unwrap only at the edge (`Match`, `TryGet`, HTTP mapping). Avoid `if (r.IsFailure) return ...` ladders.
- Async methods return `ValueTask<Result<T>>`. The async extensions are defined on `ValueTask<Result<T>>` only, not `Task<Result<T>>` — wrap a `Task` with `new ValueTask<Result<T>>(task)` or make the method return `ValueTask`.
- `Result` and `Maybe<T>` are `readonly record struct`s: never `null`, no `?` on them.
- A result holds **one** `Failure` (several are wrapped in `AggregateFailure`).

## Creating results

```csharp
Result ok = Result.Success();                     // alias: Result.Ok()
Result<User> r1 = Result.Success(user);           // alias: Result.Ok(user)
Result<User> r2 = user;                           // implicit from value
Result<User> r3 = new ConflictFailure("Email already used"); // implicit from Failure

Result.Failure<User>("message");                  // generic Failure, code "ERROR"
Result.Failure<User>(new Failure("CODE", "message"));
Result.Failure<User>(exception);                  // wraps in ExceptionalFailure

// Typed shortcuts (also non-generic versions)
Result.FailNotFound<User>("User", id.ToString()); // (resource, identifier, messageOverride?)
Result.FailValidation<User>("Email is required");
Result.FailConflict<User>("Email already used");
Result.FailBadRequest<User>("Malformed payload");
Result.FailUnauthorized<User>("Not owner");       // authenticated but forbidden
Result.FailUnauthenticated<User>("No token");

Maybe<User> some = Maybe.Some(user);
Maybe<User> none = Maybe.None<User>();
```

## Failure types (`UnambitiousFx.Functional.Failures`)

| Type | Constructor | `Code` (`FailureCodes.*`) |
|------|-------------|------|
| `Failure` | `(message)` or `(code, message)` | `ERROR` |
| `ValidationFailure` | `(string message)` or `(IReadOnlyList<string> failures)` | `VALIDATION` |
| `NotFoundFailure` | `(resource, identifier, messageOverride?)` — **not** `(message)` | `NOT_FOUND` |
| `ConflictFailure` | `(message)` | `CONFLICT` |
| `BadRequestFailure` / `UnauthorizedFailure` / `UnauthenticatedFailure` / `TimeoutFailure` | see ctor | matching code |
| `ExceptionalFailure` | `(exception)` | `EXCEPTION` |
| `AggregateFailure` | `(IEnumerable<Failure>)` | `AGGREGATE_ERROR` |

Custom domain failures: derive a `record` from `Failure`.

## Composing (Result<T>)

| Need | Use |
|------|-----|
| Transform the value | `Map(v => ...)` |
| Next step can fail | `Bind(v => StepReturningResult(v))` |
| Validate | `Ensure(v => cond, v => new ValidationFailure("..."))` (failure is a **factory**) |
| Side effect, keep value | `Tap(v => ...)`, `TapFailure(f => ...)`, `TapIf(pred, ...)` |
| Fallback on failure | `Recover(f => fallbackValue)` or `Recover(fallbackValue)` |
| Run code that may throw | `Try(v => ...)` — exceptions become `ExceptionalFailure` |
| Many results → one | `results.Combine()` → `Result<IEnumerable<T>>` |
| Unwrap nested | `Flatten()` |
| Drop the value | `ToResult()` → `Result` |
| Attach context | `WithMetadata("key", value)` / `WithMetadata(b => b.Add(...))` |
| Query syntax | `from a in r1 from b in r2 select ...` (`Select`/`SelectMany`/`Where`) |

```csharp
public ValueTask<Result<Order>> PlaceOrderAsync(Guid userId, OrderRequest request) =>
    _users.GetAsync(userId)                                   // ValueTask<Result<User>>
        .Ensure(u => u.IsActive, u => new ValidationFailure($"User {u.Id} is inactive"))
        .Bind(u => Order.Create(u, request))                  // sync Result<Order>
        .Bind(order => _orders.SaveAsync(order))              // ValueTask<Result<Order>>
        .Tap(order => _logger.LogInformation("Order {Id} placed", order.Id));
```

## Maybe<T>

`Map`, `Bind`, `Filter(pred)`, `OrElse(...)`, `TapSome`/`TapNone`, `ValueOr(fallback)`, `ToResult(failure | () => failure | "message")`, LINQ query syntax.

```csharp
Result<User> user = _repo.FindByEmail(email)               // Maybe<User>
    .ToResult(() => new NotFoundFailure("User", email));
```

## Consuming at the edge

```csharp
string text = result.Match(
    success: user => $"Hello {user.Name}",
    failure: failure => $"Error {failure.Code}: {failure.Message}");

if (result.TryGet(out var value, out var failure)) { /* value */ } else { /* failure */ }
if (result.TryGetFailure(out var f)) { ... }

var v1 = result.ValueOr(fallback);        // or ValueOr(() => fallback)
var v2 = result.ValueOrDefault();
var v3 = result.ValueOrThrow();           // only at boundaries where throwing is intended

maybe.Match(some: u => ..., none: () => ...);
if (maybe.Some(out var u)) { ... }
```

## Pitfalls

- `new NotFoundFailure("User not found")` does not compile — use `("User", id)` or `Result.FailNotFound<T>(...)`.
- `Ensure` takes `Func<T, Failure>`, not a `Failure` instance.
- There is no `TapBoth`; chain `Tap(...).TapFailure(...)`.
- Do not read `.Value` style properties — there are none; use `Match`/`TryGet`/`ValueOr`.
- Do not catch exceptions to build failures by hand when `Try(...)` or `Result.Failure(exception)` does it.
- For ASP.NET Core responses use the `UnambitiousFx.Functional.AspNetCore` package; for tests use `UnambitiousFx.Functional.xunit` (`result.ShouldBe().Success()`).

Full docs: https://functional.unambitiousfx.com/llms.txt
