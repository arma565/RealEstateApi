using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Properties.Addresses.Map;

#pragma warning disable CA1515
public class PropertyLocation
{
    [Key]
    public Guid Id { get; set; } = new();

    public required double Latitude { get; set; }

    public required double Longitude { get; set; }

    #region Relationships
    
    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;

    #endregion

}
