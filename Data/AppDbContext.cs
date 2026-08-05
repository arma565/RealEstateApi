using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Services.Models.Images;
using RealEstate.Services.Models.Images.Properties;
using RealEstate.Services.Models.Persons;
using RealEstate.Services.Models.Properties;
using RealEstate.Services.Models.Properties.Addresses;
using RealEstate.Services.Models.Properties.Addresses.Map;
using RealEstate.Services.Models.Properties.Documents;
using RealEstate.Services.Models.Properties.Features;
using RealEstate.Services.Models.Properties.Leases;
using RealEstate.Services.Models.Properties.Payments;
using RealEstate.Services.Models.Supports;
using RealEstate.Services.Models.Users;

#pragma warning disable CA1515
namespace RealEstate.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public required DbSet<RealEstateImage> Images { get; set; }

    public required DbSet<PropertyImage> PropertyImages { get; set; }

    public required DbSet<Person> Persons { get; set; }

    public required DbSet<PropertyAddress> Addresses { get; set; }

    public required DbSet<PropertyLocation> Locations { get; set; }

    public required DbSet<PropertyDeed> PropertyDeeds { get; set; }

    public required DbSet<Lease> Leases { get; set; }

    public required DbSet<Payment> Payments { get; set; }

    public required DbSet<PropertyFeature> PropertyFeatures { get; set; }

    public required DbSet<RealEstateProperty> Properties { get; set; }

    public required DbSet<RealEstateSupport> Supports { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
              .HasOne(applicationUser => applicationUser.ProfileImage)
              .WithOne(image => image.User)
              .HasForeignKey<RealEstateImage>(image => image.UserId)
              .HasPrincipalKey<ApplicationUser>(applicationUser => applicationUser.Id)
              .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ApplicationUser>()
               .HasMany(applicationUser => applicationUser.Properties)
               .WithOne(property => property.Agent)
               .HasPrincipalKey(applicationUser => applicationUser.Id)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PropertyDeed>()
            .HasOne(propertyDeed => propertyDeed.Image)
            .WithOne(image => image.Deed)
            .HasForeignKey<RealEstateImage>(image => image.PropertyDeedId)
            .HasPrincipalKey<PropertyDeed>(propertyDeed => propertyDeed.Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Lease>()
            .HasOne(lease => lease.Property)
            .WithOne(property => property.Lease)
            .HasForeignKey<RealEstateProperty>(property => property.LeaseId)
            .HasPrincipalKey<Lease>(lease => lease.Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Lease>()
            .HasMany(lease => lease.Persons)
            .WithOne(person => person.Lease)
            .HasPrincipalKey(lease => lease.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Lease>()
            .HasMany(lease => lease.Payments)
            .WithOne(payment => payment.Lease)
            .HasPrincipalKey(lease => lease.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
           .HasOne(property => property.Address)
           .WithOne(address => address.Property)
           .HasForeignKey<PropertyAddress>(address => address.PropertyId)
           .HasPrincipalKey<RealEstateProperty>(property => property.Id)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
           .HasOne(property => property.Location)
           .WithOne(location => location.Property)
           .HasForeignKey<PropertyLocation>(location => location.PropertyId)
           .HasPrincipalKey<RealEstateProperty>(property => property.Id)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
            .HasOne(property => property.Owner)
            .WithOne(person => person.Property)
            .HasForeignKey<Person>(person => person.PropertyId)
            .HasPrincipalKey<RealEstateProperty>(property => property.Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<RealEstateProperty>()
        .HasOne(property => property.PropertyDeed)
        .WithOne(deed => deed.Property)
        .HasForeignKey<PropertyDeed>(deed => deed.PropertyId)
        .HasPrincipalKey<RealEstateProperty>(property => property.Id)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
            .HasMany(property => property.Features)
            .WithOne(feature => feature.Property)
            .HasPrincipalKey(property => property.Id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
           .HasMany(property => property.Images)
           .WithOne(image => image.Property)
           .HasPrincipalKey(property => property.Id)
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateSupport>()
           .HasOne(realEstatesupport => realEstatesupport.Image)
           .WithOne(realEstateImage => realEstateImage.Support)
           .HasForeignKey<RealEstateImage>(realEstateImage => realEstateImage.SupportId)
           .HasPrincipalKey<RealEstateSupport>(realEstatesupport => realEstatesupport.Id)
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<RealEstateProperty>()
            .HasIndex(property => property.PlatesNumber)
            .IsUnique();

        builder.Entity<Lease>()
            .Property(lease => lease.DepositAmount)
            .HasPrecision(18, 2);

        builder.Entity<Lease>()
            .Property(lease => lease.MonthlyRent)
            .HasPrecision(18, 2);

        builder.Entity<Payment>()
            .Property(payment => payment.Amount)
            .HasPrecision(18, 2);

        builder.Entity<RealEstateProperty>()
            .Property(property => property.BuildingArea)
            .HasPrecision(18, 2);

        builder.Entity<RealEstateProperty>()
            .Property(property => property.LandArea)
            .HasPrecision(18, 2);

        builder.Entity<RealEstateProperty>()
            .Property(property => property.Price)
            .HasPrecision(18, 2);

    }
}

