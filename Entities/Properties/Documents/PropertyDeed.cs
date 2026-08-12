using RealEstate.Entities.Images.Documents;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Entities.Properties.Documents;

#pragma warning disable CA1515
public class PropertyDeed
{
    [Key]
    public Guid Id { get; set; } = new();

    public required string DeedNumber { get; set; }

    public required string RegistryNumber { get; set; }

    public DateTime IssueDate { get; set; } = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    public required string IssuedBy { get; set; }

    #region Relationships

    public PropertyDeedImage? PropertyDeedImage { get; set; }

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;

    #endregion
}
