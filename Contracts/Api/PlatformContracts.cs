using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Contracts.Api;

public sealed class ImportPreviewRequest
{
    [Required]
    public List<string> Urls { get; set; } = [];
}

public sealed class ImportedItemPreviewResponse
{
    public string Url { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Merchant { get; set; }
}

public sealed class ThankYouRequest
{
    [Required, StringLength(120)]
    public string RecipientName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string RecipientEmail { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Message { get; set; }
}

public sealed class AdminMetricsResponse
{
    public int Users { get; set; }
    public int Wishlists { get; set; }
    public int Items { get; set; }
    public int Contributions { get; set; }
}
