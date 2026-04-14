using MyWishList.Web.Services;

namespace MyWishList.Web.BackgroundJobs;

public class WishlistMetricsBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<WishlistMetricsBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan RunInterval = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var wishlistService = scope.ServiceProvider.GetRequiredService<IWishlistService>();
                var stats = await wishlistService.GetPlatformStatsAsync(stoppingToken);

                logger.LogInformation(
                    "Wishlist metrics heartbeat - Users: {UserCount}, Wishlists: {WishlistCount}, Items: {ItemCount}",
                    stats.UserCount,
                    stats.WishlistCount,
                    stats.ItemCount);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to collect wishlist metrics.");
            }

            await Task.Delay(RunInterval, stoppingToken);
        }
    }
}
