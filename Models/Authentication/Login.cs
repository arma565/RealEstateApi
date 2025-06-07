using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class Login
    {
        string user_name = "";

        string password = "";

        [Required]
        public string UserName
        {
            get => user_name;
            set => user_name = value;
        }

        [Required]
        [MinLength(8,ErrorMessage = "The password must be more than 8 characters!")]
        [DataType(DataType.Password)]
        public string Password
        {
            get => password;
            set => password = value;
        }
    }
}

