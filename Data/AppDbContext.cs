using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models.Estate;

namespace RealEstate.Data
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserProfileIdentity>(options)
    {
        public required DbSet<Asset> Estates { get; set; }

        public required DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Entity<UserProfileIdentity>()
            .Property(e => e.ProfileImageUrl)
            .HasConversion(
             v => v!.ToString(),// string to Uri
             v => new Uri(v));  // Uri to string        


            builder
                .Entity<Asset>()
                .HasMany(prop => prop.Persons)
                .WithOne(pers => pers.Asset)
                .HasForeignKey(pers => pers.PropertyID)
                .HasPrincipalKey(prop => prop.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IdentityUserLogin<string>>().HasNoKey();

            builder.Entity<IdentityUserRole<string>>().HasNoKey();

            builder.Entity<IdentityUserToken<string>>().HasNoKey();
        }
    }
}

