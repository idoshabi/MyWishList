using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Contracts.Api;
using MyWishList.Web.Data;

namespace MyWishList.Web.Services;

public class AdminService(AppDbContext dbContext) : IAdminService
{
    public async Task<AdminMetricsResponse> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        return new AdminMetricsResponse
        {
            Users = await dbContext.Users.CountAsync(cancellationToken),
            Wishlists = await dbContext.Wishlists.CountAsync(cancellationToken),
            Items = await dbContext.Items.CountAsync(cancellationToken),
            Contributions = await dbContext.CashContributions.CountAsync(cancellationToken)
        };
    }
}
