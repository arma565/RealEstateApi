using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Authentication
{
    public sealed class Change
    {
        private string _user_name = "";
        private string _current_password = "";
        private string _new_password = "";
        private string _repeat_password = "";

        [Required]
        public string UserName
        {
            get => _user_name;
            set => _user_name = value;
        }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword
        {
            get => _current_password;
            set => _current_password = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
        public string NewPassword
        {
            get => _new_password;
            set => _new_password = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The Password and Repeat New Password do not match!")]
        [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
        public string RepeatPassword
        {
            get => _repeat_password;
            set => _repeat_password = value;
        }
    }
}

