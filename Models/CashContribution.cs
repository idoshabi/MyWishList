using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models;

public class CashContribution
{
    public int Id { get; set; }

    public int WishlistId { get; set; }
    public Wishlist? Wishlist { get; set; }

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

    [Required, StringLength(20)]
    public string Status { get; set; } = "Completed";

    [StringLength(200)]
    public string? ExternalReference { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
