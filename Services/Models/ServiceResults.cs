using MyWishList.Web.Models;

namespace MyWishList.Web.Services.Models;

public sealed class RegisterUserResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public User? User { get; init; }
}

public sealed class LoginResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public User? User { get; init; }
}

public sealed class CreateWishlistResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public int WishlistId { get; init; }
}

public sealed class AddItemResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public int ItemId { get; init; }
}

public sealed class QueueAddItemResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class ReserveItemResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class WishlistSettingsResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class CashContributionResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
    public int ContributionId { get; init; }
}

public sealed class ItemStatusResult
{
    public bool Succeeded { get; init; }
    public string? ErrorMessage { get; init; }
}
