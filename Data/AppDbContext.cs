using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Entities.Images.Documents;
using RealEstate.Entities.Images.Properties;
using RealEstate.Entities.Images.Supports;
using RealEstate.Entities.Images.Users;
using RealEstate.Entities.Persons;
using RealEstate.Entities.Properties;
using RealEstate.Entities.Properties.Addresses;
using RealEstate.Entities.Properties.Addresses.Map;
using RealEstate.Entities.Properties.Documents;
using RealEstate.Entities.Properties.Features;
using RealEstate.Entities.Properties.Leases;
using RealEstate.Entities.Properties.Leases.Payments;
using RealEstate.Entities.Supports;
using RealEstate.Entities.Users;
using RealEstate.Enums.Properties;
using RealEstate.Enums.Properties.Payments;

#pragma warning disable CA1515
namespace RealEstate.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public required DbSet<PropertyDeedImage> PropertyDeedImages { get; set; }

    public required DbSet<PropertyImage> PropertyImages { get; set; }

    public required DbSet<SupportImage> SupportImages { get; set; }

    public required DbSet<ApplicationUserImage> AgentImages { get; set; }

    public required DbSet<Person> Persons { get; set; }

    public required DbSet<PropertyLocation> Locations { get; set; }

    public required DbSet<PropertyAddress> Addresses { get; set; }

    public required DbSet<PropertyDeed> PropertyDeeds { get; set; }

    public required DbSet<PropertyFeature> PropertyFeatures { get; set; }

    public required DbSet<Lease> Leases { get; set; }

    public required DbSet<Payment> Payments { get; set; }

    public required DbSet<RealEstateProperty> Properties { get; set; }

    public required DbSet<RealEstateSupport> Supports { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);

        builder.Entity<Person>()
            .HasMany(person => person.Leases)
            .WithOne(lease => lease.Tenant)
            .HasForeignKey(lease => lease.TenantId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Entity<Person>()
            .HasMany(person => person.RealEstateProperties)
            .WithOne(property => property.Owner)
            .HasForeignKey(property => property.OwnerId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Entity<PropertyDeed>()
            .HasOne(propertyDeed => propertyDeed.PropertyDeedImage)
            .WithOne(propertyDeedImage => propertyDeedImage.PropertyDeed)
            .HasForeignKey<PropertyDeedImage>(propertyDeedImage => propertyDeedImage.PropertyDeedId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Entity<Lease>()
            .HasMany(lease => lease.Payments)
            .WithOne(payment => payment.Lease)
            .HasForeignKey(payment => payment.LeaseId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Entity<RealEstateProperty>()
             .HasMany(property => property.PropertyImages)
             .WithOne(propertyImage => propertyImage.Property)
             .HasForeignKey(propertyImage => propertyImage.PropertyId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();

        builder.Entity<RealEstateProperty>()
             .HasOne(property => property.Location)
             .WithOne(location => location.Property)
             .HasForeignKey<PropertyLocation>(location => location.PropertyId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();

        builder.Entity<RealEstateProperty>()
           .HasOne(property => property.Address)
           .WithOne(address => address.Property)
           .HasForeignKey<PropertyAddress>(address => address.PropertyId)
           .OnDelete(DeleteBehavior.Cascade)
           .IsRequired();

        builder.Entity<RealEstateProperty>()
            .HasOne(property => property.PropertyDeed)
            .WithOne(propertyDeed => propertyDeed.Property)
            .HasForeignKey<PropertyDeed>(propertyDeed => propertyDeed.PropertyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Entity<RealEstateProperty>()
            .HasMany(property => property.PropertyFeatures)
            .WithOne(feature => feature.Property)
            .HasForeignKey(feature => feature.PropertyId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.Entity<RealEstateProperty>()
           .HasMany(property => property.Leases)
           .WithOne(lease => lease.Property)
           .HasForeignKey(lease => lease.PropertyId)
           .OnDelete(DeleteBehavior.Cascade)
           .IsRequired();

        builder.Entity<RealEstateSupport>()
           .HasOne(realEstatesupport => realEstatesupport.SupportImage)
           .WithOne(supportImage => supportImage.Support)
           .HasForeignKey<Entities.Images.Supports.SupportImage>(supportImage => supportImage.SupportId)
           .OnDelete(DeleteBehavior.Cascade)
           .IsRequired();

        builder.Entity<ApplicationUser>()
          .HasOne(applicationUser => applicationUser.AgentImage)
          .WithOne(agentImage => agentImage.Agent)
          .HasForeignKey<ApplicationUserImage>(agentImage => agentImage.AgentId)
          .OnDelete(DeleteBehavior.Cascade)
          .IsRequired();

        builder.Entity<ApplicationUser>()
           .HasMany(applicationUser => applicationUser.RealEstateProperties)
           .WithOne(property => property.Agent)
           .HasForeignKey(property => property.AgentId)
           .OnDelete(DeleteBehavior.Cascade)
           .IsRequired();


        builder
           .Entity<PropertyAddress>()
           .HasIndex(address => address.PlatesNumber)
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



        builder.Entity<PropertyFeature>()
           .Property(property => property.PropertyFeatureCategory)
           .HasConversion(
               v => v.ToString(),
               v => Enum.Parse<PropertyFeatureCategory>(v)
           );

        builder.Entity<Payment>()
            .Property(payment => payment.PaymentType)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PaymentType>(v)
            );

        builder.Entity<RealEstateProperty>()
            .Property(property => property.PropertyType)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<PropertyType>(v)
            );

        builder.Entity<RealEstateProperty>()
          .Property(property => property.PropertyStatus)
          .HasConversion(
              v => v.ToString(),
              v => Enum.Parse<PropertyStatus>(v)
          );

        builder.Entity<RealEstateProperty>()
         .Property(property => property.PropertyCurrency)
         .HasConversion(
             v => v.ToString(),
             v => Enum.Parse<PropertyCurrency>(v)
         );
    }
}

