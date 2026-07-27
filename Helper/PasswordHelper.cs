using Microsoft.AspNetCore.Identity;
using RealEstate.Models.Users;




#pragma warning disable CA1515
namespace RealEstate.Helper;

public sealed class PasswordHelper
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher;

    public PasswordHelper()
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


