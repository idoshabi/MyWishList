using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Data;
using MyWishList.Web.Models;
using MyWishList.Web.Services.Models;

namespace MyWishList.Web.Services;

public class WishlistService(AppDbContext dbContext) : IWishlistService
{
    public async Task<IReadOnlyList<WishlistSummaryDto>> GetDashboardWishlistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistSummaryDto
            {
                Id = w.Id,
                Name = w.Name,
                RegistryType = w.RegistryType,
                Visibility = w.Visibility,
                CashFundGoal = w.CashFundGoal,
                CashFundRaised = w.CashFundRaised,
                Description = w.Description,
                EventDate = w.EventDate,
                ShareToken = w.ShareToken,
                ItemCount = w.Items.Count
            })
            .OrderByDescending(w => w.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateWishlistResult> CreateAsync(int userId, CreateWishlistCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedName = command.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            return new CreateWishlistResult
            {
                Succeeded = false,
                ErrorMessage = "Wishlist name is required."
            };
        }

        var wishlist = new Wishlist
        {
            Name = normalizedName,
            RegistryType = NormalizeRegistryType(command.RegistryType),
            Visibility = NormalizeVisibility(command.Visibility),
            ShareToken = Guid.NewGuid().ToString("N"),
            CashFundGoal = command.CashFundGoal,
            CashFundRaised = 0m,
            Description = NormalizeDescription(command.Description),
            EventDate = command.EventDate,
            UserId = userId
        };

        dbContext.Wishlists.Add(wishlist);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateWishlistResult
        {
            Succeeded = true,
            WishlistId = wishlist.Id
        };
    }

    public async Task<WishlistSettingsResult> UpdateSettingsAsync(int userId, int wishlistId, UpdateWishlistSettingsCommand command, CancellationToken cancellationToken = default)
    {
        var wishlist = await dbContext.Wishlists
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);

        if (wishlist is null)
        {
            return new WishlistSettingsResult
            {
                Succeeded = false,
                ErrorMessage = "Wishlist not found."
            };
        }

        var name = command.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return new WishlistSettingsResult
            {
                Succeeded = false,
                ErrorMessage = "Wishlist name is required."
            };
        }

        wishlist.Name = name;
        wishlist.RegistryType = NormalizeRegistryType(command.RegistryType);
        wishlist.Visibility = NormalizeVisibility(command.Visibility);
        wishlist.CashFundGoal = command.CashFundGoal;
        wishlist.Description = NormalizeDescription(command.Description);
        wishlist.EventDate = command.EventDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new WishlistSettingsResult { Succeeded = true };
    }

    public async Task<WishlistDetailsDto?> GetDetailsAsync(int userId, int wishlistId, CancellationToken cancellationToken = default)
    {
        var wishlist = await dbContext.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);

        if (wishlist is null)
        {
            return null;
        }

        return new WishlistDetailsDto
        {
            WishlistId = wishlist.Id,
            WishlistName = wishlist.Name,
            RegistryType = wishlist.RegistryType,
            Visibility = wishlist.Visibility,
            CashFundGoal = wishlist.CashFundGoal,
            CashFundRaised = wishlist.CashFundRaised,
            Description = wishlist.Description,
            EventDate = wishlist.EventDate,
            ShareToken = wishlist.ShareToken,
            Items = wishlist.Items
                .OrderByDescending(i => i.Id)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Link = i.Link,
                    Merchant = i.Merchant,
                    Type = i.Type,
                    IsReserved = i.IsReserved,
                    ReservedByName = i.ReservedByName,
                    ReservedAtUtc = i.ReservedAtUtc,
                    IsPurchased = i.IsPurchased,
                    PurchasedByName = i.PurchasedByName,
                    PurchasedAtUtc = i.PurchasedAtUtc,
                    IsReceived = i.IsReceived,
                    ReceivedAtUtc = i.ReceivedAtUtc
                }).ToList()
        };
    }

    public async Task<WishlistDetailsDto?> GetPublicDetailsByShareTokenAsync(string shareToken, CancellationToken cancellationToken = default)
    {
        var token = shareToken.Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var wishlist = await dbContext.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.ShareToken == token && w.Visibility == "Public", cancellationToken);

        if (wishlist is null)
        {
            return null;
        }

        return new WishlistDetailsDto
        {
            WishlistId = wishlist.Id,
            WishlistName = wishlist.Name,
            RegistryType = wishlist.RegistryType,
            Visibility = wishlist.Visibility,
            CashFundGoal = wishlist.CashFundGoal,
            CashFundRaised = wishlist.CashFundRaised,
            Description = wishlist.Description,
            EventDate = wishlist.EventDate,
            ShareToken = wishlist.ShareToken,
            Items = wishlist.Items
                .OrderByDescending(i => i.Id)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    ProductName = i.ProductName,
                    Link = i.Link,
                    Merchant = i.Merchant,
                    Type = i.Type,
                    IsReserved = i.IsReserved,
                    ReservedByName = i.ReservedByName,
                    ReservedAtUtc = i.ReservedAtUtc,
                    IsPurchased = i.IsPurchased,
                    PurchasedByName = i.PurchasedByName,
                    PurchasedAtUtc = i.PurchasedAtUtc,
                    IsReceived = i.IsReceived,
                    ReceivedAtUtc = i.ReceivedAtUtc
                }).ToList()
        };
    }

    public async Task<IReadOnlyList<WishlistSummaryDto>> SearchPublicAsync(string? query, string? registryType, CancellationToken cancellationToken = default)
    {
        var searchable = dbContext.Wishlists
            .Where(w => w.Visibility == "Public");

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = query.Trim();
            searchable = searchable.Where(w =>
                w.Name.Contains(normalized) ||
                (w.Description != null && w.Description.Contains(normalized)));
        }

        var normalizedType = NormalizeRegistryType(registryType ?? "General");
        if (!string.IsNullOrWhiteSpace(registryType))
        {
            searchable = searchable.Where(w => w.RegistryType == normalizedType);
        }

        return await searchable
            .OrderByDescending(w => w.Id)
            .Select(w => new WishlistSummaryDto
            {
                Id = w.Id,
                Name = w.Name,
                RegistryType = w.RegistryType,
                Visibility = w.Visibility,
                CashFundGoal = w.CashFundGoal,
                CashFundRaised = w.CashFundRaised,
                Description = w.Description,
                EventDate = w.EventDate,
                ShareToken = w.ShareToken,
                ItemCount = w.Items.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CashContributionResult> ContributeAsync(int wishlistId, ContributeCashCommand command, CancellationToken cancellationToken = default)
    {
        var wishlist = await dbContext.Wishlists
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.Visibility == "Public", cancellationToken);

        if (wishlist is null)
        {
            return new CashContributionResult
            {
                Succeeded = false,
                ErrorMessage = "Wishlist not found."
            };
        }

        if (command.Amount <= 0m)
        {
            return new CashContributionResult
            {
                Succeeded = false,
                ErrorMessage = "Contribution amount must be greater than zero."
            };
        }

        var contributor = command.ContributorName.Trim();
        if (string.IsNullOrWhiteSpace(contributor))
        {
            return new CashContributionResult
            {
                Succeeded = false,
                ErrorMessage = "Contributor name is required."
            };
        }

        var contribution = new CashContribution
        {
            WishlistId = wishlistId,
            Provider = NormalizeProvider(command.Provider),
            Amount = command.Amount,
            ContributorName = contributor,
            ContributorEmail = NormalizeNullable(command.ContributorEmail),
            Message = NormalizeNullable(command.Message),
            Status = "Completed",
            ExternalReference = Guid.NewGuid().ToString("N"),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        dbContext.CashContributions.Add(contribution);
        wishlist.CashFundRaised += contribution.Amount;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CashContributionResult
        {
            Succeeded = true,
            ContributionId = contribution.Id
        };
    }

    public async Task<IReadOnlyList<CashContributionDto>> GetContributionsAsync(int userId, int wishlistId, CancellationToken cancellationToken = default)
    {
        var owns = await dbContext.Wishlists.AnyAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);
        if (!owns)
        {
            return [];
        }

        return await dbContext.CashContributions
            .Where(c => c.WishlistId == wishlistId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Select(c => new CashContributionDto
            {
                Id = c.Id,
                Provider = c.Provider,
                Amount = c.Amount,
                ContributorName = c.ContributorName,
                ContributorEmail = c.ContributorEmail,
                Message = c.Message,
                Status = c.Status,
                CreatedAtUtc = c.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public Task<bool> UserOwnsWishlistAsync(int userId, int wishlistId, CancellationToken cancellationToken = default)
    {
        return dbContext.Wishlists.AnyAsync(w => w.Id == wishlistId && w.UserId == userId, cancellationToken);
    }

    public async Task<WishlistPlatformStatsDto> GetPlatformStatsAsync(CancellationToken cancellationToken = default)
    {
        var userCount = await dbContext.Users.CountAsync(cancellationToken);
        var wishlistCount = await dbContext.Wishlists.CountAsync(cancellationToken);
        var itemCount = await dbContext.Items.CountAsync(cancellationToken);

        return new WishlistPlatformStatsDto
        {
            UserCount = userCount,
            WishlistCount = wishlistCount,
            ItemCount = itemCount
        };
    }

    private static string NormalizeRegistryType(string value)
    {
        return value.Trim() switch
        {
            "Wedding" => "Wedding",
            "Baby" => "Baby",
            "Birthday" => "Birthday",
            "Housewarming" => "Housewarming",
            "Nonprofit" => "Nonprofit",
            _ => "General"
        };
    }

    private static string NormalizeVisibility(string value)
    {
        return value.Trim() switch
        {
            "Public" => "Public",
            _ => "Private"
        };
    }

    private static string NormalizeProvider(string value)
    {
        return value.Trim() switch
        {
            "PayPal" => "PayPal",
            _ => "Stripe"
        };
    }

    private static string? NormalizeNullable(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeDescription(string? value)
    {
        var normalized = NormalizeNullable(value);
        if (normalized is null)
        {
            return null;
        }

        return normalized.Length <= 1000 ? normalized : normalized[..1000];
    }
}
