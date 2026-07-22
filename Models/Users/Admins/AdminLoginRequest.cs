using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Users;

public sealed class AdminLoginRequest
{
   private string _user_name = "";

   private string _password = "";

    [Required(ErrorMessage = "UserName is required!")]
    public string UserName
    {
        get => _user_name;
        set => _user_name = value;
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

