# Functional.AspNetCore

ASP.NET Core integration for `Result<T>` and `Maybe<T>`. Provides one-line conversion to HTTP responses for both Minimal
APIs and MVC controllers, with predictable defaults and zero boilerplate for common cases.

## Features

- ✅ **AOT-Compatible**: Designed for Native AOT compilation
- ✅ **Minimal API Support**: Convert `Result<T>` / `Maybe<T>` to `IResult`
- ✅ **MVC Controller Support**: Convert `Result<T>` / `Maybe<T>` to `IActionResult`
- ✅ **async-first**: First-class `ValueTask<Result<T>>` and `ValueTask<Maybe<T>>` overloads
- ✅ **Policy-based defaults**: Configure success/none HTTP behavior globally via `AddResultHttp`
- ✅ **Extensible Error Mapping**: Typed helper builders and chain-of-responsibility mapper pipeline
- ✅ **Problem Details**: RFC 7807 compliant error responses out of the box

## Installation

```bash
dotnet add package UnambitiousFx.Functional.AspNetCore
```

## Setup

Register the services once in your app startup:

```csharp
builder.Services.AddResultHttp();
```

The mappers and policy configured here are read from the request services when the response is written, so every
`AsHttpBuilder()` / `AsActionResultBuilder()` call without an explicit mapper uses them. Without `AddResultHttp`, the
built-in defaults apply.

## Quick Start

The builders are awaitable: `await result.AsHttpBuilder()` gives an `IResult`, `await result.AsActionResultBuilder()`
gives an `IActionResult`.

### Minimal API

```csharp
using UnambitiousFx.Functional.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResultHttp();

var app = builder.Build();

// Result<T> — success: 200 OK with value, failure: mapped error status
app.MapGet("/users/{id:guid}", async (Guid id, IUserService svc) =>
    await svc.GetUserAsync(id).AsHttpBuilder());

// Maybe<T> — Some: 200 OK with value, None: 404 Not Found (by default)
app.MapGet("/profiles/{id:guid}", async (Guid id, IProfileService svc) =>
    await svc.TryGetProfileAsync(id).AsHttpBuilder());

// Result (non-generic) — success: 204 No Content (by default), failure: mapped error status
app.MapDelete("/users/{id:guid}", async (Guid id, IUserService svc) =>
    await svc.DeleteUserAsync(id).AsHttpBuilder());

// Created response from Result<T>
app.MapPost("/users", async (CreateUserRequest req, IUserService svc) =>
    await svc.CreateUserAsync(req).AsHttpBuilder().AsCreated(user => $"/users/{user.Id}"));

app.Run();
```

### MVC Controllers

```csharp
using UnambitiousFx.Functional.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _svc;

    public UsersController(IUserService svc) => _svc = svc;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
        => await _svc.GetUserAsync(id).AsActionResultBuilder();

    [HttpGet("profiles/{id:guid}")]
    public async Task<IActionResult> GetProfile(Guid id)
        // Maybe<T> — Some: 200 OK, None: 404 Not Found (by default)
        => await _svc.TryGetProfileAsync(id).AsActionResultBuilder();

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest req)
        => await _svc.CreateUserAsync(req)
                     .AsActionResultBuilder()
                     .WithResponseFormatter(user => new UserDto(user))
                     .AsCreated(user => $"/api/users/{user.Id}");

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
        => await _svc.DeleteUserAsync(id).AsActionResultBuilder();
}
```

## Error Handling

The built-in default mapper converts standard failure types to RFC 7807 Problem Details responses:

| Failure Type             | HTTP Status               |
|--------------------------|---------------------------|
| `ValidationFailure`      | 400 Bad Request           |
| `NotFoundFailure`        | 404 Not Found             |
| `UnauthorizedFailure`    | 401 Unauthorized          |
| `UnauthenticatedFailure` | 403 Forbidden             |
| `ConflictFailure`        | 409 Conflict              |
| `ExceptionalFailure`     | 500 Internal Server Error |
| Unrecognised `Failure`   | 500 Internal Server Error |

