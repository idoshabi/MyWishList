using Microsoft.EntityFrameworkCore;
using MyWishList.Web.Models;

namespace MyWishList.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<CashContribution> CashContributions => Set<CashContribution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Wishlist>()
            .HasIndex(w => w.ShareToken)
            .IsUnique();

        modelBuilder.Entity<Wishlist>()
            .Property(w => w.CashFundGoal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Wishlist>()
            .Property(w => w.CashFundRaised)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Wishlist>()
            .HasOne(w => w.User)
            .WithMany(u => u.Wishlists)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CashContribution>()
            .Property(c => c.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CashContribution>()
            .HasOne(c => c.Wishlist)
            .WithMany(w => w.CashContributions)
            .HasForeignKey(c => c.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.Wishlist)
            .WithMany(w => w.Items)
            .HasForeignKey(i => i.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
