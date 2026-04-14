namespace MyWishList.Web.Services.Models;

public sealed class RegisterUserCommand
{
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public DateOnly DateOfBirth { get; init; }
}

public sealed class CreateItemCommand
{
    public string ProductName { get; init; } = string.Empty;
    public string? Link { get; init; }
    public string? Merchant { get; init; }
    public string? Type { get; init; }
}
