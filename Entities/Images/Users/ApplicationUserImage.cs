using RealEstate.Entities.Users;

namespace RealEstate.Entities.Images.Users;

#pragma warning disable CA1515
public class ApplicationUserImage : BaseImage
{
    #region Relationships

    public string AgentId { get; set; } = null!;
    public ApplicationUser Agent { get; set; } = null!;

    #endregion
}