Example error response (RFC 7807 Problem Details):

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "Email is required; Password too short",
  "code": "VALIDATION",
  "errors": { "Email": ["Email is required"], "Password": ["Password too short"] },
  "errorCodes": { "Email": ["Required"] }
}
```

`errors` groups messages by field (errors without a field use the `""` key). `errorCodes` is present only when at least one `ValidationError` has a `Code`.

## Configuration

### Global policy defaults

```csharp
builder.Services.AddResultHttp(options =>
{
    // Result (non-generic) success: 204 NoContent (default) or 200 OK
    options.Policy = new ResultHttpAdapterPolicy
    {
        ResultSuccessBehavior = ResultSuccessHttpBehavior.NoContent, // default
        MaybeNoneBehavior     = MaybeNoneHttpBehavior.NotFound       // default
    };

    // Include the failure message, exception type and stack trace in 500 responses (development only)
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();
});
```

#### `ResultSuccessHttpBehavior`

| Value                 | Behaviour                                     |
|-----------------------|-----------------------------------------------|
| `NoContent` (default) | Non-generic `Result` success → 204 No Content |
| `Ok`                  | Non-generic `Result` success → 200 OK         |

#### `MaybeNoneHttpBehavior`

| Value                | Behaviour                        |
|----------------------|----------------------------------|
| `NotFound` (default) | `Maybe<T>` None → 404 Not Found  |
| `NoContent`          | `Maybe<T>` None → 204 No Content |

### Per-endpoint override

The builders override the global policy and mappers at any call site:

```csharp
// Status code on success, instead of the policy default
app.MapPost("/publish", async (IPublishService svc) =>
    await svc.PublishAsync().AsHttpBuilder().WithStatusCode(StatusCodes.Status200OK));

// Response on None, instead of the policy default
app.MapGet("/drafts/{id}", async (string id, IDraftService svc) =>
    await svc.TryGetAsync(id).AsHttpBuilder().AsNone(() => Results.NoContent()));

// Explicit mapper, instead of the one registered by AddResultHttp
app.MapGet("/legacy/{id}", async (string id, ILegacyService svc) =>
    await svc.GetAsync(id).AsHttpBuilder(new LegacyFailureMapper()));
```

### Custom error mappers

#### Typed helper (most common)

Register a handler for a specific failure type using a status code or a factory:

```csharp
builder.Services.AddResultHttp(options =>
{
    // Simple: status code + default ProblemDetails body
    options.AddMapper<RateLimitFailure>(429);
    options.AddMapper<PaymentRequiredFailure>(402);

    // Full control: custom factory
    options.AddMapper<RateLimitFailure>(f => new FailureHttpResponse
    {
        StatusCode = 429,
        Body = new { message = f.Message, retryAfter = f.RetryAfterSeconds },
        Headers = new Dictionary<string, string>
        {
            ["Retry-After"] = f.RetryAfterSeconds.ToString()
        }
    });
});
```

Typed mappers participate in the same first-match-wins chain and fall through to the default mapper for unhandled types.

#### Full `IFailureHttpMapper` implementation

Use this when one mapper needs to handle several related failure types:

```csharp
public sealed class DomainErrorMapper : IFailureHttpMapper
{
    public FailureHttpResponse? GetFailureResponse(IFailure failure) =>
        failure switch
        {
            RateLimitFailure f => new FailureHttpResponse
            {
                StatusCode = 429,
                Body       = new { message = f.Message },
                Headers    = new Dictionary<string, string> { ["Retry-After"] = "60" }
            },
            PaymentRequiredFailure f => new FailureHttpResponse
            {
                StatusCode = 402,
                Body       = new { message = f.Message, subscriptionRequired = true }
            },
            _ => null  // return null to pass to the next mapper in the chain
        };
}

