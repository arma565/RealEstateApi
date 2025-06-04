using Microsoft.AspNetCore.Identity;

namespace RealEstate.Data
{
    public sealed class UserProfileIdentity : IdentityUser
    {
        public Uri? ProfileImageUrl { get; set; }
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
        public bool AcceptTerms { get; set; }
    }
}