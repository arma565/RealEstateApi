using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserProfileIdentity>(options)
{
    public required DbSet<Property> Properties { get; set; }

    public required DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Property>()
            .HasMany(prop => prop.Persons)
            .WithOne(pers => pers.Property)
            .HasForeignKey(pers => pers.PropertyID)
            .HasPrincipalKey(prop => prop.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IdentityUserLogin<string>>().HasNoKey();

        modelBuilder.Entity<IdentityUserRole<string>>().HasNoKey();

        modelBuilder.Entity<IdentityUserToken<string>>().HasNoKey();
    }
}
