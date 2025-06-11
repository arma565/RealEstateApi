using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class Login
    {
       private string _user_name = "";

       private string _password = "";

        [Required]
        public string UserName
        {
            get => _user_name;
            set => _user_name = value;
        }

        [Required]
        [MinLength(8,ErrorMessage = "The password must be more than 8 characters!")]
        [DataType(DataType.Password)]
        public string Password
        {
            get => _password;
            set => _password = value;
        }
    }
}

