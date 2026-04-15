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

public sealed class CreateWishlistCommand
{
    public string Name { get; init; } = string.Empty;
    public string RegistryType { get; init; } = "General";
    public string Visibility { get; init; } = "Private";
    public decimal? CashFundGoal { get; init; }
    public string? Description { get; init; }
    public DateOnly? EventDate { get; init; }
}

public sealed class UpdateWishlistSettingsCommand
{
    public string Name { get; init; } = string.Empty;
    public string RegistryType { get; init; } = "General";
    public string Visibility { get; init; } = "Private";
    public decimal? CashFundGoal { get; init; }
    public string? Description { get; init; }
    public DateOnly? EventDate { get; init; }
}

public sealed class ContributeCashCommand
{
    public string Provider { get; init; } = "Stripe";
    public decimal Amount { get; init; }
    public string ContributorName { get; init; } = string.Empty;
    public string? ContributorEmail { get; init; }
    public string? Message { get; init; }
}
