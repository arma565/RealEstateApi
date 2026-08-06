using RealEstate.Enums.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Properties;

#pragma warning disable CA1515
public class RealEstatePropertyDTO()
{
    [DefaultValue(1)]
    public int OrderId { get; set; } = default;

    [Required(ErrorMessage = "Title is required!")]
    public string Title { get; set; } = default!;

    [DefaultValue("")]
    public string Description { get; set; } = default!;

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

    public Guid? LocationId { get; set; } = null!;

    public Guid? OwnerId { get; set; } = null!;

    public string? AgentId { get; set; } = null!;

    public Guid? PropertyDeedId { get; set; } = null!;

    public Guid? LeaseId { get; set; } = null!;

    #endregion

}

