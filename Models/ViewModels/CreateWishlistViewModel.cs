using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models.ViewModels;

public class CreateWishlistViewModel
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;
}