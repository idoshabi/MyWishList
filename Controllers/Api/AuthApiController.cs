using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWishList.Web.Contracts.Api;
using MyWishList.Web.Models;
using MyWishList.Web.Services;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Controllers.Api;

[Route("api/auth")]
public class AuthApiController(IAuthService authService) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<AuthUserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterApiRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(new RegisterUserCommand
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
            DateOfBirth = request.DateOfBirth
        }, cancellationToken);

        if (!result.Succeeded || result.User is null)
        {
            return BadRequest(new ApiErrorResponse { Message = result.ErrorMessage ?? "Registration failed." });
        }

        await SignInUser(result.User);
        return CreatedAtAction(nameof(Me), new { }, ToAuthUserResponse(result.User));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginApiRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ValidateCredentialsAsync(request.UsernameOrEmail, request.Password, cancellationToken);
        if (!result.Succeeded || result.User is null)
        {
            return Unauthorized(new ApiErrorResponse { Message = result.ErrorMessage ?? "Invalid credentials." });
        }

        await SignInUser(result.User);
        return Ok(ToAuthUserResponse(result.User));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<AuthUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var user = await authService.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(ToAuthUserResponse(user));
    }

    private async Task SignInUser(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("FirstName", user.FirstName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }

    private static AuthUserResponse ToAuthUserResponse(User user)
    {
        return new AuthUserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}
