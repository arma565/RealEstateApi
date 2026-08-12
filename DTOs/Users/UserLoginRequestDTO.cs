using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Users;

#pragma warning disable CA1515
public class UserLoginRequestDTO
{
    [DefaultValue("")]
    [Required(ErrorMessage = "UserName is required!")]
    public required string UserName { get; set; } 

    [DefaultValue("")]
    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
    [DataType(DataType.Password)]
    public required string Password { get; set; } 
}

