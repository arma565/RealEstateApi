
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Support;

#pragma warning disable CA1515
public class SupportImage
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public string? SupportImageFileName { get; set; } = default;

    public Guid SupportId { get; set; }
    public SupportApp Support { get; set; } = null!;
}
