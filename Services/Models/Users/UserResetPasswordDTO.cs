using System.ComponentModel.DataAnnotations;

namespace RealEstate.Services.Models.Users;

#pragma warning disable CA1515
public class UserResetPasswordDTO
{
    private string _email = "";
    private string _token = "";
    private string _new_password = "";
    private string _repeat_new_password = "";

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string Email
    {
        get => _email;
        set => _email = value;
    }

    public string Token
    {
        get => _token;
        set => _token = value;
    }

    [Required(ErrorMessage = "NewPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The newPassword must be more than 8 characters!")]
    public string NewPassword
    {
        get => _new_password;
        set => _new_password = value;
    }

    [Required(ErrorMessage = "RepeatNewPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The repeatNewPassword must be more than 8 characters!")]
    [Compare("NewPassword", ErrorMessage = "The newPassword and repeatNewPassword do not match!")]
    public string RepeatNewPassword
    {
        get => _repeat_new_password;
        set => _repeat_new_password = value;
    }
}

