using RealEstate.Entities.Images.Documents;
using RealEstate.Entities.Images.Properties;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace RealEstate.Entities.Properties.Documents;

#pragma warning disable CA1515
public class PropertyDeed
{
    [Key]
    public Guid Id { get; set; } = new();

    public required long DeedNumber { get; set; }

    public required long RegistryNumber { get; set; }

    public DateTime IssueDate { get; set; } = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public required string IssuedBy { get; set; }

    #region Relationships

    public ICollection<PropertyDeedImage> PropertyDeedImages { get; } = [];

    public Guid PropertyId { get; set; }
    [JsonIgnore]
    public RealEstateProperty Property { get; set; } = null!;

    #endregion
}
