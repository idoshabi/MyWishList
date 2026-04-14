namespace MyWishList.Web.Models.ViewModels;

public class DashboardViewModel
{
    public List<WishlistSummary> Wishlists { get; set; } = [];
}

public class WishlistSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}
