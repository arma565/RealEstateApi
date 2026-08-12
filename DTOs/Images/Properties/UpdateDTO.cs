using System.ComponentModel;

namespace RealEstate.DTOs.Images.Properties;

#pragma warning disable CA1515
public class UpdateDTO : BaseImageDTO
{

    [DefaultValue("")]
    public int OrderId { get; set; }

    [DefaultValue("")]
    public bool IsCoverImage { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }

    #endregion
}
