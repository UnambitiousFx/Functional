# UnambitiousFx.Functional Examples

This directory contains example projects demonstrating how to use the UnambitiousFx.Functional library in various scenarios.

## Examples

### 1. BasicUsage

A comprehensive console application demonstrating the core features of the library:

**Topics covered:**

- `Result<T>` success and failure cases
- Pattern matching with `Match()`
- `Maybe<T>` for optional values (no null reference exceptions)
- Railway-oriented programming with `Bind()` for chaining operations
- `Ensure()` for validation with predicates
- `Recover()` for error handling and recovery
- `Tap()` for side effects without changing values
- `Filter()` for filtering `Maybe<T>` values
- Different failure types: `ValidationFailure`, `NotFoundFailure`, `ConflictFailure`, `UnauthorizedFailure`, `TimeoutFailure`
- `Metadata` support for attaching contextual information to results
- `IfSuccess()` and `IfFailure()` for conditional execution
- `Map()` for transforming values
- `Try()` for catching exceptions and converting them to functional results

**Run the example:**

```bash
cd examples/BasicUsage
dotnet run
```

**Key concepts:**

Railway-Oriented Programming (ROP) is a functional pattern that treats functions as railway tracks:

- Success values flow on the happy path (upper track)
- Failures flow on the failure path (lower track)
- Operations are composed so that failures automatically short-circuit subsequent operations

```csharp
// Instead of throwing exceptions and using try-catch:
public Result<Order> CreateOrder(int userId, OrderRequest request)
{
    return GetUser(userId)
        .Ensure(user => user.IsActive, _ => new ValidationFailure(["User is not active"]))
        .Bind(user => ValidateOrder(request))
        .Bind(validOrder => SaveOrder(validOrder))
        .Tap(order => LogOrderCreated(order))
        .WithMetadata("userId", userId);
}
```

### 2. AspNetCoreExample

A minimal ASP.NET Core API demonstrating integration with web frameworks:

**Topics covered:**

- Converting `Result<T>` to HTTP responses with `AsHttpBuilder()`
- Converting `Maybe<T>` directly to HTTP responses (404 for None)
- Automatic HTTP status code mapping for different failure types:
  - `ValidationFailure` → 400 Bad Request
  - `NotFoundFailure` → 404 Not Found
  - `UnauthorizedFailure` → 403 Forbidden
  - `UnauthenticatedFailure` → 401 Unauthorized
  - `ConflictFailure` → 409 Conflict
  - `ExceptionalFailure` → 500 Internal Server Error
- Authentication and authorization checks
- Async operations with functional results
- Complex validation with multiple error collection
- Custom status codes and response transformations
- RESTful API design with functional results

**Run the example:**

```bash
cd examples/AspNetCoreExample
dotnet run
```

Then test the API endpoints:

```bash
# Example 1: Simple division (success case)
curl http://localhost:5000/api/divide/10/2

# Example 1: Division by zero (validation failure - returns 400)
curl http://localhost:5000/api/divide/10/0

# Example 2: Get existing user (returns 200)
curl http://localhost:5000/api/users/1

# Example 2: Get non-existent user (returns 404)
curl http://localhost:5000/api/users/999

# Example 3: Create user (success case)
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Alice","email":"alice@example.com"}'

# Example 3: Create user with validation errors (returns 400)
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"","email":"invalid"}'

# Example 3: Create user with conflicting email (returns 409)
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Bob","email":"taken@example.com"}'

# Example 4: Get user details without authentication (returns 401)
curl http://localhost:5000/api/users/1/details

# Example 4: Get user details with authentication
curl -H "Authorization: Bearer token" http://localhost:5000/api/users/1/details

# Example 5: Delete user without admin role (returns 403)
curl -X DELETE http://localhost:5000/api/users/1 \
  -H "Authorization: Bearer token"

# Example 5: Delete user with admin role (returns 204)
curl -X DELETE http://localhost:5000/api/users/1 \
  -H "Authorization: Bearer token" \
  -H "X-Admin-Role: true"

# Example 6: Bulk create users
curl -X POST http://localhost:5000/api/users/bulk \
  -H "Content-Type: application/json" \
  -d '[{"name":"Carol","email":"carol@example.com"},{"name":"Dave","email":"dave@example.com"}]'

# Example 7: Send notification (async operation)
curl http://localhost:5000/api/users/1/send-notification
```

## Key Patterns Demonstrated

### Error Handling Pattern

```csharp
// Traditional approach (imperative, exceptions)
public User GetUser(int id)
{
    var user = repository.Find(id);
    if (user == null)
        throw new NotFoundException($"User {id} not found");
    return user;
}

// Functional approach (declarative, composable)
public Result<User> GetUser(int id)
{
    var user = repository.Find(id);
    return user != null
        ? Result.Success(user)
        : Result.Failure<User>(new NotFoundFailure("User", id.ToString()));
}
```

