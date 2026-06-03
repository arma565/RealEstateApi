using Microsoft.AspNetCore.Identity;
using RealEstate.Models.Authentication.Users;

#pragma warning disable CA1515
namespace RealEstate.Helper;

public sealed class PasswordHelper
{
    private readonly PasswordHasher<User> _passwordHasher;

    public PasswordHelper()
    {
        _passwordHasher = new PasswordHasher<User>();
    }

    public bool VerifyPassword(User user, string hashedPassword, string inputPassword)
    {
        // Verify the password
        return _passwordHasher.VerifyHashedPassword(user, hashedPassword, inputPassword)
            == PasswordVerificationResult.Success;
    }
}


