using System.ComponentModel;

namespace RealEstate.DTOs.Persons;

#pragma warning disable CA1515
public class UpdateDTO()
{
    [DefaultValue("")]
    public string? FirstName { get; set; }

    [DefaultValue("")]
    public string? LastName { get; set; }

    [DefaultValue("")]
    public string? FatherName { get; set; }

    [DefaultValue(0)]
    public long BirthCertificateNumber { get; set; }

    [DefaultValue("")]
    public string? BirthCertificateIssued { get; set; }

    [DefaultValue(0)]
    public long NationalId { get; set; }

    [DefaultValue("")]
    public string? Born { get; set; }

    [DefaultValue("")]
    public string? Phone { get; set; }

    [DefaultValue("")]
    public string? Address { get; set; }
}

