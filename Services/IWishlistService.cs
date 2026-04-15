using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public interface IWishlistService
{
    Task<IReadOnlyList<WishlistSummaryDto>> GetDashboardWishlistsAsync(int userId, CancellationToken cancellationToken = default);
    Task<CreateWishlistResult> CreateAsync(int userId, CreateWishlistCommand command, CancellationToken cancellationToken = default);
    Task<WishlistSettingsResult> UpdateSettingsAsync(int userId, int wishlistId, UpdateWishlistSettingsCommand command, CancellationToken cancellationToken = default);
    Task<WishlistDetailsDto?> GetDetailsAsync(int userId, int wishlistId, CancellationToken cancellationToken = default);
    Task<WishlistDetailsDto?> GetPublicDetailsByShareTokenAsync(string shareToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WishlistSummaryDto>> SearchPublicAsync(string? query, string? registryType, CancellationToken cancellationToken = default);
    Task<CashContributionResult> ContributeAsync(int wishlistId, ContributeCashCommand command, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CashContributionDto>> GetContributionsAsync(int userId, int wishlistId, CancellationToken cancellationToken = default);
    Task<bool> UserOwnsWishlistAsync(int userId, int wishlistId, CancellationToken cancellationToken = default);
    Task<WishlistPlatformStatsDto> GetPlatformStatsAsync(CancellationToken cancellationToken = default);
}
