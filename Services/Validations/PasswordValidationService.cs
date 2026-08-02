using Microsoft.AspNetCore.Identity;
using RealEstate.Services.Models.Users;

namespace RealEstate.Services.Helpers;

#pragma warning disable CA1515
public sealed class PasswordValidationService
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;

    public PasswordValidationService()
    {
        _passwordHasher = new PasswordHasher<ApplicationUser>();
    }

    public bool VerifyPassword(ApplicationUser user, string hashedPassword, string inputPassword)
    {
        // Verify the password
        return _passwordHasher.VerifyHashedPassword(user, hashedPassword, inputPassword)
            == PasswordVerificationResult.Success;
    }
}


