# UnambitiousFx.Functional Examples

This directory contains example projects demonstrating how to use the UnambitiousFx.Functional library in various scenarios.

## Examples

### 1. BasicUsage

A console application demonstrating the core features of the library:

- `Result<T>` success and failure cases
- Pattern matching with `Match()`
- `Maybe<T>` for optional values
- Chaining operations with `Bind()`
- Different error types (NotFound, Validation, Unauthorized)
- Metadata support
- Side effects with `IfSuccess()` and `IfFailure()`

**Run the example:**

```bash
cd examples/BasicUsage
dotnet run
```

### 2. AspNetCoreExample

A minimal API demonstrating integration with ASP.NET Core:

- Converting `Result<T>` to `IResult` with `ToHttpResult()`
- Converting `Maybe<T>` to `Result<T>` then to `IResult`
- Automatic HTTP status code mapping for different error types
- Validation error handling
- Authorization error handling
- RESTful API endpoints

**Run the example:**

```bash
cd examples/AspNetCoreExample
dotnet run
```

Then test the API endpoints using curl or your browser.

**Example API calls:**

```bash
# Success case - divide numbers
curl https://localhost:5001/api/divide/10/2

# Error case - divide by zero
curl https://localhost:5001/api/divide/10/0

# Success case - get user
curl https://localhost:5001/api/users/123

# Not found case - unknown user
curl https://localhost:5001/api/users/999

# Success case - get config value
curl https://localhost:5001/api/config/app-name

# Not found case - missing config
curl https://localhost:5001/api/config/unknown-key

# Success case - create user
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Jane Doe","email":"jane@example.com"}'

# Validation error - invalid email
curl -X POST https://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{"name":"Jane Doe","email":"invalid-email"}'

# Unauthorized error - admin endpoint
curl https://localhost:5001/api/admin/users/123
```

## Key Concepts Demonstrated

### Result<T>

Railway-oriented programming with `Result<T>` allows you to handle success and failure cases explicitly:

```csharp
var result = Divide(10, 2);
result.Match(
    onSuccess: value => Console.WriteLine($"Result: {value}"),
    onFailure: error => Console.WriteLine($"Error: {error.Message}")
);
```

### Maybe<T>

Optional values with `Maybe<T>` eliminate null reference exceptions:

```csharp
var user = FindUser("john");
user
    .IfSome(u => Console.WriteLine($"Found: {u.Name}"))
    .IfNone(() => Console.WriteLine("User not found"));
```

### Chaining Operations

Chain multiple operations that can fail:

```csharp
var result = Result.Success("42")
    .Bind(ParseInt)
    .Bind(MultiplyByTwo)
    .Bind(ValidatePositive);
```

### Error Types

Use specific error types for different failure scenarios:

```csharp
// Not found
Result.Failure<User>(new NotFoundError("User", id));

// Validation
Result.Failure<User>(new ValidationError("Email is required"));

// Unauthorized
Result.Failure<User>(new UnauthorizedError("Admin access required"));
```

### ASP.NET Core Integration

Seamlessly convert functional types to HTTP results:

```csharp
app.MapGet("/api/users/{id}", (string id) =>
{
    var result = GetUser(id);
    return result.ToHttpResult(); // Automatic status code mapping
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
