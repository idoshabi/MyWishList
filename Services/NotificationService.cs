using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyWishList.Web.Services;

public class NotificationService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<NotificationService> logger) : INotificationService
{
    public async Task SendThankYouAsync(string recipientName, string recipientEmail, string message, CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["SendGrid:ApiKey"];
        var fromEmail = configuration["SendGrid:FromEmail"] ?? "no-reply@mywishlist.local";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogInformation("Mock thank-you email -> {Recipient} <{Email}>: {Message}", recipientName, recipientEmail, message);
            return;
        }

        var payload = new
        {
            personalizations = new[]
            {
                new
                {
                    to = new[] { new { email = recipientEmail, name = recipientName } }
                }
            },
            from = new { email = fromEmail, name = "MyWishList" },
            subject = "Thank you from MyWishList",
            content = new[]
            {
                new { type = "text/plain", value = message }
            }
        };

        var http = httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sendgrid.com/v3/mail/send");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("SendGrid request failed with status code {StatusCode}.", (int)response.StatusCode);
        }
    }
}
