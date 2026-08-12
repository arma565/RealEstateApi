using RealEstate.Enums.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RealEstate.DTOs.Properties;

#pragma warning disable CA1515
public class CreateDTO()
{
    [JsonIgnore]
    [DefaultValue(0)]
    public int OrderId { get; set; } = 0;

    [DefaultValue("")]
    [Required(ErrorMessage = "Title is required!")]
    public required string Title { get; set; }

    [DefaultValue("")]
    public string? Description { get; set; }

    [DefaultValue(PropertyType.NotSet)]
    [Range(0, 7, ErrorMessage = "Out of range")]
    public PropertyType PropertyType { get; set; }

    [DefaultValue(PropertyStatus.NotSet)]
    [Range(0, 5, ErrorMessage = "Out of range")]
    public PropertyStatus PropertyStatus { get; set; }

    [DefaultValue(PropertyCurrency.NotSet)]
    [Range(0, 4, ErrorMessage = "Out of range")]
    public PropertyCurrency Currency { get; set; }

    [DefaultValue(0)]
    [Required(ErrorMessage = "YearBuilt is required!")]
    public required int YearBuilt { get; set; } 

    [DefaultValue(0.0)]
    [Required(ErrorMessage = "Price is required!")]
    public required decimal Price { get; set; } 

    [DefaultValue(0.0)]
    [Required(ErrorMessage = "LandArea size is required!")]
    public required decimal LandArea { get; set; } 

    [DefaultValue(0.0)]
    [Required(ErrorMessage = "BuildingArea size is required!")]
    public required decimal BuildingArea { get; set; } 


    #region Relationships

    [Required(ErrorMessage = "OwnerId is required!")]
    public required Guid OwnerId { get; set; }

    [Required(ErrorMessage = "AgentId is required!")]
    public required string AgentId { get; set; }

    #endregion

}

