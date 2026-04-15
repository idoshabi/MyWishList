using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Contracts.Api;

public sealed class CreateWishlistApiRequest
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string RegistryType { get; set; } = "General";

    [Required, StringLength(20)]
    public string Visibility { get; set; } = "Private";

    [Range(0, 1_000_000_000)]
    public decimal? CashFundGoal { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateOnly? EventDate { get; set; }
}

public sealed class UpdateWishlistSettingsApiRequest
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string RegistryType { get; set; } = "General";

    [Required, StringLength(20)]
    public string Visibility { get; set; } = "Private";

    [Range(0, 1_000_000_000)]
    public decimal? CashFundGoal { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateOnly? EventDate { get; set; }
}

public sealed class WishlistSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistryType { get; set; } = "General";
    public string Visibility { get; set; } = "Private";
    public decimal? CashFundGoal { get; set; }
    public decimal CashFundRaised { get; set; }
    public string? Description { get; set; }
    public DateOnly? EventDate { get; set; }
    public string ShareToken { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public sealed class WishlistDetailsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistryType { get; set; } = "General";
    public string Visibility { get; set; } = "Private";
    public decimal? CashFundGoal { get; set; }
    public decimal CashFundRaised { get; set; }
    public string? Description { get; set; }
    public DateOnly? EventDate { get; set; }
    public string ShareToken { get; set; } = string.Empty;
    public IReadOnlyList<ItemResponse> Items { get; set; } = [];
}

public sealed class CreateItemApiRequest
{
    [Required, StringLength(180)]
    public string ProductName { get; set; } = string.Empty;

    [Url, StringLength(1000)]
    public string? Link { get; set; }

    [StringLength(120)]
    public string? Merchant { get; set; }

    [StringLength(80)]
    public string? Type { get; set; }
}

public sealed class QueueItemResponse
{
    public string Status { get; set; } = "Queued";
    public string Message { get; set; } = "Item was queued for background processing.";
}

public sealed class ItemResponse
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Merchant { get; set; }
    public string? Type { get; set; }
    public bool IsReserved { get; set; }
    public string? ReservedByName { get; set; }
    public DateTimeOffset? ReservedAtUtc { get; set; }
    public bool IsPurchased { get; set; }
    public string? PurchasedByName { get; set; }
    public DateTimeOffset? PurchasedAtUtc { get; set; }
    public bool IsReceived { get; set; }
    public DateTimeOffset? ReceivedAtUtc { get; set; }
}

public sealed class ReserveItemRequest
{
    [Required, StringLength(120)]
    public string ReservedByName { get; set; } = string.Empty;
}

public sealed class PurchaseItemRequest
{
    [Required, StringLength(120)]
    public string PurchasedByName { get; set; } = string.Empty;
}

public sealed class CashContributionRequest
{
    [Required, StringLength(20)]
    public string Provider { get; set; } = "Stripe";

    [Range(1, 1_000_000_000)]
    public decimal Amount { get; set; }

    [Required, StringLength(120)]
    public string ContributorName { get; set; } = string.Empty;

    [EmailAddress, StringLength(200)]
    public string? ContributorEmail { get; set; }

    [StringLength(120)]
    public string? Message { get; set; }
}

public sealed class CashContributionResponse
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ContributorName { get; set; } = string.Empty;
    public string? ContributorEmail { get; set; }
    public string? Message { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
}
