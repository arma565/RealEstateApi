using RealEstate.Models.Persons;
using RealEstate.Models.Property.Enums;
using RealEstate.Models.Property.Features;
using RealEstate.Models.Property.Locations;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Numerics;

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

    [Required(ErrorMessage = "ContractType is required!")]
    public string? ContractType { get; set; } = default;

    public ICollection<PropertyFeatures> Features => [];

    public ICollection<PropertyImage> PropertyImages => [];

    public ICollection<Person> Persons => [];
}

