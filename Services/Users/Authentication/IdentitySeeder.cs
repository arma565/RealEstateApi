using Microsoft.AspNetCore.Identity;
using RealEstate.Entities.Users;
using RealEstate.Enums.Users.Authentications;

namespace RealEstate.Services.Users.Authentication;

#pragma warning disable CA1515
public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles =
        [
        Roles.Manager.ToString(),
        Roles.Admin.ToString(),
        Roles.Agent.ToString()
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

    public static async Task CreateManager(IServiceProvider serviceProvider)
    {

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if manager user already exists
        var managerUser = await userManager.FindByNameAsync("Manager").ConfigureAwait(false);

        if (managerUser == null)
        {
            managerUser = new ApplicationUser
            {
                UserName = "Manager",
                Email = "manager@manager.com",
                AcceptTerms = true
            };

            var userResult = await userManager.CreateAsync(
                managerUser,
                "Manager@123").ConfigureAwait(false);

            if (!userResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", userResult.Errors.Select(e => e.Description)));
            }
        }

        // Make sure user is in Manager role
        if (!await userManager.IsInRoleAsync(managerUser, Roles.Manager.ToString()).ConfigureAwait(false))
        {
            var roleResult = await userManager.AddToRoleAsync(
                managerUser,
                Roles.Manager.ToString()).ConfigureAwait(false);

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
