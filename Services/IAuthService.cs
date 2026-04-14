using MyWishList.Web.Models;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public interface IAuthService
{
    Task<RegisterUserResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken = default);
    Task<LoginResult> ValidateCredentialsAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
}
