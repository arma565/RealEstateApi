
using RealEstate.Enums.Persons;
using RealEstate.Models.Property;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Persons;

public class Person()
{
    [Key]
    public Guid Id { get; set; }

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

    public Guid PropertyId { get; set; }
    public RealEstateProperty? Property { get; set; } = null!;

    public Guid LeaseId { get; set; } = default!;
    public Lease Lease { get; set; } = default!;
}

