using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Persons;

#pragma warning disable CA1515
public class Person()
{
    [Key]
    public Guid Id { get; set; } = new();

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string FatherName { get; set; }

    public required long BirthCertificateNumber { get; set; }

    public required string BirthCertificateIssued { get; set; }

    public required long NationalId { get; set; }

    public required string Born { get; set; }

    public required string Phone { get; set; }

    public required string Address { get; set; }
}

