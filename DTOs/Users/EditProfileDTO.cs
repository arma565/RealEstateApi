using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Users;

#pragma warning disable CA1515
public class EditProfileDTO
{
    [DefaultValue("")]
    public string? UserName { get; set; }

    [DefaultValue("")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string? Email { get; set; }

    [DefaultValue("")]
    public string? PhoneNumber { get; set; }

    [DefaultValue("")]
    public string? FirstName { get; set; }

    [DefaultValue("")]
    public string? LastName { get; set; }
}

