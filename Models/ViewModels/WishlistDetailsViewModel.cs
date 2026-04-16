namespace MyWishList.Web.Models.ViewModels;

public class WishlistDetailsViewModel
{
    public int WishlistId { get; set; }
    public string WishlistName { get; set; } = string.Empty;
    public string RegistryType { get; set; } = "General";
    public string Visibility { get; set; } = "Private";
    public string ShareToken { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? EventDate { get; set; }
    public decimal? CashFundGoal { get; set; }
    public decimal CashFundRaised { get; set; }
    public bool IsOwner { get; set; }
    public List<ItemSummary> Items { get; set; } = [];
}

public class ItemSummary
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Merchant { get; set; }
    public string? Type { get; set; }
    public bool IsReserved { get; set; }
    public string? ReservedByName { get; set; }
    public bool IsPurchased { get; set; }
    public string? PurchasedByName { get; set; }
    public bool IsReceived { get; set; }
}
