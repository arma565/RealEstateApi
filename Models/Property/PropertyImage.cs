using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
#pragma warning disable CS0649
namespace RealEstate.Models.Property;

public sealed class PropertyImage()
{
    [Key]
    public Guid PropertyImageId { get; set; }

    public Uri? ImageUrl { get; set; } = default!;

    public Uri? ThumbnailUrl { get; set; } = default!;

    [DefaultValue("")]
    public string? Caption { get; set; } = default;

    public bool IsPrimary { get; set; }

    public int DisplayOrder { get; set; }

    [DefaultValue("")]
    public string? AltText { get; set; } = default;

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = default;

}
