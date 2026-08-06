using RealEstate.Entities.Images;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Supports;

#pragma warning disable CA1515
public class RealEstateSupport
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

    #region Relationships

    public Guid? ImageId { get; set; } = null!;
    public RealEstateImage? Image { get; set; } = null!;
    #endregion

}
