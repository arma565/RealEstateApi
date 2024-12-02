using Microsoft.AspNetCore.Identity;

public class UserProfileIdentity : IdentityUser
{
    public string ProfileImagePath {get;set;} = "";
    public string FirstName {get;set;} = "";
    public string LastName {get;set;} = "";
    public bool AcceptTerms{get;set;} = false;
}