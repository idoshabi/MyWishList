using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public class AuthService(AppDbContext dbContext) : IAuthService
{
    public async Task<RegisterUserResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = command.Username.Trim();
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var usernameExists = await dbContext.Users.AnyAsync(u => u.Username == normalizedUsername, cancellationToken);
        var emailExists = await dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

        if (usernameExists || emailExists)
        {
            return new RegisterUserResult
            {
                Succeeded = false,
                ErrorMessage = "Username or email already exists."
            };
        }

        var user = new User
        {
            Username = normalizedUsername,
            Email = normalizedEmail,
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            DateOfBirth = command.DateOfBirth,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password)
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new RegisterUserResult
        {
            Succeeded = true,
            User = user
        };
    }

    public async Task<LoginResult> ValidateCredentialsAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        var userInput = usernameOrEmail.Trim();
        var normalizedEmail = userInput.ToLowerInvariant();

        var user = await dbContext.Users.FirstOrDefaultAsync(u =>
            u.Username == userInput || u.Email == normalizedEmail,
            cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return new LoginResult
            {
                Succeeded = false,
                ErrorMessage = "Invalid credentials."
            };
        }

        return new LoginResult
        {
            Succeeded = true,
            User = user
        };
    }

    public Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}
