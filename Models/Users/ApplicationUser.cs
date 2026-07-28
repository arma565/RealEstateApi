using Microsoft.AspNetCore.Identity;
using RealEstate.Models.Images;
using RealEstate.Models.Property;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Users;

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

    [Required]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions!")]
    public bool AcceptTerms { get; set; } = default;

    public ICollection<RealEstateProperty> Properties => [];

    public Guid ImageId { get; set; }
    public RealEstateImage ProfileImage { get; set; } = default!;

}

