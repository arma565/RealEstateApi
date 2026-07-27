#pragma warning disable CA1515
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Support;

public class SupportApp
{
    [Key]
    public Guid Id { get; set; }

    [DefaultValue("")]
    public string? Title { get; set; } = default;

    [DefaultValue("")]
    public string? DetailsTitle { get; set; } = default;

    [DefaultValue("")]
    public string? DetailsSubtitle { get; set; } = default;

    public ICollection<string> DetailsDescriptionList => [];

    public Guid SupportImageId { get; set; }
    public SupportImage SupportImage { get; set; } = null!;

}
