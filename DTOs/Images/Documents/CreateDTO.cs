using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Images.Documents;

#pragma warning disable CA1515
public class CreateDTO : BaseImageDTO{

    [Required(ErrorMessage = "Image is required!")]
    public override IFormFile? Image { get => base.Image; set => base.Image = value; }

    #region Relationships

    [Required(ErrorMessage = "PropertyDeedId is required!")]
    public required Guid PropertyDeedId { get; set; }

    #endregion
}
