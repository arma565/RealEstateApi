using RealEstate.Entities.Images.Supports;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Supports;

#pragma warning disable CA1515
public class RealEstateSupport
{
    [Key]
    public Guid Id { get; set; } = new();

    public required string Title { get; set; }

    public required string DetailsTitle { get; set; }

    public required string DetailsSubtitle { get; set; }

    public ICollection<string> DetailsDescriptionList => [];

    #region Relationships

    public SupportImage? SupportImage { get; set; }

    #endregion

}
