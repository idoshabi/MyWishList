using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Contracts.Api;
using MyWishList.Web.Services;

namespace MyWishList.Web.Controllers.Api;

[ApiController]
[Route("api/admin")]
public class AdminApiController(IAdminService adminService, IConfiguration configuration) : ControllerBase
{
    [HttpGet("metrics")]
    [ProducesResponseType<AdminMetricsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Metrics([FromHeader(Name = "X-Admin-Key")] string? adminKey, CancellationToken cancellationToken)
    {
        var expected = configuration["Admin:ApiKey"];
        if (string.IsNullOrWhiteSpace(expected) || !string.Equals(adminKey, expected, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        return Ok(await adminService.GetMetricsAsync(cancellationToken));
    }
}
