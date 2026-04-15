namespace MyWishList.Web.Services;

public interface IImportService
{
    Task<IReadOnlyList<(string Url, string ProductName, string? Merchant)>> PreviewAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default);
}
