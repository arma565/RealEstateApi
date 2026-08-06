using System.ComponentModel.DataAnnotations;

namespace RealEstate.DTOs.Users;

#pragma warning disable CA1515
public class UserLoginRequestDTO
{
   private string _userName = "";

   private string _password = "";

    [Required(ErrorMessage = "UserName is required!")]
    public string UserName
    {
        get => _userName;
        set => _userName = value;
    }

    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8,ErrorMessage = "The password must be more than 8 characters!")]
    [DataType(DataType.Password)]
    public string Password
    {
        get => _password;
        set => _password = value;
    }
}

