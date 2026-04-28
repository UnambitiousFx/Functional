using UnambitiousFx.Functional;
using UnambitiousFx.Functional.AspNetCore.Http;
using UnambitiousFx.Functional.Failures;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  🚀 Welcome to Functional API Land 🚀                          ║");
Console.WriteLine("║     Where errors are caught, not thrown!                       ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

// ============================================================================
// Example 1: Simple Result<T> - Division endpoint (The Math Police)
// ============================================================================
app.MapGet("/api/divide/{numerator:int}/{denominator:int}",
    async (int numerator, int denominator) =>
    {
        var result = Divide(numerator, denominator);
        return await result.AsHttpBuilder();
    })
    .WithName("Divide")
    .Produces<double>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .WithSummary("Divide two numbers (Math: The Functional Way)");

// ============================================================================
// Example 2: Maybe<T> - Get user profile (returns 404 if not found)
// ============================================================================
app.MapGet("/api/users/{id:int}",
    async (int id) =>
    {
        var profile = GetUserProfile(id);
        return await profile.AsHttpBuilder();
    })
    .WithName("GetUser")
    .Produces<UserProfile>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithSummary("Get user profile by ID");

// ============================================================================
// Example 3: Result with validation errors (multiple field validation)
// ============================================================================
app.MapPost("/api/users",
    async (CreateUserRequest request) =>
    {
        var result = ValidateAndCreateUser(request);
        return await result.AsHttpBuilder();
    })
    .WithName("CreateUser")
    .Produces<UserProfile>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict)
    .WithSummary("Create a new user");

// ============================================================================
// Example 4: Authentication check - returns 401 for unauthenticated
// ============================================================================
app.MapGet("/api/users/{id:int}/details",
    async (int id, HttpContext context) =>
    {
        var authResult = CheckAuthentication(context);
        if (authResult.IsFailure)
        {
            return await authResult.AsHttpBuilder();
        }

        var profileResult = GetUserProfile(id);
        return await profileResult.AsHttpBuilder();
    })
    .WithName("GetUserDetails")
    .Produces<UserProfile>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound)
    .WithSummary("Get user details (requires auth)");

// ============================================================================
// Example 5: Authorization check - returns 403 for insufficient permissions
// ============================================================================
app.MapDelete("/api/users/{id:int}",
    async (int id, HttpContext context) =>
    {
        var authResult = CheckAdminAuthorization(context);
        if (authResult.IsFailure)
        {
            return await authResult.AsHttpBuilder();
        }

        var deleteResult = DeleteUser(id);
        return await deleteResult.AsHttpBuilder();
    })
    .WithName("DeleteUser")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound)
    .WithSummary("Delete a user (admin only)");

// ============================================================================
// Example 6: Custom response mapping - return created location
// ============================================================================
app.MapPost("/api/users/bulk",
    async (List<CreateUserRequest> requests) =>
    {
        var result = CreateMultipleUsers(requests);
        return await result
            .AsHttpBuilder()
            .WithStatusCode(StatusCodes.Status201Created);
    })
    .WithName("BulkCreateUsers")
    .Produces<List<UserProfile>>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithSummary("Bulk create users");

// ============================================================================
// Example 7: Async operations - simulated database query
// ============================================================================
app.MapGet("/api/users/{id:int}/send-notification",
    async (int id) =>
    {
        var result = await SendUserNotificationAsync(id);
        return await result.AsHttpBuilder();
    })
    .WithName("SendNotification")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError)
    .WithSummary("Send notification to user");

app.Run();

// ============================================================================
// Helper Methods
// ============================================================================

static Result<double> Divide(int numerator, int denominator)
{
    if (denominator == 0)
        return Result.Failure<double>(new ValidationFailure(["Cannot divide by zero"]));

    return Result.Success((double)numerator / denominator);
}

