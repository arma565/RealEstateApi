using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Images;

#pragma warning disable CA1515
public class BaseImage
{
    [Key]
    public Guid Id { get; set; } = new();

    public required string ImageFilePath { get; set; }

    public required string ThumbnailFilePath { get; set; }
}
