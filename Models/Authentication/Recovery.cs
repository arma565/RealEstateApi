using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1515
namespace RealEstate.Models.Authentication
{
    public sealed class Recovery
    {
        private string _email = "";

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email
        {
            get => _email;
            set => _email = value;
        }
    }
}

