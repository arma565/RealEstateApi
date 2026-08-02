using RealEstate.Services.Enums.Persons;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Persons;

#pragma warning disable CA1515
public class PersonDTO()
{
    [Required(ErrorMessage = "First name is required!")]
    public string? FirstName { get; set; } = default;

    [Required(ErrorMessage = "Last name is required!")]
    public string? LastName { get; set; } = default;

    [Required(ErrorMessage = "Father's name is required!")]
    public string? FatherName { get; set; } = default;

    [Required(ErrorMessage = "Birth certificate number is required!")]
    [Range(10, long.MaxValue, ErrorMessage = ("Birth Certificate Number must be greater than 10 numbers!"))]
    public long BirthCertificateNumber { get; set; } = default;

    public string? BirthCertificateIssued { get; set; } = default!;

    [Required(ErrorMessage = "National ID is required!")]
    [Range(10, long.MaxValue, ErrorMessage = ("NationalId must be greater than 10 numbers!"))]
    public long NationalId { get; set; } = default;

    public string Born { get; set; } = default!;

    [Required(ErrorMessage = "Phone is required!")]
    public string Phone { get; set; } = default!;

    [Required(ErrorMessage = "Address is required!")]
    public string Address { get; set; } = default!;

    [DefaultValue(PersonRoles.LandLord)]
    public PersonRoles Role { get; set; } = default!;

    #region Relationships

    public Guid? PropertyId { get; set; } = null!;

    public Guid? LeaseId { get; set; } = null!;
    #endregion
}

