using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace RealEstate.Models.Property;
#pragma warning disable CA1515
public sealed class PropertyImage()
{
    [Key]
    public Guid PropertyImageId { get; set; }
    public Uri? ImageUrl { get; set; } = default!;
    public Uri? ThumbnailUrl { get; set; } = default!;
    [DefaultValue("")]
    public string? Caption { get; set; } = default;
    [DefaultValue(true)]
    public bool IsPrimary { get; set; } = default;
    [DefaultValue(0)]
    public int DisplayOrder { get; set; } = default;
    [DefaultValue("")]
    public string? AltText { get; set; } = default;
    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = default!;

}
