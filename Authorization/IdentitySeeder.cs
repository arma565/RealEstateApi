using Microsoft.AspNetCore.Identity;
using RealEstate.Entities.Users;

namespace RealEstate.Authorization;

#pragma warning disable CA1515
public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        [
        Roles.Admin,
        Roles.Agent
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role).ConfigureAwait(false))
            {
                await roleManager.CreateAsync(new IdentityRole(role))
                    .ConfigureAwait(false);
            }
        }
    }

    public static async Task CreateAdmin(IServiceProvider serviceProvider) {

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if admin user already exists
        var adminUser = await userManager.FindByNameAsync("Admin").ConfigureAwait(false);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = "admin@example.com",
                AcceptTerms = true
            };

            var userResult = await userManager.CreateAsync(
                adminUser,
                "Admin@123").ConfigureAwait(false);

            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", userResult.Errors.Select(e => e.Description)));
            }
        }

        // Make sure user is in Admin role
        if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin).ConfigureAwait(false))
        {
            var roleResult = await userManager.AddToRoleAsync(
                adminUser,
                Roles.Admin).ConfigureAwait(false);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
