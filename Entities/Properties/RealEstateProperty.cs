using RealEstate.Entities.Images.Properties;
using RealEstate.Entities.Persons.Owners;
using RealEstate.Entities.Properties.Addresses;
using RealEstate.Entities.Properties.Addresses.Map;
using RealEstate.Entities.Properties.Documents;
using RealEstate.Entities.Properties.Features;
using RealEstate.Entities.Properties.Leases;
using RealEstate.Entities.Users;
using RealEstate.Enums.Properties;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Properties;

#pragma warning disable CA1515
public class RealEstateProperty()
{

    [Key]
    public Guid Id { get; set; } = new();

    public int OrderId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public PropertyType PropertyType { get; set; }

    public PropertyStatus PropertyStatus { get; set; }

    public PropertyCurrency PropertyCurrency { get; set; }

    public required int YearBuilt { get; set; }

    public required decimal Price { get; set; }

    public required decimal LandArea { get; set; }

    public required decimal BuildingArea { get; set; }

    #region Relationships

    public Guid OwnerId { get; set; }
    [JsonIgnore]
    public Owner Owner { get; set; } = null!;

    public PropertyLocation? Location { get; set; }

    public PropertyAddress? Address { get; set; }

    public PropertyDeed? PropertyDeed { get; set; }

    public string? AgentId { get; set; }
    [JsonIgnore]
    public ApplicationUser Agent { get; set; } = null!;

    public ICollection<Lease> Leases { get; } = [];

    public ICollection<PropertyFeature> PropertyFeatures { get; } = [];

    public ICollection<PropertyImage> PropertyImages { get; } = [];

    #endregion

}