static Maybe<UserProfile> GetUserProfile(int id)
{
    var users = new Dictionary<int, UserProfile>
    {
        [1] = new(1, "Alice 🦸‍♀️ Wonder", "alice@example.com", "Engineering"),
        [2] = new(2, "Bob 🏋️ TheBuilder", "bob@example.com", "Construction"),
        [3] = new(3, "Charlie 🎸 Shreds", "charlie@example.com", "Audio Engineering"),
    };

    return users.TryGetValue(id, out var profile)
        ? Maybe.Some(profile)
        : Maybe.None<UserProfile>();
}

static Result<UserProfile> ValidateAndCreateUser(CreateUserRequest request)
{
    var errors = new List<string>();

    if (string.IsNullOrWhiteSpace(request.Name))
        errors.Add("Name is required (we need to call you something!)");

    if (string.IsNullOrWhiteSpace(request.Email))
        errors.Add("Email is required (so we can spam you with newsletters)");
    else if (!request.Email.Contains('@'))
        errors.Add("Email must have @ (it's the law)");

    if (errors.Any())
        return Result.Failure<UserProfile>(new ValidationFailure(errors));

    // Check for email conflict - the ultimate sin
    if (request.Email == "taken@example.com")
        return Result.Failure<UserProfile>(new ConflictFailure("Email already registered (sorry, first come first served!)"));

    var newUser = new UserProfile(
        Random.Shared.Next(100, 999),
        request.Name,
        request.Email,
        "Awesome Department" // Everyone starts awesome
    );

    return Result.Success(newUser);
}

static Result<object> CheckAuthentication(HttpContext context)
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (string.IsNullOrEmpty(authHeader))
        return Result.Failure<object>(new UnauthenticatedFailure("Show me your credentials, spy! 🕵️"));

    return Result.Success(new object());
}

static Result<object> CheckAdminAuthorization(HttpContext context)
{
    var authResult = CheckAuthentication(context);
    if (authResult.IsFailure)
        return authResult;

    var isAdmin = context.Request.Headers.ContainsKey("X-Admin-Role");
    if (!isAdmin)
        return Result.Failure<object>(new UnauthorizedFailure("Nice try! Admin powers only! 👑"));

    return Result.Success(new object());
}

static Result<object> DeleteUser(int id)
{
    if (id < 1)
        return Result.Failure<object>(new ValidationFailure(["Invalid user ID"]));

    var profile = GetUserProfile(id);
    if (profile.IsNone)
        return Result.Failure<object>(new NotFoundFailure("User", id.ToString()));

    return Result.Success(new object());
}

static Result<List<UserProfile>> CreateMultipleUsers(List<CreateUserRequest> requests)
{
    if (!requests.Any())
        return Result.Failure<List<UserProfile>>(new ValidationFailure(["At least one user is required"]));

    var results = new List<UserProfile>();
    var errors = new List<string>();

    foreach (var (req, idx) in requests.Select((r, i) => (r, i)))
    {
        var result = ValidateAndCreateUser(req);
        if (result.IsSuccess)
        {
            result.IfSuccess(profile => results.Add(profile));
        }
        else
        {
            result.IfFailure(f => errors.Add($"Request {idx + 1}: {f.Message}"));
        }
    }

    if (errors.Any())
        return Result.Failure<List<UserProfile>>(new ValidationFailure(errors));

    return Result.Success(results);
}

static async Task<Result<object>> SendUserNotificationAsync(int id)
{
    // Simulate async database operation (dramatic pause)
    await Task.Delay(100);

    var profile = GetUserProfile(id);
    if (profile.IsNone)
        return Result.Failure<object>(new NotFoundFailure("User", id.ToString()));

    // Simulate potential catastrophic failure
    if (id == 999)
        return Result.Failure<object>(new ExceptionalFailure(new InvalidOperationException("The notification server just yelled 'NOOOOPE!' 📢")));

    return Result.Success(new object());
}

// ============================================================================
// Domain Models
// ============================================================================

record CreateUserRequest(string Name, string Email);
record UserProfile(int Id, string Name, string Email, string Department);
