using System.ComponentModel;

namespace RealEstate.DTOs.Images;

#pragma warning disable CA1515
public class BaseImageDTO
{
    [DefaultValue(null)]
    public virtual IFormFile? Image { get; set; }

    [DefaultValue("")]
    public virtual string? ImageFilePath { get; set; }

    [DefaultValue("")]
    public virtual string? ThumbnailFilePath { get; set; }
}
