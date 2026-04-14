using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Models.ViewModels;

public class RegisterViewModel
{
    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateOnly DateOfBirth { get; set; }
}
