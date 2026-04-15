using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Contracts.Api;
using MyWishList.Web.Services;

namespace MyWishList.Web.Controllers.Api;

[ApiController]
[Route("api/platform")]
public class PlatformApiController(IImportService importService, INotificationService notificationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("import-preview")]
    [ProducesResponseType<IReadOnlyList<ImportedItemPreviewResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> PreviewImport([FromBody] ImportPreviewRequest request, CancellationToken cancellationToken)
    {
        var previews = await importService.PreviewAsync(request.Urls, cancellationToken);
        return Ok(previews.Select(p => new ImportedItemPreviewResponse
        {
            Url = p.Url,
            ProductName = p.ProductName,
            Merchant = p.Merchant
        }));
    }

    [AllowAnonymous]
    [HttpPost("thank-you")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SendThankYou([FromBody] ThankYouRequest request, CancellationToken cancellationToken)
    {
        var message = string.IsNullOrWhiteSpace(request.Message)
            ? $"Thank you, {request.RecipientName}! We really appreciate your gift."
            : request.Message.Trim();

        await notificationService.SendThankYouAsync(
            request.RecipientName.Trim(),
            request.RecipientEmail.Trim(),
            message,
            cancellationToken);

        return NoContent();
    }
}
