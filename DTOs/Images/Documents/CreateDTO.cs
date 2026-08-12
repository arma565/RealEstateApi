using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Images.Documents;

#pragma warning disable CA1515
public class CreateDTO : BaseImageDTO{

    [Required(ErrorMessage = "ImageFilePath is required!")]
    public required override string? ImageFilePath { get => base.ImageFilePath; set => base.ImageFilePath = value; }
   
    [Required(ErrorMessage = "ThumbnailFilePath is required!")]
    public required override string? ThumbnailFilePath { get => base.ThumbnailFilePath; set => base.ThumbnailFilePath = value; }

    #region Relationships

    [Required(ErrorMessage = "PropertyDeedId is required")]
    public required Guid PropertyDeedId { get; set; }

    #endregion
}
