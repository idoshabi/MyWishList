using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models;

public class Wishlist
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string RegistryType { get; set; } = "General";

    [Required, StringLength(20)]
    public string Visibility { get; set; } = "Private";

    [Required, StringLength(64)]
    public string ShareToken { get; set; } = Guid.NewGuid().ToString("N");

    [Range(0, 1_000_000_000)]
    public decimal? CashFundGoal { get; set; }

    [Range(0, 1_000_000_000)]
    public decimal CashFundRaised { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateOnly? EventDate { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<CashContribution> CashContributions { get; set; } = new List<CashContribution>();
}
