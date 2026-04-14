using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public interface IWishlistService
{
    Task<IReadOnlyList<WishlistSummaryDto>> GetDashboardWishlistsAsync(int userId, CancellationToken cancellationToken = default);
    Task<CreateWishlistResult> CreateAsync(int userId, string wishlistName, CancellationToken cancellationToken = default);
    Task<WishlistDetailsDto?> GetDetailsAsync(int userId, int wishlistId, CancellationToken cancellationToken = default);
    Task<bool> UserOwnsWishlistAsync(int userId, int wishlistId, CancellationToken cancellationToken = default);
    Task<WishlistPlatformStatsDto> GetPlatformStatsAsync(CancellationToken cancellationToken = default);
}
