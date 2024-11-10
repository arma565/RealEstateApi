using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties { get; set; } 

    public DbSet<Person> Persons { get; set; }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
            modelBuilder.Entity<Property>()
            .HasMany(prop => prop.Persons)
            .WithOne(pers => pers.Property)
            .HasForeignKey(pers => pers.PropertyID)
            .HasPrincipalKey(prop => prop.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}