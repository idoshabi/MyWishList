using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models;

public class User
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }

    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
