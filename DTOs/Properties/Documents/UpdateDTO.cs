using System.ComponentModel;

namespace RealEstate.DTOs.Properties.Documents;

#pragma warning disable CA1515
public class UpdateDTO
{
    [DefaultValue("")]
    public long DeedNumber { get; set; }

    [DefaultValue("")]
    public long RegistryNumber { get; set; }

    [DefaultValue("")]
    public string? IssuedBy { get; set; }

    #region Relationships

    public Guid PropertyId { get; set; }

    #endregion
}
