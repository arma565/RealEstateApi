using System.ComponentModel;

namespace RealEstate.DTOs.Supports;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue("")]
    public string? Title { get; set; }

    [DefaultValue("")]
    public string? DetailsTitle { get; set; }

    [DefaultValue("")]
    public string? DetailsSubtitle { get; set; }

    public ICollection<string> DetailsDescriptionList => [];

}
