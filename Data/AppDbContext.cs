using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models.Authentication.Users;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
using RealEstate.Models.Support;
using System.Reflection.Emit;


#pragma warning disable CA1515
namespace RealEstate.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
{
    public required DbSet<Asset> Assets { get; set; }
    public required DbSet<AssetImage> AssetImages { get; set; }
    public required DbSet<Person> Persons { get; set; }
    public required DbSet<ProfileImage> UserProfileImages { get; set; }
    public required DbSet<Support> Supports { get; set; }
    public required DbSet<SupportImage> SupportImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder
            .Entity<Asset>()
            .HasMany(asset => asset.Persons)
            .WithOne(pers => pers.Asset)
            .HasForeignKey(pers => pers.AssetID)
            .HasPrincipalKey(asset => asset.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<Asset>()
            .HasMany(asset => asset.AssetImages)
            .WithOne(assetImg => assetImg.Asset)
            .HasForeignKey(assetImg => assetImg.AssetID)
            .HasPrincipalKey(asset => asset.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<User>()
            .HasOne(user => user.ProfileImage)
            .WithOne(profileImg => profileImg.User)
            .HasForeignKey<ProfileImage>(profileImg => profileImg.UserID)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProfileImage>()
        .HasIndex(p => p.UserID)
        .IsUnique();

        builder
            .Entity<Support>()
            .HasOne(support => support.SupportImage)
            .WithOne(supportImg => supportImg.Support)
            .HasForeignKey<SupportImage>(supportImg => supportImg.SupportId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<SupportImage>()
            .HasIndex(supportImg => supportImg.SupportId)
            .IsUnique();
    }
}

