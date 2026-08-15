using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Properties.Addresses;

#pragma warning disable CA1515
public class PropertyAddress
{
    [Key]
    public Guid Id { get; set; } = new();

    public required string Country { get; set; } 

    public required string Province { get; set; } 

    public required string City { get; set; }

    public required string District { get; set; }

    public required string Street { get; set; }

    public required int PlateNumber { get; set; }

    public required string PostalCode { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }
    [JsonIgnore]
    public RealEstateProperty Property { get; set; } = null!;

    #endregion
}