### Validation Pattern

```csharp
// Chaining multiple validations with Ensure
public Result<CreateUserRequest> ValidateCreateUserRequest(CreateUserRequest request)
{
    return Result.Success(request)
        .Ensure(
            r => !string.IsNullOrWhiteSpace(r.Name),
            _ => new ValidationFailure(["Name is required"]))
        .Ensure(
            r => !string.IsNullOrWhiteSpace(r.Email),
            _ => new ValidationFailure(["Email is required"]))
        .Ensure(
            r => r.Email.Contains("@"),
            _ => new ValidationFailure(["Email is invalid"]))
        .Ensure(
            r => !EmailExists(r.Email),
            _ => new ConflictFailure("Email already registered"));
}
```

### Side Effects Pattern

```csharp
// Using Tap for logging/side effects without changing the value
public Result<Order> CreateOrder(CreateOrderRequest request)
{
    return ValidateRequest(request)
        .Bind(validated => SaveOrder(validated))
        .Tap(order => LogOrderCreated(order))
        .Tap(order => PublishOrderCreatedEvent(order));
}
```

### Optional Values Pattern

```csharp
// Using Maybe<T> instead of null checks
public Maybe<UserProfile> GetUserProfile(int userId)
{
    var profile = database.GetProfile(userId);
    return profile != null
        ? Maybe.Some(profile)
        : Maybe.None<UserProfile>();
}

// Chaining operations on optional values
var userEmail = userProfile
    .Bind(profile => profile.ContactInfo)
    .Bind(contactInfo => contactInfo.Email)
    .Match(
        some: email => $"Email: {email}",
        none: () => "No email available");
```

## Best Practices

1. **Use Result<T> for operations that can fail** - Avoid exceptions for control flow
2. **Use Maybe<T> for optional values** - Avoid null reference exceptions
3. **Chain operations with Bind() and Map()** - Compose operations functionally
4. **Use Ensure() for validation** - Keep validation logic clear and composable
5. **Use Tap() for side effects** - Logging, publishing events, etc.
6. **Use Pattern Matching** - Leverage Match() for readable success/failure handling
7. **Use Metadata** - Attach context (request IDs, timestamps) to results
8. **Return Result types from API methods** - Let the framework (ASP.NET Core, etc.) handle conversion

## Failure Types

The library provides several built-in failure types for common scenarios:

- **`ValidationFailure`** - For validation errors (HTTP 400)
- **`NotFoundFailure`** - For missing resources (HTTP 404)
- **`ConflictFailure`** - For conflicting operations (HTTP 409)
- **`UnauthorizedFailure`** - For insufficient permissions (HTTP 403)
- **`UnauthenticatedFailure`** - For missing authentication (HTTP 401)
- **`TimeoutFailure`** - For timeout errors (HTTP 408)
- **`ExceptionalFailure`** - For unexpected exceptions (HTTP 500)

## Further Reading

- [Railway-Oriented Programming](https://fsharpforfunandprofit.com/rop/) - Scott Wlaschin
- [UnambitiousFx.Functional Documentation](../README.md)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
  var user = FindUser("john");
  user
  .IfSome(u => Console.WriteLine($"Found: {u.Name}"))
  .IfNone(() => Console.WriteLine("User not found"));

````

### Chaining Operations

Chain multiple operations that can fail:

```csharp
var result = Result.Success("42")
    .Bind(ParseInt)
    .Bind(MultiplyByTwo)
    .Bind(ValidatePositive);
````

### Failure Types

Use specific failure types for different failure scenarios:

```csharp
// Not found
Result.Failure<User>(new NotFoundFailure("User", id));

// Validation
Result.Failure<User>(new ValidationFailure("Email is required"));

// Unauthorized
Result.Failure<User>(new UnauthorizedFailure("Admin access required"));
```

### ASP.NET Core Integration

Seamlessly convert functional types to HTTP results:

```csharp
app.MapGet("/api/users/{id}", (string id) =>
{
    var result = GetUser(id);
    return result.ToHttpResult(); // Automatic status code mapping
});

app.MapGet("/api/profiles/{id}", (string id) =>
{
  var maybe = TryGetProfile(id);
  return maybe.ToHttpResult(); // Some -> 200, None -> 404 (default)
});
```

## Learning Path

1. Start with **BasicUsage** to understand the core concepts
2. Explore **AspNetCoreExample** to see real-world API integration
3. Review the main README.md for comprehensive documentation
4. Check out the test projects for advanced usage patterns

## Additional Resources

- [Main Documentation](../README.md)
- [Contributing Guide](../CONTRIBUTING.md)
- [Changelog](../CHANGELOG.md)
