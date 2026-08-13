namespace RealEstate.DTOs.Images.Users;

#pragma warning disable CA1515
public class UpdateDTO : BaseImageDTO
{
    #region Relationships

    public string AgentId { get; set; } = null!;

    #endregion
}
