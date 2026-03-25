using UnambitiousFx.Functional;
using UnambitiousFx.Functional.AspNetCore;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.Failures;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddResultHttp();

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
app.MapGet("/api/profiles/{id}", (string id) =>
{
    var profile = TryGetProfile(id);
    return profile.ToHttpResult();
})
.WithName("GetProfile")
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
        return Result.Fail<double>(new ValidationFailure(["Cannot divide by zero"]));

    return Result.Ok((double)numerator / denominator);
}

static Result<User> GetUser(string id)
{
    if (string.IsNullOrWhiteSpace(id))
        return Result.Fail<User>(new ValidationFailure(["User ID is required"]));

    if (id == "123")
        return Result.Ok(new User(id, "John Doe", "john@example.com"));

    return Result.Fail<User>(new NotFoundFailure("User", id));
}

static Maybe<UserProfile> TryGetProfile(string id)
{
    if (string.IsNullOrWhiteSpace(id))
    {
        return Maybe.None<UserProfile>();
    }

    var profiles = new Dictionary<string, UserProfile>
    {
        ["123"] = new UserProfile("123", "Engineering", "UTC"),
        ["456"] = new UserProfile("456", "Design", "Europe/Paris")
    };

    return profiles.TryGetValue(id, out var value)
        ? Maybe.Some(value)
        : Maybe.None<UserProfile>();
}

static Result<User> CreateUser(CreateUserRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Result.Fail<User>(new ValidationFailure(["Name is required"]));

    if (string.IsNullOrWhiteSpace(request.Email))
        return Result.Fail<User>(new ValidationFailure(["Email is required"]));

    if (!request.Email.Contains('@'))
        return Result.Fail<User>(new ValidationFailure(["Invalid email format"]));

    var user = new User(Guid.NewGuid().ToString(), request.Name, request.Email);
    return Result.Ok(user);
}

static Result<User> GetUserWithAuth(string id)
{
    // Simulate authorization check
    var isAuthorized = false; // In real app, check claims/tokens

    if (!isAuthorized)
        return Result.Fail<User>(new UnauthorizedFailure("Admin access required"));

    return GetUser(id);
}

// ============================================================================
// Models
// ============================================================================

record User(string Id, string Name, string Email);
record UserProfile(string UserId, string Department, string TimeZone);
record CreateUserRequest(string Name, string Email);
