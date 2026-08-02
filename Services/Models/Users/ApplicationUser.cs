using Microsoft.AspNetCore.Identity;
using RealEstate.Services.Models.Images;
using RealEstate.Services.Models.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Users;

#pragma warning disable CA1515
public class ApplicationUser : IdentityUser
{
    [Key]
    [DefaultValue("")]
    public override string Id { get; set; } = default!;

    [DefaultValue("")]
    public override string? UserName { get; set; } = default;

    [DefaultValue("")]
    public override string? Email { get; set; } = default;

    [DefaultValue("")]
    public override string? PhoneNumber { get; set; } = default;

    [DefaultValue("")]
    public string? FirstName { get; set; } = default;

    [DefaultValue("")]
    public string? LastName { get; set; } = default;

    [DefaultValue(true)]
    public bool AcceptTerms { get; set; } = default;

    #region Relationships

    public ICollection<RealEstateProperty> Properties => [];

    public Guid ImageId { get; set; }
    public RealEstateImage ProfileImage { get; set; } = default!;
    #endregion

}

