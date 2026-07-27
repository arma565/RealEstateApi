using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RealEstate.Models.Property.Documents;

#pragma warning disable CA1515
public class PropertyDeed
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public string? DeedNumber { get; set; } = default;

    [DefaultValue("")]
    public string? RegistryNumber { get; set; } = default;

    public DateTime IssueDate { get; set; } = DateTime.Parse(DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    [DefaultValue("")]
    public string? IssuedBy { get; set; } = default;

    public Uri FileUrl { get; set; } = default!;

    public Guid PropertyId { get; set; }
    public RealEstateProperty Property { get; set; } = null!;
}
