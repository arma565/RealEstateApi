using System.ComponentModel;

namespace RealEstate.DTOs.Supports;

#pragma warning disable CA1515
public class RealEstateSupportDTO
{
    [DefaultValue("")]
    public string? Title { get; set; } = default;

    [DefaultValue("")]
    public string? DetailsTitle { get; set; } = default;

    [DefaultValue("")]
    public string? DetailsSubtitle { get; set; } = default;

    public ICollection<string> DetailsDescriptionList => [];

    #region Relationships

    public Guid? ImageId { get; set; } = null!;
    #endregion

}
