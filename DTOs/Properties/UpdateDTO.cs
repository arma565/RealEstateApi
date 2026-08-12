using RealEstate.Enums.Properties;
using System.ComponentModel;

namespace RealEstate.DTOs.Properties;

#pragma warning disable CA1515
public class UpdateDTO()
{
    [DefaultValue("")]
    public string? Title { get; set; }

    [DefaultValue("")]
    public string? Description { get; set; }

    [DefaultValue(PropertyType.NotSet)]
    public PropertyType PropertyType { get; set; }

    [DefaultValue(PropertyStatus.NotSet)]
    public PropertyStatus PropertyStatus { get; set; } = default;

    [DefaultValue(PropertyCurrency.NotSet)]
    public PropertyCurrency PropertyCurrency { get; set; } = default;

    [DefaultValue(0)]
    public int? YearBuilt { get; set; }

    [DefaultValue(0.0)]
    public decimal? Price { get; set; }

    [DefaultValue(0.0)]
    public decimal? LandArea { get; set; }

    [DefaultValue(0.0)]
    public decimal? BuildingArea { get; set; }

    #region Relationships

    public Guid OwnerId { get; set; }

    public string AgentId { get; set; } = null!;

    #endregion

}

