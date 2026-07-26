using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Property.Features;

#pragma warning disable CA1515
public class PropertyFeatures
{
    [Key]
    public Guid PropertyFeaturesId { get; set; } = default; 
    public Feature Feature { get; set; } = default!;
    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;
}
