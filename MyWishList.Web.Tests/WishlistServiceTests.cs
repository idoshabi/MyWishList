using Microsoft.Extensions.DependencyInjection;
using MyWishList.Web.Services;

namespace MyWishList.Web.Tests;

public class WishlistServiceTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task CreateAndGetDetails_ReturnsCreatedWishlist()
    {
        await factory.ResetDatabaseAsync();
        var user = await factory.CreateUserAsync("service-user");

        using var scope = factory.Services.CreateScope();
        var wishlistService = scope.ServiceProvider.GetRequiredService<IWishlistService>();

        var createResult = await wishlistService.CreateAsync(user.Id, "Birthday");
        Assert.True(createResult.Succeeded);
        Assert.True(createResult.WishlistId > 0);

        var details = await wishlistService.GetDetailsAsync(user.Id, createResult.WishlistId);
        Assert.NotNull(details);
        Assert.Equal("Birthday", details!.WishlistName);
    }
}
