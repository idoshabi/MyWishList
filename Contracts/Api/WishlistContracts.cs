using System.ComponentModel.DataAnnotations;

namespace MyWishList.Web.Contracts.Api;

public sealed class CreateWishlistApiRequest
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;
}

public sealed class WishlistSummaryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public sealed class WishlistDetailsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<ItemResponse> Items { get; set; } = [];
}

public sealed class CreateItemApiRequest
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

public sealed class ItemResponse
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Merchant { get; set; }
    public string? Type { get; set; }
}
