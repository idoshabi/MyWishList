using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models.ViewModels;

public class CreateItemViewModel
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
