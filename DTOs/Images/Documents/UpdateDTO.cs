namespace RealEstate.DTOs.Images.Documents;

#pragma warning disable CA1515
public class UpdateDTO : BaseImageDTO
{
    #region Relationships

    public Guid PropertyDeedId { get; set; }

    #endregion
}
