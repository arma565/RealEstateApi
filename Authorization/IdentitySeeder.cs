using Microsoft.AspNetCore.Identity;

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
}
