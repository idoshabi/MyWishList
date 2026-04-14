using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MyWishList.Web.Contracts.Api;

namespace MyWishList.Web.Tests;

public class WishlistsApiTests(TestWebApplicationFactory factory) : IClassFixture<TestWebApplicationFactory>
{
    [Fact]
    public async Task GetWishlists_WithoutAuth_ReturnsUnauthorized()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/wishlists");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RegisterThenCreateWishlist_ReturnsCreated()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", new RegisterApiRequest
        {
            Username = "api-user",
            Email = "api-user@example.com",
            FirstName = "Api",
            LastName = "User",
            Password = "P@ssw0rd",
            DateOfBirth = new DateOnly(2001, 1, 1)
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var createWishlistResponse = await client.PostAsJsonAsync("/api/wishlists", new CreateWishlistApiRequest
        {
            Name = "API Wishlist"
        });
        Assert.Equal(HttpStatusCode.Created, createWishlistResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/wishlists");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var data = await getResponse.Content.ReadFromJsonAsync<List<WishlistSummaryResponse>>();
        Assert.NotNull(data);
        Assert.Single(data!);
        Assert.Equal("API Wishlist", data[0].Name);
    }
}
