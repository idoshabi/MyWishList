namespace MyWishList.Web.Services.Models;

public sealed class WishlistSummaryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string RegistryType { get; init; } = "General";
    public string Visibility { get; init; } = "Private";
    public decimal? CashFundGoal { get; init; }
    public decimal CashFundRaised { get; init; }
    public string? Description { get; init; }
    public DateOnly? EventDate { get; init; }
    public string ShareToken { get; init; } = string.Empty;
    public int ItemCount { get; init; }
}

public sealed class ItemDto
{
    public int Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? Link { get; init; }
    public string? Merchant { get; init; }
    public string? Type { get; init; }
    public bool IsReserved { get; init; }
    public string? ReservedByName { get; init; }
    public DateTimeOffset? ReservedAtUtc { get; init; }
    public bool IsPurchased { get; init; }
    public string? PurchasedByName { get; init; }
    public DateTimeOffset? PurchasedAtUtc { get; init; }
    public bool IsReceived { get; init; }
    public DateTimeOffset? ReceivedAtUtc { get; init; }
}

public sealed class WishlistDetailsDto
{
    public int WishlistId { get; init; }
    public string WishlistName { get; init; } = string.Empty;
    public string RegistryType { get; init; } = "General";
    public string Visibility { get; init; } = "Private";
    public decimal? CashFundGoal { get; init; }
    public decimal CashFundRaised { get; init; }
    public string? Description { get; init; }
    public DateOnly? EventDate { get; init; }
    public string ShareToken { get; init; } = string.Empty;
    public IReadOnlyList<ItemDto> Items { get; init; } = [];
}

public sealed class CashContributionDto
{
    public int Id { get; init; }
    public string Provider { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string ContributorName { get; init; } = string.Empty;
    public string? ContributorEmail { get; init; }
    public string? Message { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
}

public sealed class WishlistPlatformStatsDto
{
    public int UserCount { get; init; }
    public int WishlistCount { get; init; }
    public int ItemCount { get; init; }
}
