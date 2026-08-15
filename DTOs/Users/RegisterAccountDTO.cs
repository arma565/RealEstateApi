using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Users;

#pragma warning disable CA1515
public class RegisterAccountDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "UserName is required!")]
    public required string UserName { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public required string Email{ get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "Password is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
    public required string Password { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "RepeatPassword is required!")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match!")]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
    public required string RepeatPassword { get; set; } 

    [DefaultValue(true)]
    [Required]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions!")]
    public required bool AcceptTerms { get; set; }
}

