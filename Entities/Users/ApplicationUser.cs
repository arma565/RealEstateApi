using Microsoft.AspNetCore.Identity;
using RealEstate.Entities.Images.Users;
using RealEstate.Entities.Properties;
using RealEstate.Entities.Users.Authentications;
using RealEstate.Services.Users.Authentications;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Users;

#pragma warning disable CA1515
public class ApplicationUser : IdentityUser
{
    [Key]
    public override string Id { get; set; } = default!;

    public override string? UserName { get; set; }

    public override string? Email { get; set; }

    public override string? PhoneNumber { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool AcceptTerms { get; set; }

    #region Relationships

    public ApplicationUserImage? AgentImage { get; set; }

    public ICollection<RealEstateProperty> RealEstateProperties { get; } = [];

    public RefreshToken? RefreshToken { get; set; }

    #endregion

}

