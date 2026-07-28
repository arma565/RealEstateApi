using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models.Images;
using RealEstate.Models.Persons;
using RealEstate.Models.Property;
using RealEstate.Models.Property.Documents;
using RealEstate.Models.Supports;
using RealEstate.Models.Users;
using static System.Net.Mime.MediaTypeNames;

#pragma warning disable CA1515
namespace RealEstate.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public required DbSet<RealEstateImage> Images { get; set; }

    public required DbSet<Person> Persons { get; set; }

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
              .IsRequired()
              .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
               .HasMany(applicationUser => applicationUser.Properties)
               .WithOne(property => property.Agent)
               .HasPrincipalKey(applicationUser => applicationUser.Id)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateImage>()
           .HasOne(realEstateImage => realEstateImage.Support)
           .WithOne(realEstateSupport => realEstateSupport.Image)
           .HasForeignKey<RealEstateSupport>(realEstateSupport => realEstateSupport.ImageId)
           .HasPrincipalKey<RealEstateImage>(realEstateImage => realEstateImage.Id)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Person>()
           .HasOne(person => person.Property)
           .WithOne(property => property.Owner)
           .HasForeignKey<RealEstateProperty>(property => property.OwnerId)
           .HasPrincipalKey<Person>(person => person.Id)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PropertyDeed>()
            .HasOne(propertyDeed => propertyDeed.Image)
            .WithOne(image => image.Deed)
            .HasForeignKey<RealEstateImage>(image => image.PropertyDeedId)
            .HasPrincipalKey<PropertyDeed>(propertyDeed => propertyDeed.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PropertyDeed>()
            .HasOne(propertyDeed => propertyDeed.Property)
            .WithOne(property => property.PropertyDeed)
            .HasForeignKey<RealEstateProperty>(property => property.PropertyDeedId)
            .HasPrincipalKey<PropertyDeed>(propertyDeed => propertyDeed.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Lease>()
            .HasOne(lease => lease.Property)
            .WithOne(property => property.Lease)
            .HasForeignKey<RealEstateProperty>(property => property.LeaseId)
            .HasPrincipalKey<Lease>(lease => lease.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Lease>()
            .HasMany(lease => lease.Persons)
            .WithOne(person => person.Lease)
            .HasPrincipalKey(lease => lease.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Lease>()
            .HasMany(lease => lease.Payments)
            .WithOne(payment => payment.Lease)
            .HasPrincipalKey(lease => lease.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
            .HasMany(property => property.Features)
            .WithOne(feature => feature.Property)
            .HasPrincipalKey(property => property.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<RealEstateProperty>()
           .HasMany(property => property.Images)
           .WithOne(image => image.Property)
           .HasPrincipalKey(property => property.Id)
           .IsRequired()
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<RealEstateProperty>()
            .HasIndex(property => property.PlatesNumber)
            .IsUnique();
    }
}

