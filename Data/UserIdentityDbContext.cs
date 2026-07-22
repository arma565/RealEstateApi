using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Models.Users;

namespace RealEstate.Data
{
    #pragma warning disable CA1515
    public class UserIdentityDbContext(DbContextOptions<UserIdentityDbContext> options) : IdentityDbContext<User>(options)
    {
        public required DbSet<UserProfileImage> UserProfileImages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(user => user.ProfileImage)
                .WithOne(profileImg => profileImg.User)
                .HasForeignKey<UserProfileImage>(profileImg => profileImg.UserID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserProfileImage>()
            .HasIndex(p => p.UserID)
            .IsUnique();
        }
    }
}
