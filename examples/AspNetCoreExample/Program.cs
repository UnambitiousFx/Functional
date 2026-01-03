using UnambitiousFx.Functional;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.Errors;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// ============================================================================
// Example 1: Simple Result<T> to IResult conversion
// ============================================================================
app.MapGet("/api/divide/{numerator:int}/{denominator:int}", (int numerator, int denominator) =>
{
    var result = Divide(numerator, denominator);
    return result.ToHttpResult();
})
.WithName("Divide");

// ============================================================================
// Example 2: Result<T> with custom error mapping
// ============================================================================
app.MapGet("/api/users/{id}", (string id) =>
{
    var result = GetUser(id);
    return result.ToHttpResult();
})
.WithName("GetUser")
;

// ============================================================================
// Example 3: Maybe<T> to IResult conversion
// ============================================================================
app.MapGet("/api/config/{key}", (string key) =>
{
    var config = GetConfigValue(key);
    return config.ToResult(new NotFoundError("Config", key)).ToHttpResult();
})
.WithName("GetConfig")
;

// ============================================================================
// Example 4: Result with validation errors
// ============================================================================
app.MapPost("/api/users", (CreateUserRequest request) =>
{
    var result = CreateUser(request);
    return result.ToHttpResult();
})
.WithName("CreateUser")
;

// ============================================================================
// Example 5: Multiple error types
// ============================================================================
app.MapGet("/api/admin/users/{id}", (string id) =>
{
    var result = GetUserWithAuth(id);
    return result.ToHttpResult();
})
.WithName("GetUserWithAuth")
;

app.Run();

// ============================================================================
// Helper Methods
// ============================================================================

static Result<double> Divide(int numerator, int denominator)
{
    if (denominator == 0)
        return Result.Failure<double>(new ValidationError(["Cannot divide by zero"]));

    return Result.Success((double)numerator / denominator);
}

static Result<User> GetUser(string id)
{
    if (string.IsNullOrWhiteSpace(id))
        return Result.Failure<User>(new ValidationError(["User ID is required"]));

    if (id == "123")
        return Result.Success(new User(id, "John Doe", "john@example.com"));

    return Result.Failure<User>(new NotFoundError("User", id));
}

static Maybe<string> GetConfigValue(string key)
{
    var config = new Dictionary<string, string>
    {
        ["app-name"] = "AspNetCoreExample",
        ["version"] = "1.0.0"
    };

    return config.TryGetValue(key, out var value)
        ? Maybe.Some(value)
        : Maybe.None<string>();
}

static Result<User> CreateUser(CreateUserRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Result.Failure<User>(new ValidationError(["Name is required"]));

    if (string.IsNullOrWhiteSpace(request.Email))
        return Result.Failure<User>(new ValidationError(["Email is required"]));

    if (!request.Email.Contains('@'))
        return Result.Failure<User>(new ValidationError(["Invalid email format"]));

    var user = new User(Guid.NewGuid().ToString(), request.Name, request.Email);
    return Result.Success(user);
}

static Result<User> GetUserWithAuth(string id)
{
    // Simulate authorization check
    var isAuthorized = false; // In real app, check claims/tokens

    if (!isAuthorized)
        return Result.Failure<User>(new UnauthorizedError("Admin access required"));

    return GetUser(id);
}

// ============================================================================
// Models
// ============================================================================

record User(string Id, string Name, string Email);
record CreateUserRequest(string Name, string Email);
