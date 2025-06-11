using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models.Authentication
{
    public sealed class Recovery
    {
        private string _email = "";

        [Required(ErrorMessage = "Email is reqired!")]
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email
        {
            get => _email;
            set => _email = value;
        }
    }
}

