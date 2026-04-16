namespace MyWishList.Web.Models.ViewModels;

public class DashboardViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public int TotalLists { get; set; }
    public int TotalItems { get; set; }
    public int GiftsPurchased { get; set; }
    public int UpcomingEvents { get; set; }
    public List<WishlistSummary> Wishlists { get; set; } = [];
}

public class WishlistSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistryType { get; set; } = "General";
    public string Visibility { get; set; } = "Private";
    public string ShareToken { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int ReservedCount { get; set; }
    public int PurchasedCount { get; set; }
    public DateOnly? EventDate { get; set; }
    public string? Description { get; set; }
    public decimal? CashFundGoal { get; set; }
    public decimal CashFundRaised { get; set; }
}
