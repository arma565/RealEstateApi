using RealEstate.Services.Enums.Properties;
using RealEstate.Services.Models.Images;
using RealEstate.Services.Models.Images.Properties;
using RealEstate.Services.Models.Persons;
using RealEstate.Services.Models.Properties.Addresses;
using RealEstate.Services.Models.Properties.Addresses.Map;
using RealEstate.Services.Models.Properties.Documents;
using RealEstate.Services.Models.Properties.Features;
using RealEstate.Services.Models.Properties.Leases;
using RealEstate.Services.Models.Users;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Properties;

#pragma warning disable CA1515
public class RealEstateProperty()
{

    [Key]
    public Guid Id { get; set; }

    [DefaultValue(0)]
    public int OrderId { get; set; } = default;

    [DefaultValue("")]
    public string Title { get; set; } = default!;

    [DefaultValue("")]
    public string? Description { get; set; } = default;

    [DefaultValue(0)]
    public int PlatesNumber { get; set; } = default;

    [DefaultValue(PropertyType.House)]
    public PropertyType PropertyType { get; set; } = default;

    [DefaultValue(PropertyStatus.ForRent)]
    public PropertyStatus PropertyStatus { get; set; } = default;

    [DefaultValue(0.0)]
    public decimal Price { get; set; } = default;

    [DefaultValue(PropertyCurrency.USD)]
    public PropertyCurrency Currency { get; set; } = default;

    [Required(ErrorMessage = "YearBuilt is required!")]
    public int YearBuilt { get; set; } = default;

    [Required(ErrorMessage = "LandArea size is required!")]
    public decimal LandArea { get; set; } = default;

    [Required(ErrorMessage = "BuildingArea size is required!")]
    public decimal BuildingArea { get; set; } = default;

    #region Relationships

    public Guid? AddressId { get; set; } = null!;
    public PropertyAddress? Address { get; set; } = null!;

    public Guid? LocationId { get; set; } = null!;
    public PropertyLocation? Location { get; set; } = null!;

    public Guid? OwnerId { get; set; } = null!;
    public Person? Owner { get; set; } = null!;

    public string? AgentId { get; set; } = null!;
    public ApplicationUser? Agent { get; set; } = null!;

    public Guid? PropertyDeedId { get; set; } = null!;
    public PropertyDeed? PropertyDeed { get; set; } = null!;

    public Guid? LeaseId { get; set; } = null!;
    public Lease? Lease { get; set; } = null!;

    public ICollection<PropertyFeature> Features { get; } = [];

    public ICollection<PropertyImage> Images { get; } = [];
    #endregion

}

