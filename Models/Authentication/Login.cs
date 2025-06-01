using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public class Login
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
        [DataType(DataType.Password)]
        public string Password
        {
            get => password;
            set => password = value;
        }
    }
}

