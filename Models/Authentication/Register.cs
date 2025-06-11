using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class Register
    {
        private string _user_name = "";
        private string _email = "";
        private string _password = "";
        private string _repeat_password = "";

        [Required]
        private bool accept_terms = false;

        [Required]
        public string UserName
        {
            get => _user_name;
            set => _user_name = value;
        }

        [Required(ErrorMessage = "Email is reqired!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email
        {
            get => _email;
            set => _email = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password do not match!")]
        [MinLength(8, ErrorMessage = "The password must be more than 8 characters!")]
        public string RepeatPassword
        {
            get => _repeat_password;
            set => _repeat_password = value;
        }
        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions!")]
        public bool AcceptTerms
        {
            get => accept_terms;
            set => accept_terms = value;
        }
    }
}