builder.Services.AddResultHttp(options =>
{
    options.AddMapper(new DomainErrorMapper());
});
```

Custom mappers registered via `AddMapper` are tried in the order they are added, before the built-in default mapper.

## API Reference

### Minimal API (`IResult`) — namespace `UnambitiousFx.Functional.AspNetCore.Http`

| Entry point                                 | Available on                                                                         |
|---------------------------------------------|--------------------------------------------------------------------------------------|
| `AsHttpBuilder(failureMapper?)`             | `Result`, `ValueTask<Result>`, `Result<T>`, `ValueTask<Result<T>>`                   |
| `AsHttpBuilder()`                           | `Maybe<T>`, `ValueTask<Maybe<T>>`                                                    |

| Builder method                   | Result | Result&lt;T&gt; | Maybe&lt;T&gt; | Effect                                                          |
|----------------------------------|:------:|:---------------:|:--------------:|-----------------------------------------------------------------|
| `WithStatusCode(int)`            |   ✓    |        ✓        |       ✓        | Success status code (default `200`; `Result`: `204`/`200` per policy) |
| `WithHeader(key, value)`         |   ✓    |        ✓        |       ✓        | Adds a response header on success                               |
| `WithResponseFormatter(v => …)`  |        |        ✓        |       ✓        | Maps the value to the response body                             |
| `AsCreated(v => location)`       |        |        ✓        |       ✓        | `201 Created` with a `Location` header                          |
| `AsNone(() => IResult)`          |        |                 |       ✓        | Response on None (default `404`/`204` per policy)               |

Failures go through `failureMapper` when given, else the mapper registered by `AddResultHttp`, else the default mapper.

---

### MVC (`IActionResult`) — namespace `UnambitiousFx.Functional.AspNetCore.Mvc`

`AsActionResultBuilder(failureMapper?)` on the same types, with the same builder methods. `AsNone` takes a
`Func<IActionResult>`.

---

### `ResultHttpOptions` configuration

| Member                                                   | Type                      | Default   | Description                                     |
|----------------------------------------------------------|---------------------------|-----------|-------------------------------------------------|
| `Policy`                                                 | `ResultHttpAdapterPolicy` | `Default` | Global success/none behavior defaults           |
| `IncludeExceptionDetails`                                | `bool`                    | `false`   | Include message, exception type and stack trace in 500 bodies |
| `AddMapper(IFailureHttpMapper)`                            | —                         | —         | Register a custom mapper instance               |
| `AddMapper<TFailure>(int statusCode)`                    | —                         | —         | Register a typed mapper returning a status code |
| `AddMapper<TFailure>(Func<TFailure, FailureHttpResponse>)` | —                         | —         | Register a typed mapper with a custom factory   |

---

### `IFailureHttpMapper` contract

```csharp
public interface IFailureHttpMapper
{
    /// <summary>
    /// Returns a response for the given failure, or null to pass to the next mapper.
    /// </summary>
    FailureHttpResponse? GetFailureResponse(IFailure failure);
}
```

`FailureHttpResponse` properties: `StatusCode` (required), `Body`, `Headers`, `ContentType`.

## Migration from v1

| v1                                                              | v2                                                                                    |
|-----------------------------------------------------------------|---------------------------------------------------------------------------------------|
| `result.ToHttpResult()`                                         | `await result.AsHttpBuilder()`                                                        |
| `result.ToHttpResult(v => dto)`                                 | `await result.AsHttpBuilder().WithResponseFormatter(v => dto)`                        |
| `result.ToCreatedHttpResult(loc)`                               | `await result.AsHttpBuilder().AsCreated(loc)`                                         |
| `result.ToActionResult()`                                       | `await result.AsActionResultBuilder()`                                                |
| `result.ToCreatedActionResult(...)`                             | `await result.AsActionResultBuilder().AsCreated(v => location)`                       |
| `options.UseProblemDetails = true`                              | Removed — Problem Details is always on                                                |
| Implicit `IFailureHttpMapper.GetStatusCode` / `GetResponseBody` | Implement `GetFailureResponse(IFailure)` → `FailureHttpResponse?`                     |

## Thread Safety

All components are thread-safe and designed for concurrent use in ASP.NET Core applications.

## AOT Compatibility

This library is fully compatible with Native AOT compilation. All error mappings use static patterns that do not require
runtime code generation.

## License

This library is part of the UnambitiousFx project and follows the same license.
