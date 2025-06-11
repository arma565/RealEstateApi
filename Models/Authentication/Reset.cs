using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class Reset
    {
        private string _email = "";
        private string _token = "";
        private string _new_password = "";
        private string _repeat_new_password = "";

        [Required(ErrorMessage = "Email is reqired!")]
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
        public string RepeatNewPassword
        {
            get => _repeat_new_password;
            set => _repeat_new_password = value;
        }
    }
}

