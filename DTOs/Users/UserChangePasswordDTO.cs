using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Users;

#pragma warning disable CA1515
public class UserChangePasswordDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "UserName is required!")]
    public required string UserName { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "OldPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The oldPassword must be more than 8 characters!")]
    public required string OldPassword { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "NewPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The newPassword must be more than 8 characters!")]
    public required string NewPassword { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "RepeatNewPassword is required!")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The newPassword and repeatNewPassword do not match!")]
    [MinLength(8, ErrorMessage = "The repeatNewPassword must be more than 8 characters!")]
    public required string RepeatNewPassword { get; set; } 

}

