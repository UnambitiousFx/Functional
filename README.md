# UnambitiousFx.Functional

[![Build Status](https://github.com/UnambitiousFx/Functional/workflows/CI/badge.svg)](https://github.com/UnambitiousFx/Functional/actions)
[![NuGet](https://img.shields.io/nuget/v/UnambitiousFx.Functional.svg)](https://www.nuget.org/packages/UnambitiousFx.Functional/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/UnambitiousFx.Functional.svg)](https://www.nuget.org/packages/UnambitiousFx.Functional/)
[![codecov](https://codecov.io/gh/UnambitiousFx/Functional/branch/main/graph/badge.svg)](https://codecov.io/gh/UnambitiousFx/Functional)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)

A lightweight, modern functional programming library for .NET that makes error handling and optional values elegant and type-safe.

## 🔧 Compatibility & support

- **Dependency-free**: No external runtime dependencies — the library is dependency-less at runtime.
- **Supported .NET versions**: Supports all Microsoft LTS releases **and** the latest non-LTS release. See the CI matrix or the Releases page for exact versions.

## 🎯 Features

- **`Result<T>`** - Railway-oriented programming for error handling without exceptions
- **`Maybe<T>`** - Type-safe optional values, no more null reference exceptions
- **`OneOf<T1..T10>`** - Discriminated unions via source generators
- **Rich Error Types** - `ValidationError`, `NotFoundError`, `ConflictError`, `UnauthorizedError`, and more
- **Comprehensive Extensions** - Bind, Map, Match, Tap, Ensure, Recover, and dozens more
- **Async First** - Full support for `Task<T>` and `ValueTask<T>`
- **ASP.NET Core Integration** - Convert results to HTTP responses effortlessly
- **Metadata Support** - Attach contextual information to results
- **xUnit Testing Utilities** - Fluent assertions for functional types
- **Performance Focused** - Zero allocation where possible, benchmarks included
- **Debugger Friendly** - Enhanced debugging experience with custom visualizers

## 📦 Installation

```bash
dotnet add package UnambitiousFx.Functional
```

For ASP.NET Core integration:
```bash
dotnet add package UnambitiousFx.Functional.AspNetCore
```

For xUnit testing utilities:
```bash
dotnet add package UnambitiousFx.Functional.xunit
```

## 🚀 Quick Start

### Result<T> - Error Handling Made Simple

Instead of throwing exceptions:

```csharp
using UnambitiousFx.Functional;

// Traditional approach with exceptions
public User GetUser(int id)
{
    var user = _repository.Find(id);
    if (user is null)
        throw new NotFoundException($"User {id} not found");
    return user;
}

// Functional approach with Result<T>
public Result<User> GetUser(int id)
{
    var user = _repository.Find(id);
    return user is not null
        ? Result.Success(user)
        : Result.Failure<User>(new NotFoundError($"User {id} not found"));
}
```

### Pattern Matching

```csharp
var result = GetUser(42);

result.Match(
    success: user => Console.WriteLine($"Found: {user.Name}"),
    failure: error => Console.WriteLine($"Error: {error.Message}")
);
```

### Chaining Operations (Railway-Oriented Programming)

```csharp
public Result<Order> CreateOrder(int userId, OrderRequest request)
{
    return GetUser(userId)
        .Ensure(user => user.IsActive, new ValidationError("User is not active"))
        .Bind(user => ValidateOrder(request))
        .Bind(validOrder => SaveOrder(validOrder))
        .Tap(order => _eventBus.Publish(new OrderCreated(order.Id)))
        .WithMetadata("userId", userId);
}
```

### Maybe<T> - Optional Values Without Null

```csharp
public Maybe<User> FindUserByEmail(string email)
{
    var user = _repository.FindByEmail(email);
    return user is not null
        ? Maybe.Some(user)
        : Maybe.None<User>();
}

// Pattern matching
var maybeUser = FindUserByEmail("user@example.com");
maybeUser.Match(
    some: user => Console.WriteLine($"Found: {user.Name}"),
    none: () => Console.WriteLine("User not found")
);

// Convert to Result
var result = maybeUser.ToResult(new NotFoundError("User not found"));
```

### OneOf<T> - Discriminated Unions

```csharp
// Represent a value that can be one of several types
public OneOf<Success, ValidationError, NotFoundError> ProcessRequest(int id)
{
    if (id <= 0)
        return new ValidationError("Invalid ID");

    var item = _repository.Find(id);
    if (item is null)
        return new NotFoundError($"Item {id} not found");

    return new Success();
}

// Pattern match on all cases
var response = ProcessRequest(id);
response.Match(
    first: success => Ok(),
    second: validation => BadRequest(validation.Message),
    third: notFound => NotFound(notFound.Message)
);
```

## 📚 Core Concepts

### Result<T> Operations

#### Transformation
```csharp
// Map - Transform success value
Result<int> result = Result.Success(5);
Result<int> doubled = result.Map(x => x * 2); // Success(10)

// Bind - Chain operations that return Result
Result<User> userResult = GetUser(id);
Result<Order> orderResult = userResult.Bind(user => CreateOrder(user));

// Flatten - Unwrap nested results
Result<Result<int>> nested = Result.Success(Result.Success(42));
Result<int> flat = nested.Flatten(); // Success(42)
```

#### Error Handling
```csharp
// Recover - Provide fallback on error
Result<User> result = GetUser(id)
    .Recover(error => GetDefaultUser());

// MapError - Transform error
Result<User> result = GetUser(id)
    .MapError(error => new CustomError(error.Message));

// Ensure - Add validation
Result<User> result = GetUser(id)
    .Ensure(user => user.Age >= 18, new ValidationError("Must be 18+"));
```

#### Side Effects
```csharp
// Tap - Execute side effect on success
Result<Order> result = CreateOrder(request)
    .Tap(order => _logger.LogInformation($"Order {order.Id} created"));

// TapError - Execute side effect on failure
Result<Order> result = CreateOrder(request)
    .TapError(error => _logger.LogError($"Failed: {error.Message}"));

// TapBoth - Execute side effect regardless of result
Result<Order> result = CreateOrder(request)
    .TapBoth(
        success: order => _metrics.RecordSuccess(),
        failure: error => _metrics.RecordFailure()
    );
```

#### Value Access
```csharp
// TryGet - Safe value extraction
if (result.TryGet(out var value, out var error))
{
    Console.WriteLine($"Success: {value}");
}
else
{
    Console.WriteLine($"Error: {error.Message}");
}

// ValueOr - Provide default value
var user = result.ValueOr(GetDefaultUser());

// ValueOrThrow - Get value or throw
var user = result.ValueOrThrow(); // Throws if failed
```

### Error Types

```csharp
// Basic error
var error = new Error("Something went wrong");

// Validation error with field details
var validationError = new ValidationError("Invalid input", new Dictionary<string, object?>
{
    ["Email"] = "Invalid email format",
    ["Age"] = "Must be at least 18"
});

// Not found error
var notFoundError = new NotFoundError("User not found");

// Conflict error
var conflictError = new ConflictError("Email already exists");

// Unauthorized error
var unauthorizedError = new UnauthorizedError("Access denied");

// Exceptional error (wrap exceptions)
try
{
    // risky operation
}
catch (Exception ex)
{
    return Result.Failure(new ExceptionalError(ex));
}

// Aggregate error (multiple errors)
var errors = new List<Error>
{
    new ValidationError("Invalid name"),
    new ValidationError("Invalid email")
};
var aggregateError = new AggregateError(errors);
```

### Metadata

Attach contextual information to results:

```csharp
var result = Result.Success(user)
    .WithMetadata("requestId", requestId)
    .WithMetadata("timestamp", DateTime.UtcNow)
    .WithMetadata("source", "UserService");

// Access metadata
if (result.Metadata.TryGetValue("requestId", out var requestId))
{
    _logger.LogInformation($"Request {requestId} completed");
}

// Fluent builder
var result = Result.Success(user)
    .WithMetadata(builder => builder
        .Add("requestId", requestId)
        .Add("duration", stopwatch.ElapsedMilliseconds)
        .Add("cacheHit", false));
```

### Async Support

All operations work seamlessly with `Task<T>` and `ValueTask<T>`:

```csharp
public async Task<Result<Order>> CreateOrderAsync(int userId, OrderRequest request)
{
    return await GetUserAsync(userId)
        .Bind(user => ValidateOrderAsync(request))
        .Bind(validOrder => SaveOrderAsync(validOrder))
        .Tap(order => PublishEventAsync(order));
}

// Works with ValueTask too
public async ValueTask<Result<User>> GetUserAsync(int id)
{
    var user = await _repository.FindAsync(id);
    return user is not null
        ? Result.Success(user)
        : Result.Failure<User>(new NotFoundError($"User {id} not found"));
}
```

## 🌐 ASP.NET Core Integration

```csharp
using UnambitiousFx.Functional.AspNetCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Automatically converts Result to appropriate HTTP response
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        return await _userService
            .GetUserAsync(id)
            .ToHttpResult(); // 200 OK or 404 Not Found
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        return await _userService
            .CreateUserAsync(request)
            .ToHttpResult(); // 200 OK, 400 Bad Request, or 409 Conflict
    }
}
```

Configure error mapping:

```csharp
services.AddResultHttp(options =>
{
    options.MapError<ValidationError>(StatusCodes.Status400BadRequest);
    options.MapError<NotFoundError>(StatusCodes.Status404NotFound);
    options.MapError<ConflictError>(StatusCodes.Status409Conflict);
    options.MapError<UnauthorizedError>(StatusCodes.Status401Unauthorized);
});
```

## 🧪 Testing with xUnit

```csharp
using UnambitiousFx.Functional.xunit;

[Fact]
public void CreateUser_WithValidData_ReturnsSuccess()
{
    // Arrange
    var request = new CreateUserRequest { Name = "John", Email = "john@example.com" };

    // Act
    var result = _service.CreateUser(request);

    // Assert
    result.Should().BeSuccess()
        .Which(user => user.Name.Should().Be("John"));
}

[Fact]
public void CreateUser_WithInvalidEmail_ReturnsValidationError()
{
    // Arrange
    var request = new CreateUserRequest { Name = "John", Email = "invalid" };

    // Act
    var result = _service.CreateUser(request);

    // Assert
    result.Should().BeFailure()
        .WithError<ValidationError>()
        .Which(error => error.Message.Should().Contain("email"));
}

[Fact]
public void FindUser_WhenNotExists_ReturnsNone()
{
    // Act
    var maybe = _repository.FindByEmail("notfound@example.com");

    // Assert
    maybe.Should().BeNone();
}
```

## 🎓 Philosophy

This library embraces:

- **Railway-Oriented Programming** - Operations form a "railway track" where success continues down the happy path and errors short-circuit to the failure path
- **Explicit Error Handling** - Errors are values, not exceptions. They're part of your type signatures
- **Composition Over Conditionals** - Chain operations naturally using Bind, Map, and other combinators
- **Type Safety** - Compiler enforces error handling, reducing runtime surprises
- **Functional Core, Imperative Shell** - Pure functional core with pragmatic integration for .NET ecosystem

## 📊 Performance

We take performance seriously. Check our [benchmarks](benchmarks/FunctionalBenchmark) comparing against:
- Traditional exception-based code
- FluentResults
- Ardalis.Result

Key highlights:
- Zero allocations for success paths in many scenarios
- `readonly struct` value types minimize heap pressure
- Lazy error aggregation
- Optimized async state machines

## 🤝 Contributing

We welcome contributions! Please read our [Contributing Guide](CONTRIBUTING.md) for details on:

- Code of conduct
- Development setup
- Coding standards
- Testing requirements
- Pull request process

Check out our [good first issues](https://github.com/UnambitiousFx/Functional/labels/good-first-issue) to get started!

## 📝 Release notes

See the Releases page on GitHub for detailed release notes and version history: https://github.com/UnambitiousFx/Functional/releases

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

Inspired by:
- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/) by Scott Wlaschin
- F# Result type
- Rust's Result and Option types
- [LanguageExt](https://github.com/louthy/language-ext)
- [FluentResults](https://github.com/altmann/FluentResults)

## 📚 Further Reading

- [Railway Oriented Programming](https://fsharpforfunandprofit.com/rop/)
- [Functional Error Handling](https://fsharpforfunandprofit.com/posts/recipe-part2/)
- [The Power of Composition](https://fsharpforfunandprofit.com/composition/)

---

Made with ❤️ by the UnambitiousFx team
