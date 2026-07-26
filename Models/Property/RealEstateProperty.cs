using RealEstate.Models.Persons;
using RealEstate.Models.Property.Addresses;
using RealEstate.Models.Property.Documents;
using RealEstate.Models.Property.Enums;
using RealEstate.Models.Property.Features;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Property;

public class RealEstateProperty()
{

    [Key]
    public Guid PropertyId { get; set; } = default;
    [DefaultValue(0)]
    public int OrderId { get; set; } = default;
    [DefaultValue("")]
    public string? Title { get; set; } = default;
    [DefaultValue("")]
    public string? Description { get; set; } = default;
    [DefaultValue(PropertyType.House)]
    public PropertyType PropertyType { get; set; } = default;
    [DefaultValue(PropertyStatus.ForRent)]
    public PropertyStatus PropertyStatus { get; set; } = default;
    [Required(ErrorMessage = "Price is required!")]
    public string? Price { get; set; } = default;
    [DefaultValue(PropertyCurrency.USD)]
    public PropertyCurrency Currency { get; set; } = default;
    [Required(ErrorMessage = "YearBuilt is required!")]
    public string? YearBuilt { get; set; } = default;
    [Required(ErrorMessage = "LandArea size is required!")]
    public decimal LandArea { get; set; } = default;
    [Required(ErrorMessage = "BuildingArea size is required!")]
    public decimal BuildingArea { get; set; } = default;
    [Required(ErrorMessage = "Address is required!")]
    public PropertyAddress? Address { get; set; } = default;
    [Required(ErrorMessage = "Location is required!")]
    public PropertyLocation? Location { get; set; } = default;
    public Guid PropertyDeedId { get; set; }
    public PropertyDeed? PropertyDeed { get; set; } = null!;
    public ICollection<PropertyFeatures> Features => [];
    public ICollection<PropertyImage> PropertyImages => [];
    public ICollection<Person> Persons => [];
}

