using System.ComponentModel.DataAnnotations;

namespace RealEstate.Entities.Users;

#pragma warning disable CA1515
public class UserChangePasswordDTO
{
    private string _user_name = "";
    private string _old_password = "";
    private string _new_password = "";
    private string _repeat_new_password = "";

    [Required(ErrorMessage = "UserName is required!")]
    public string UserName
    {
        get => _user_name;
        set => _user_name = value;
    }

    [Required(ErrorMessage = "OldPassword is required!")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "The oldPassword must be more than 8 characters!")]
    public string OldPassword
    {
        get => _old_password;
        set => _old_password = value;
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
    [Compare("NewPassword", ErrorMessage = "The newPassword and repeatNewPassword do not match!")]
    [MinLength(8, ErrorMessage = "The repeatNewPassword must be more than 8 characters!")]
    public string RepeatNewPassword
    {
        get => _repeat_new_password;
        set => _repeat_new_password = value;
    }
}

