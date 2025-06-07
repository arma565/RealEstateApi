using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class Change
    {
        private string user_name = "";
        private string current_password = "";
        private string new_password = "";
        private string repeat_password = "";

        [Required]
        public string UserName
        {
            get => user_name;
            set => user_name = value;
        }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword
        {
            get => current_password;
            set => current_password = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
        public string NewPassword
        {
            get => new_password;
            set => new_password = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The Password and Repeat New Password do not match!")]
        [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
        public string RepeatPassword
        {
            get => repeat_password;
            set => repeat_password = value;
        }
    }
}

