using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Persons;

#pragma warning disable CA1515
public class CreateDTO()
{
    [DefaultValue("")]
    [Required(ErrorMessage = "First name is required!")]
    public required string FirstName { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Last name is required!")]
    public required string LastName { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Father name is required!")]
    public required string FatherName { get; set; }

    [DefaultValue(0)]
    [Required(ErrorMessage = "Birth certificate number is required!")]
    [Range(10, long.MaxValue, ErrorMessage = ("Birth Certificate Number must be greater than 10 numbers!"))]
    public required long BirthCertificateNumber { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "BirthCertificateIssued is required!")]
    public required string BirthCertificateIssued { get; set; }

    [DefaultValue(0)]
    [Required(ErrorMessage = "National ID is required!")]
    [Range(10, long.MaxValue, ErrorMessage = ("NationalId must be greater than 10 numbers!"))]
    public required long NationalId { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Born is required!")]
    public required string Born { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Phone is required!")]
    public required string Phone { get; set; }

    [DefaultValue("")]
    [Required(ErrorMessage = "Address is required!")]
    public required string Address { get; set; }
}

