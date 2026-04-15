using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models;

public class Item
{
    public int Id { get; set; }

    [Required, StringLength(180)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Link { get; set; }

    [StringLength(120)]
    public string? Merchant { get; set; }

    [StringLength(80)]
    public string? Type { get; set; }

    public bool IsReserved { get; set; }

    [StringLength(120)]
    public string? ReservedByName { get; set; }

    public DateTimeOffset? ReservedAtUtc { get; set; }

    public bool IsPurchased { get; set; }

    [StringLength(120)]
    public string? PurchasedByName { get; set; }

    public DateTimeOffset? PurchasedAtUtc { get; set; }

    public bool IsReceived { get; set; }

    public DateTimeOffset? ReceivedAtUtc { get; set; }

    public int WishlistId { get; set; }
    public Wishlist? Wishlist { get; set; }
}
