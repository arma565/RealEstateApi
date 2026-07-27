using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models.Persons;
using RealEstate.Models.Property;
using RealEstate.Models.Property.Documents;
using RealEstate.Models.Support;
using RealEstate.Models.Users;

#pragma warning disable CA1515
namespace RealEstate.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public required DbSet<UserProfileImage> UserProfileImages { get; set; }

    public required DbSet<Person> Persons { get; set; }

    public required DbSet<PropertyDeed> PropertyDeeds { get; set; }

    public required DbSet<Lease> Leases { get; set; }

    public required DbSet<Payment> Payments { get; set; }

    public required DbSet<PropertyFeature> PropertyFeatures { get; set; }

    public required DbSet<PropertyImage> PropertyImages { get; set; }

    public required DbSet<RealEstateProperty> Properties { get; set; }

    public required DbSet<SupportApp> Supports { get; set; }

    public required DbSet<SupportImage> SupportImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
              .HasOne(user => user.ProfileImage)
              .WithOne(profileImg => profileImg.User)
              .HasForeignKey<UserProfileImage>(profileImg => profileImg.UserId)
              .HasPrincipalKey<ApplicationUser>(user => user.Id)
              .IsRequired()
              .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasMany(user => user.Properties)
            .WithOne(property => property.Agent)
            .HasForeignKey(property => property.AgentId)
            .HasPrincipalKey(property => property.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<Person>()
            .HasOne(person => person.Property)
            .WithOne(property => property.Owner)
            .HasForeignKey<Person>(person => person.PropertyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Person>()
            .HasOne(person => person.Lease)
            .WithMany(lease => lease.Persons)
            .HasForeignKey(person => person.LeaseId)
            .HasPrincipalKey(person => person.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PropertyDeed>()
            .HasOne(propertyDeed => propertyDeed.Property)
            .WithOne(property => property.PropertyDeed)
            .HasForeignKey<PropertyDeed>(propertyDeed => propertyDeed.PropertyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Lease>()
            .HasOne(lease => lease.Property)
            .WithOne(property => property.Lease)
            .HasForeignKey<Lease>(lease => lease.PropertyId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Payment>()
            .HasOne(payment => payment.Lease)
            .WithMany(lease => lease.Payments)
            .HasForeignKey(payment => payment.LeaseId)
            .HasPrincipalKey(payment => payment.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PropertyFeature>()
            .HasOne(propertyFeature => propertyFeature.Property)
            .WithMany(property => property.Features)
            .HasForeignKey(propertyFeature => propertyFeature.PropertyId)
            .HasPrincipalKey(propertyFeature => propertyFeature.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PropertyImage>()
            .HasOne(propertyImage => propertyImage.Property)
            .WithMany(property => property.PropertyImages)
            .HasForeignKey(propertyImage => propertyImage.PropertyId)
            .HasPrincipalKey(propertyImage => propertyImage.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Entity<SupportApp>()
            .HasOne(support => support.SupportImage)
            .WithOne(supportImg => supportImg.Support)
            .HasForeignKey<SupportImage>(supportImg => supportImg.SupportId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}

