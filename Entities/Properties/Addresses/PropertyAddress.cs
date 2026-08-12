using System.ComponentModel.DataAnnotations;

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

    public required int PlatesNumber { get; set; }

    public required string PostalCode { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;

    #endregion
}
