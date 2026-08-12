using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Images.Properties;

#pragma warning disable CA1515
public class CreateDTO : BaseImageDTO
{
    [Required(ErrorMessage = "ImageFilePath is required!")]
    public required override string? ImageFilePath { get => base.ImageFilePath; set => base.ImageFilePath = value; }

    [Required(ErrorMessage = "ThumbnailFilePath is required!")]
    public required override string? ThumbnailFilePath { get => base.ThumbnailFilePath; set => base.ThumbnailFilePath = value; }

    [DefaultValue("")]
    public int OrderId { get; set; }

    [DefaultValue(false)]
    public bool IsCoverImage { get; set; }

    #region Relationships

    [Required(ErrorMessage = "PropertyId is required!")]
    public required Guid PropertyId { get; set; }

    #endregion

}