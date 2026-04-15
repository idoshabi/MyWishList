namespace MyWishList.Web.Services;

public class ImportService : IImportService
{
    public Task<IReadOnlyList<(string Url, string ProductName, string? Merchant)>> PreviewAsync(IEnumerable<string> urls, CancellationToken cancellationToken = default)
    {
        var output = new List<(string Url, string ProductName, string? Merchant)>();
        foreach (var raw in urls)
        {
            var url = raw?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var merchant = TryGetHost(url);
            var productName = BuildProductName(url);
            output.Add((url, productName, merchant));
        }

        return Task.FromResult<IReadOnlyList<(string Url, string ProductName, string? Merchant)>>(output);
    }

    private static string? TryGetHost(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return null;
        }

        return uri.Host.Replace("www.", "", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildProductName(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return "Imported Item";
        }

        var segment = uri.Segments.LastOrDefault()?.Trim('/').Replace("-", " ");
        if (string.IsNullOrWhiteSpace(segment))
        {
            return "Imported Item";
        }

        return segment.Length <= 180 ? segment : segment[..180];
    }
}
