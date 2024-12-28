using Microsoft.AspNetCore.Identity;

public class UserProfileIdentity : IdentityUser
{
    public string? ProfileImageUrl { get; set; } = "";
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public bool AcceptTerms { get; set; } = false;
}
