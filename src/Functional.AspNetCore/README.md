# Functional.AspNetCore

ASP.NET Core integration for UnambitiousFx `Result<T>` types. This library provides seamless conversion from `Result` types to HTTP responses for both minimal APIs and MVC controllers.

## Features

- ✅ **AOT-Compatible**: Designed for Native AOT compilation
- ✅ **Minimal API Support**: Convert `Result<T>` to `IResult`
- ✅ **MVC Controller Support**: Convert `Result<T>` to `IActionResult`
- ✅ **DTO Mapping**: Optional transformation layer for response DTOs
- ✅ **Extensible Error Mapping**: Custom error-to-HTTP status code mappers
- ✅ **Problem Details Support**: RFC 7807 compliant error responses
- ✅ **Built-in Error Types**: Automatic mapping for ValidationError, NotFoundError, UnauthorizedError, etc.

## Installation

```bash
dotnet add package UnambitiousFx.Functional.AspNetCore
```

## Quick Start

### Minimal API

```csharp
using UnambitiousFx.Functional.AspNetCore.Extensions;
using UnambitiousFx.Functional.Results;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResultHttp();

var app = builder.Build();

// Simple success/failure
app.MapGet("/users/{id:guid}", async (Guid id, IUserService userService) =>
{
    var result = await userService.GetUserAsync(id);
    return result.ToHttpResult();
});

// With DTO mapping
app.MapGet("/users/{id:guid}", async (Guid id, IUserService userService) =>
{
    var result = await userService.GetUserAsync(id);
    return result.ToHttpResult(user => new UserDto(user.Id, user.Email));
});

// Created response
app.MapPost("/users", async (CreateUserRequest request, IUserService userService) =>
{
    var result = await userService.CreateUserAsync(request);
    return result.ToCreatedHttpResult(
        user => $"/users/{user.Id}",
        user => new UserDto(user.Id, user.Email));
});

app.Run();
```

### MVC Controllers

```csharp
using Microsoft.AspNetCore.Mvc;
using UnambitiousFx.Functional.AspNetCore.Extensions;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await _userService.GetUserAsync(id);
        return result.ToActionResult(user => new UserDto(user.Id, user.Email));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        return result.ToCreatedActionResult(
            "GetUser",
            user => new UserDto(user.Id, user.Email),
            new { id = result.TryGet(out var user, out _) ? user.Id : Guid.Empty });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result.ToActionResult();
    }
}
```

## Error Handling

The library automatically maps standard error types to appropriate HTTP status codes:

| Error Type | HTTP Status Code |
|------------|------------------|
| `ValidationError` | 400 Bad Request |
| `NotFoundError` | 404 Not Found |
| `UnauthorizedError` | 401 Unauthorized |
| `ConflictError` | 409 Conflict |
| `ExceptionalError` | 500 Internal Server Error |
| Custom `Error` | 400 Bad Request |

### Example Error Responses

```csharp
// Validation error → 400 Bad Request
var result = Result.Failure<User>(
    new ValidationError(["Email is required", "Password must be at least 8 characters"]));
return result.ToHttpResult();

// Not found → 404 Not Found
var result = Result.Failure<User>(
    new NotFoundError("User", userId.ToString()));
return result.ToHttpResult();

// Unauthorized → 401 Unauthorized
var result = Result.Failure<User>(
    new UnauthorizedError("Invalid API key"));
return result.ToHttpResult();
```

## Configuration

### Basic Configuration

```csharp
builder.Services.AddResultHttp();
```

### With Problem Details (RFC 7807)

```csharp
builder.Services.AddResultHttp(options =>
{
    options.UseProblemDetails = true;
    options.IncludeExceptionDetails = builder.Environment.IsDevelopment();
});
```

Problem Details response example:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Failed",
  "status": 400,
  "detail": "Email is required; Password must be at least 8 characters",
  "code": "VALIDATION",
  "failures": [
    "Email is required",
    "Password must be at least 8 characters"
  ]
}
```

### Custom Error Mappers

Create a custom mapper for domain-specific errors:

```csharp
public class DomainErrorMapper : IErrorHttpMapper
{
    public int? GetStatusCode(IError error)
    {
        return error switch
        {
            PaymentRequiredError => 402,
            RateLimitError => 429,
            _ => null // Let other mappers handle it
        };
    }

    public object? GetResponseBody(IError error)
    {
        return error switch
        {
            PaymentRequiredError payment => new
            {
                error = payment.Code,
                message = payment.Message,
                subscriptionRequired = true
            },
            RateLimitError rateLimit => new
            {
                error = rateLimit.Code,
                message = rateLimit.Message,
                retryAfter = rateLimit.RetryAfter
            },
            _ => null
        };
    }
}

// Register custom mapper
builder.Services.AddResultHttp(options =>
{
    options.AddMapper(new DomainErrorMapper());
});
```

Custom mappers are evaluated before the default mapper, allowing you to override behavior for specific error types.

## API Reference

### Extension Methods

#### Minimal API (`IResult`)

- `ToHttpResult()` - Convert `Result` to 200 OK or error status
- `ToHttpResult<TValue>()` - Convert `Result<T>` to 200 OK with value or error status
- `ToHttpResult<TValue, TDto>()` - Convert and map to DTO
- `ToCreatedHttpResult<TValue>()` - Convert to 201 Created with location
- `ToCreatedHttpResult<TValue, TDto>()` - Convert to 201 Created with DTO mapping

#### MVC Controllers (`IActionResult`)

- `ToActionResult()` - Convert `Result` to `OkResult` or error result
- `ToActionResult<TValue>()` - Convert `Result<T>` to `OkObjectResult` or error result
- `ToActionResult<TValue, TDto>()` - Convert and map to DTO
- `ToCreatedActionResult<TValue>()` - Convert to `CreatedAtActionResult`
- `ToCreatedActionResult<TValue, TDto>()` - Convert with DTO mapping

All extension methods accept an optional `IErrorHttpMapper` parameter for custom error handling.

### Configuration Options

- `UseProblemDetails` - Enable RFC 7807 Problem Details format (default: false)
- `IncludeExceptionDetails` - Include stack traces in responses (default: false, use only in development)
- `AddMapper(IErrorHttpMapper)` - Add custom error mapper

## Best Practices

1. **Use DTO Mapping**: Transform domain entities to DTOs at the boundary
   ```csharp
   return result.ToHttpResult(user => new UserDto(user));
   ```

2. **Register Result HTTP Services**: Always call `AddResultHttp()` in your DI configuration
   ```csharp
   builder.Services.AddResultHttp(options => { /* configure */ });
   ```

3. **Enable Problem Details in Production**: Use RFC 7807 for consistent error responses
   ```csharp
   options.UseProblemDetails = true;
   ```

4. **Create Domain-Specific Error Types**: Extend `ErrorBase` for custom errors
   ```csharp
   public sealed record PaymentRequiredError(string Message)
       : ErrorBase("PAYMENT_REQUIRED", Message);
   ```

5. **Use Specific Error Types**: Prefer `ValidationError`, `NotFoundError`, etc. over generic `Error`
   ```csharp
   // Good
   return Result.Failure<User>(new NotFoundError("User", id));

   // Avoid
   return Result.Failure<User>("User not found");
   ```

## Thread Safety

All components are thread-safe and designed for concurrent use in ASP.NET Core applications.

## AOT Compatibility

This library is fully compatible with Native AOT compilation. All error mappings use static patterns that don't require runtime code generation.

## License

This library is part of the UnambitiousFx project and follows the same license.
