using Microsoft.AspNetCore.Identity;

namespace RealEstate.Data
{
    public sealed class UserProfileIdentity : IdentityUser
    {
        public string? ProfileImageName { get; set; }
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
        public bool AcceptTerms { get; set; }
    }
}