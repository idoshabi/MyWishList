namespace MyWishList.Web.Services;

public interface INotificationService
{
    Task SendThankYouAsync(string recipientName, string recipientEmail, string message, CancellationToken cancellationToken = default);
}
