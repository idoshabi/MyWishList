namespace MyWishList.Web.Models.ViewModels;

public class WishlistDetailsViewModel
{
    public int WishlistId { get; set; }
    public string WishlistName { get; set; } = string.Empty;
    public List<ItemSummary> Items { get; set; } = [];
}

public class ItemSummary
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Merchant { get; set; }
    public string? Type { get; set; }
}
