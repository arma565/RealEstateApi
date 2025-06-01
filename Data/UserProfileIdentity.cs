using Microsoft.AspNetCore.Identity;

namespace RealEstate.Data
{
    public class UserProfileIdentity : IdentityUser
    {
        public Uri? ProfileImageUrl { get; set; } = new("");
        public string? FirstName { get; set; } = "";
        public string? LastName { get; set; } = "";
        public bool AcceptTerms { get; set; }
    }
}