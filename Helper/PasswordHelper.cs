using Microsoft.AspNetCore.Identity;
using RealEstate.Data;

namespace RealEstate.Helper
{
    public sealed class PasswordHelper
    {
        private readonly PasswordHasher<UserProfileIdentity> _passwordHasher;

        public PasswordHelper()
        {
            _passwordHasher = new PasswordHasher<UserProfileIdentity>();
        }

        public bool VerifyPassword(UserProfileIdentity user, string hashedPassword, string inputPassword)
        {
            // Verify the password
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, inputPassword)
                == PasswordVerificationResult.Success;
        }
    }
}

