using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models;

public class Wishlist
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();
}
