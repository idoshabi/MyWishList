using MyWishList.Web.Contracts.Api;

namespace MyWishList.Web.Services;

public interface IAdminService
{
    Task<AdminMetricsResponse> GetMetricsAsync(CancellationToken cancellationToken = default);
}
