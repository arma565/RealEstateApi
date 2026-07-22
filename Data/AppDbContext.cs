using Microsoft.EntityFrameworkCore;
using RealEstate.Models.Assets;
using RealEstate.Models.Persons;
using RealEstate.Models.Support;

#pragma warning disable CA1515
namespace RealEstate.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<Asset> Assets { get; set; }
    public required DbSet<AssetImage> AssetImages { get; set; }
    public required DbSet<Person> Persons { get; set; }
    public required DbSet<SupportApp> Supports { get; set; }
    public required DbSet<SupportImage> SupportImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Asset>()
            .HasMany(asset => asset.Persons)
            .WithOne(pers => pers.Asset)
            .HasForeignKey(pers => pers.AssetID)
            .HasPrincipalKey(asset => asset.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Asset>()
            .HasMany(asset => asset.AssetImages)
            .WithOne(assetImg => assetImg.Asset)
            .HasForeignKey(assetImg => assetImg.AssetID)
            .HasPrincipalKey(asset => asset.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<SupportApp>()
            .HasOne(support => support.SupportImage)
            .WithOne(supportImg => supportImg.Support)
            .HasForeignKey<SupportImage>(supportImg => supportImg.SupportId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<SupportImage>()
            .HasIndex(supportImg => supportImg.SupportId)
            .IsUnique();
    }
}

