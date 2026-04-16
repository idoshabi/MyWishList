using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models.ViewModels;

public class CreateWishlistViewModel
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    public string RegistryType { get; set; } = "General";
    public string Visibility { get; set; } = "Private";
    public string? Description { get; set; }
    public DateOnly? EventDate { get; set; }
    public decimal? CashFundGoal { get; set; }
}
