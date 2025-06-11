using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealEstate.Models.Estate;
using RealEstate.Models.Estate.Assets;
using System.Text.Json;


namespace RealEstate.Data
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserProfileIdentity>(options)
    {
        public required DbSet<Asset> Assets { get; set; }

        public required DbSet<AssetImage> AssetImages { get; set; }

        public required DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);
 
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
                .OnDelete(DeleteBehavior.Cascade); ;

            builder.Entity<IdentityUserLogin<string>>().HasNoKey();

            builder.Entity<IdentityUserRole<string>>().HasNoKey();

            builder.Entity<IdentityUserToken<string>>().HasNoKey();
        }
    }
}

