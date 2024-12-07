using Microsoft.AspNetCore.Identity;

public class PasswordHelper
{
    private readonly PasswordHasher<UserProfileIdentity> _passwordHasher;

    public PasswordHelper()
    {
        _passwordHasher = new PasswordHasher<UserProfileIdentity>();
    }

    public string HashPassword(UserProfileIdentity user,string password)
    {
        // Hash the password
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(UserProfileIdentity user,string hashedPassword, string inputPassword)
    {
        // Verify the password
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, inputPassword);
        return result == PasswordVerificationResult.Success;
    }
}