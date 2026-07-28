using RealEstate.Models.Property;
using RealEstate.Models.Property.Documents;
using RealEstate.Models.Supports;
using RealEstate.Models.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Images;

#pragma warning disable CA1515
public class RealEstateImage
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public string? ImageFileName { get; set; } = default!;

    [DefaultValue("")]
    public string? ThumbnailFileName { get; set; } = default!;

    [DefaultValue("")]
    public string? Caption { get; set; } = default;

    [DefaultValue(true)]
    public bool IsPrimary { get; set; } = default;

    [DefaultValue(0)]
    public int DisplayOrder { get; set; } = default;

    [DefaultValue("")]
    public string? AltText { get; set; } = default;


    [DefaultValue("")]
    public string? UserId { get; set; } = default;

    public ApplicationUser User { get; set; } = null!;

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = default!;

    public Guid PropertyDeedId { get; set; }
    public PropertyDeed Deed { get; set; } = default!;

    public Guid SupportId { get; set; }
    public RealEstateSupport Support { get; set; } = null!;
}
