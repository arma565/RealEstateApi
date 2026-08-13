using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Users;

#pragma warning disable CA1515
public class ResetPasswordDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public required string Email { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "Token is required!")]
    public required string Token { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "NewPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The newPassword must be more than 8 characters!")]
    public required string NewPassword { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "RepeatNewPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The repeatNewPassword must be more than 8 characters!")]
    [Compare("NewPassword", ErrorMessage = "The newPassword and repeatNewPassword do not match!")]
    public required string RepeatNewPassword { get; set; } 
}

